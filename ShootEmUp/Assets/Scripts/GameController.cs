using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ScoreManager
{
  int score;
  int highScore;

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

  public int GetScore() { return score; }
  public int GetHighScore() { return highScore; }
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

  public void InitUI(float playerHealth)
  {
    scoreText.text = "Score: " + 0;
    UpdatePlayerHealthSlider(playerHealth);
    endSubText.text = "High Score: " + 0;
    endText.enabled = false;
    endSubText.enabled = false;
    restartButton.gameObject.SetActive(false);
    menuButton.gameObject.SetActive(false);
  }

  public void UpdatePlayerHealthSlider(float newHealth)
  {
    playerHealthSlider.value = newHealth;
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
  public PlayerController playerController;
  public GameObject closingCircleObject;
  public float endScale = 0.75f;
  public UIManager uiManager;

  ScoreManager scoreManager;
  ChangeScene sceneChanger;

  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed = 0.1f;
  float endCircleSpeed = 5.0f;
  bool circleCloseStart;
  bool tickDying;
  bool gameOver;
  bool won;

  // Debugging
  bool debugging;

  private void Start()
  {
    playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    InitializeGame();
  }

  private void Update()
  {
    // Close in on the end scale then expand at the end of the game 
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

    // Circle closed and player wins
    if (closingCircleObject.transform.localScale.x <= endScale + 0.05)
    {
      gameOver = true;
      won = true;
    }

    // On Game Over
    if (gameOver)
    {
      scoreManager.SaveHighScore(uiManager.endSubText);
      uiManager.GameOverUI(won, scoreManager.GetHighScore());
      uiManager.DisableStartUI();
    }

    // Debug Toggle
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

  public bool GetDying() { return tickDying; }
  public bool GetDebug() { return debugging; }

  public void SetDying(bool value) { tickDying = value; }
  public void SetGameOver(bool result) { gameOver = result; }

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
