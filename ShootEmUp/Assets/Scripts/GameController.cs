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
  public void AddScore(int addValue) { score = score + addValue; }

  public void InitScore()
  {
    score = 0;
    highScore = PlayerPrefs.GetInt("highScore", 0);
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
  public Text powerShotText;
  public Slider playerHealthSlider;
  public Text titleText;
  public Text endText;
  public Button restartButton;
  public Button menuButton;
  public Image pausePanel;

  // updaters
  public void UpdateScoreText(int score) { scoreText.text = "Score: " + score; }
  public void UpdatePowerShotText(int amount) { powerShotText.text = "Shockwave: " + amount; }
  public void UpdatePlayerHealthSlider(float newHealth) { playerHealthSlider.value = newHealth; }

  public void InitUI(float playerHealth, int powerAmount)
  {
    UpdateScoreText(0);
    UpdatePowerShotText(powerAmount);
    UpdatePlayerHealthSlider(playerHealth);
    endText.text = "High Score: " + 0;
    titleText.enabled = false;
    endText.enabled = false;
    restartButton.gameObject.SetActive(false);
    menuButton.gameObject.SetActive(false);
    pausePanel.enabled = false;
  }

  public void PauseUI(bool paused)
  {
    if (paused)
    {
      pausePanel.enabled = true;
      titleText.text = "Paused";
      titleText.enabled = true;
      restartButton.gameObject.SetActive(true);
      menuButton.gameObject.SetActive(true);
    }
    else
    {
      pausePanel.enabled = false;
      titleText.enabled = false;
      restartButton.gameObject.SetActive(false);
      menuButton.gameObject.SetActive(false);
    }
  }

  public void GameOverUI(bool win, int endScore)
  {
    if (!win)
    {
      titleText.text = "Game Over!";
      endText.text = "Score: " + endScore;
    }
    else
    {
      titleText.text = "Winner!";
      endText.text = "High Score: " + endScore;
    }

    titleText.enabled = true;
    endText.enabled = true;
    restartButton.gameObject.SetActive(true);
    menuButton.gameObject.SetActive(true);
  }

  public void DisableStartUI()
  {
    scoreText.enabled = false;
    powerShotText.enabled = false;
    playerHealthSlider.gameObject.SetActive(false);
  }
}

public class GameController : MonoBehaviour
{
  public GameObject closingCircleObject;
  public GameObject enemy;
  public Transform[] spawnPoints = new Transform[8];
  public float endScale = 0.75f;
  public UIManager uiManager;
  public ScoreManager scoreManager;

  PlayerController playerController;

  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed = 0.25f;
  float endCircleSpeed = 5.0f;
  bool circleCloseStart;
  bool tickDying;
  bool gamePaused;
  bool gameOver;
  bool won;
  int enemiesSpawned;

  // debugging
  bool debugging;

  // getters
  public bool GetDying() { return tickDying; }
  public bool GetGamePaused() { return gamePaused; }
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

    StartCoroutine(SpawnEnemies());
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
      scoreManager.SaveHighScore(uiManager.endText);
      uiManager.GameOverUI(won, scoreManager.GetScore());
      uiManager.DisableStartUI();
    }

    if (enemiesSpawned > 10)
      circleCloseStart = true;

    // debug toggle
    if (Input.GetKeyDown("`"))
    {
      debugging = !debugging;
      Debug.Log("Debug: " + (debugging ? "ON" : "OFF"));
    }

    // TODO: menu/pause on escape
    if (debugging)
      DebugControls();
    else if (Input.GetKeyDown("escape"))
    {
      if (gamePaused)
        ResumeGame();
      else
        PauseGame();
      uiManager.PauseUI(gamePaused);
    }
  }

  void InitializeGame()
  {
    scoreManager = new ScoreManager();
    scoreManager.InitScore();
    uiManager.InitUI(playerController.GetPlayerHealth(), playerController.powerShot);
    startCircleScale = closingCircleObject.transform.localScale;
    endCircleScale = new Vector3(endScale, endScale, 1.0f);
    circleCloseStart = false;
    tickDying = false;
    gamePaused = false;
    gameOver = false;
    won = false;
    debugging = true;
    enemiesSpawned = 0;
  }

  IEnumerator SpawnEnemies()
  {
    while (!gameOver)
    {
      Instantiate(enemy, spawnPoints[Random.Range(0, spawnPoints.Length)]);
      enemiesSpawned++;
      yield return new WaitForSeconds(2.0f);
    }
  }

  void ResumeGame()
  {
    Time.timeScale = 1.0f;
    gamePaused = false;
  }

  void PauseGame()
  {
    Time.timeScale = 0.0f;
    gamePaused = true;
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
