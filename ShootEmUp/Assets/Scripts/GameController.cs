using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager
{
  int score;
  int highScore;

  // getters
  public int GetScore() { return score; }
  public int GetHighScore() { return highScore; }

  // utility
  public void AddScore(int addValue) { score += addValue; }

  public void InitScore()
  {
    score = 0;
    highScore = PlayerPrefs.GetInt("highScore", 0);
  }

  public void IncScore(Text scoreText)
  {
    score++;
    scoreText.text = "Score: " + score;
  }

  public void SaveHighScore(Text highScoreText)
  {
    highScore = score;
    PlayerPrefs.SetInt("highScore", highScore);
  }
}

[System.Serializable]
public class UIManager
{
  public Text scoreText;
  public Slider playerHealthSlider;
  public Text endText;
  public Text endSubText;
  public Button restartButton;
  public Button menuButton;

  // updaters
  public void UpdateScoreText(int score) { scoreText.text = "Score: " + score; }
  public void UpdatePlayerHealthSlider(float newHealth) { playerHealthSlider.value = newHealth; }

  public void InitUI(float playerHealth)
  {
    UpdateScoreText(0);
    UpdatePlayerHealthSlider(playerHealth);
    endSubText.text = "High Score: " + 0;
    endText.enabled = false;
    endSubText.enabled = false;
    restartButton.gameObject.SetActive(false);
    menuButton.gameObject.SetActive(false);
  }

  public void GameOverUI(bool win, int endScore)
  {
    if (!win)
    {
      endText.text = "Game Over!";
      endSubText.text = "Score: " + endScore;
    }
    else
    {
      endText.text = "Winner!";
      endSubText.text = "High Score: " + endScore;
    }

    endText.enabled = true;
    endSubText.enabled = true;
    restartButton.gameObject.SetActive(true);
    menuButton.gameObject.SetActive(true);
  }

  public void DisableStartUI()
  {
    scoreText.enabled = false;
    playerHealthSlider.gameObject.SetActive(false);
  }
}

public class GameController : MonoBehaviour
{
  public GameObject closingCircleObject;
  public float endScale = 0.75f;
  public UIManager uiManager;
  public ScoreManager scoreManager;

  PlayerController playerController;
  ChangeScene sceneChanger;

  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed = 0.1f;
  float endCircleSpeed = 5.0f;
  bool circleCloseStart;
  bool tickDying;
  bool gameOver;
  bool won;

  // debugging
  bool debugging;

  // getters
  public bool GetDying() { return tickDying; }
  public bool GetGameOver() { return gameOver; }
  public bool GetDebug() { return debugging; }

  // setters
  public void SetDying(bool value) { tickDying = value; }
  public void SetGameOver(bool result) { gameOver = result; }

  private void Start()
  {
    // set player controller reference
    playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    // set up game varaibles
    InitializeGame();
  }

  private void Update()
  {
    // close in on the end scale then expand at the end of the game 
    if (!gameOver)
    {
      if (circleCloseStart)
        closingCircleObject.transform.localScale = Vector2.Lerp(closingCircleObject.transform.localScale, endCircleScale, Time.deltaTime * circleSpeed);
    }
    else
    {
      closingCircleObject.transform.localScale = Vector2.Lerp(closingCircleObject.transform.localScale, startCircleScale, Time.deltaTime * endCircleSpeed);
      tickDying = false;
    }

    // winning condtion (circle closed)
    if (closingCircleObject.transform.localScale.x <= endScale + 0.05)
    {
      gameOver = true;
      won = true;
    }

    // game over actions
    if (gameOver)
    {
      scoreManager.SaveHighScore(uiManager.endSubText);
      uiManager.GameOverUI(won, scoreManager.GetHighScore());
      uiManager.DisableStartUI();
    }

    // debug toggle
    if (Input.GetKeyDown("`"))
    {
      debugging = !debugging;
      Debug.Log("Debug: " + (debugging ? "ON" : "OFF"));
    }
    // TODO: menu/pause on escape
    if (debugging)
      DebugControls();
    else if (Input.GetKeyDown("escape")) //put menu here instead
      sceneChanger.ChangeToScene("Menu");
  }

  void InitializeGame()
  {
    scoreManager = new ScoreManager();
    scoreManager.InitScore();
    uiManager.InitUI(playerController.GetPlayerHealth());
    sceneChanger = this.GetComponent<ChangeScene>();
    startCircleScale = closingCircleObject.transform.localScale;
    endCircleScale = new Vector3(endScale, endScale, 1.0f);
    circleCloseStart = false;
    tickDying = false;
    gameOver = false;
    won = false;
    debugging = true;
  }

  void QuitGame()
  {
    if (debugging)
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  void DebugControls()
  {
    if (Input.GetKeyDown("escape")) //Exit
    {
      QuitGame();
      Debug.Log("[DEBUG] Game Manually Closed");
    }

    if (Input.GetKeyDown("2") && !circleCloseStart)
    {
      circleCloseStart = true;
      Debug.Log("[DEBUG] Cricle Manually Started");
    }
  }
}
