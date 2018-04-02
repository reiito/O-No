using System.Collections;
using UnityEngine;


public class GameController : MonoBehaviour
{
  // public variables
  public GameObject closingCircleObject;
  public GameObject[] enemies = new GameObject[3];
  public Transform[] spawnPoints = new Transform[8];
  public float endScale = 0.75f;
  public UIManager uiManager;
  public ScoreManager scoreManager;

  // player properties
  PlayerController playerController;
  bool tickDying;

  // death circle properties
  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed = 0.25f;
  float endCircleSpeed = 5.0f;
  bool circleCloseStart;

  // game state
  bool gamePaused;
  bool gameOver;
  bool won;

  // enemy properties
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

  private void Awake()
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

    if (playerController.GetShotType())
    {
      uiManager.shotTimeOutSlider.gameObject.SetActive(true);
      uiManager.UpdateShotTimeOutSlider(playerController.GetShotTimeLeft());
      Vector3 sliderToPlayer = Camera.main.WorldToScreenPoint(playerController.gameObject.transform.position);
      sliderToPlayer.y += 60;
      uiManager.shotTimeOutSlider.gameObject.transform.position = sliderToPlayer;
    }
    else
      uiManager.shotTimeOutSlider.gameObject.SetActive(false);
  }

  void InitializeGame()
  {
    scoreManager = new ScoreManager();
    scoreManager.InitScore();
    uiManager.InitUI(playerController.GetPlayerHealth(), playerController.powerShot, playerController.shotTimeOut);
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
      Instantiate(enemies[0], spawnPoints[Random.Range(1, spawnPoints.Length)]);
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
