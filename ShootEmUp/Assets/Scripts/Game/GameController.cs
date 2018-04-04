using System.Collections;
using UnityEngine;


public class GameController : MonoBehaviour
{
  // public variables

  public UIManager uiManager;
  public ScoreManager scoreManager;
  public int enemiesToStartCircle;


  // properties

  // game system
  bool gamePaused;
  bool gameOver;
  bool won;

  // death circle
  public GameObject closingCircleObject;
  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed;
  float endCircleSpeed;
  bool circleStart;
  public float endScale = 0.75f;
  public float bossCircleScale = 1.5f;

  // player
  PlayerController playerController;
  bool tickDying;

  // enemy properties
  public GameObject[] enemies = new GameObject[2];
  public Transform[] spawnPoints = new Transform[8];
  public GameObject boss;
  public Transform bossSpawnPoint;
  int enemiesSpawned;
  bool bossSpawned;
  float popChance;

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

  // MonoBehaviour Functions

  private void Awake()
  {
    // set up game varaibles

    // game system
    scoreManager = new ScoreManager();
    scoreManager.InitScore();
    gamePaused = false;
    gameOver = false;
    won = false;
    debugging = true;

    // death circle
    circleSpeed = 0.03f;
    endCircleSpeed = 5.0f;
    startCircleScale = closingCircleObject.transform.localScale;
    endCircleScale = new Vector3(endScale, endScale, 1.0f);
    circleStart = false;

    // player
    playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    tickDying = false;

    // enemy
    enemiesSpawned = 0;
    bossSpawned = false;
    popChance = 0.75f;

    // ui
    uiManager.InitUI(playerController.GetPlayerHealth(), playerController.powerShot, playerController.shotTimeOut);

    StartCoroutine(SpawnEnemies());
  }

  private void Update()
  {
    // close in on the end scale then expand at the end of the game 
    if (!gameOver)
    {
      if (circleStart)
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

    // circle start condition
    if (enemiesSpawned > enemiesToStartCircle)
      circleStart = true;

    // boss spawn condition
    if (closingCircleObject.transform.localScale.x <= bossCircleScale)
    {
      if (!bossSpawned)
      {
        Instantiate(boss, bossSpawnPoint, transform);
        bossSpawned = true;
      }
    }

    // change shoot type and display limit
    if (playerController.GetShotType() && !gameOver)
    {
      uiManager.shotTimeOutSlider.gameObject.SetActive(true);
      uiManager.UpdateShotTimeOutSlider(playerController.GetShotTimeLeft());
      Vector3 sliderToPlayer = Camera.main.WorldToScreenPoint(playerController.gameObject.transform.position);
      sliderToPlayer.y += 60;
      uiManager.shotTimeOutSlider.gameObject.transform.position = sliderToPlayer;
    }
    else
      uiManager.shotTimeOutSlider.gameObject.SetActive(false);

    // debug toggle
    if (Input.GetKeyDown("`"))
    {
      debugging = !debugging;
      Debug.Log("[DEBUG] " + (debugging ? "on" : "off"));
    }

    // debug inputs
    if (debugging)
      DebugControls();
    else if (Input.GetKeyDown("escape")) //pause game
    {
      if (gamePaused)
        ResumeGame();
      else
        PauseGame();
      uiManager.PauseUI(gamePaused);
    }
  }


  // Non-MonoBehaviour Functions


  IEnumerator SpawnEnemies()
  {
    while (!gameOver && !bossSpawned)
    {
      // spawn random (but weighted) enemies
      if (SpawnChance.SpawnRatio(popChance))
        Instantiate(enemies[0], spawnPoints[Random.Range(0, spawnPoints.Length)].position, new Quaternion(0, 0, 0, 0));
      else
        Instantiate(enemies[1], spawnPoints[Random.Range(0, spawnPoints.Length)].position, new Quaternion(0, 0, 0, 0));
      enemiesSpawned++;
      yield return new WaitForSeconds(1.0f);
    }
  }

  // un-pause game
  void ResumeGame()
  {
    Time.timeScale = 1.0f;
    gamePaused = false;
  }

  // stop time
  void PauseGame()
  {
    Time.timeScale = 0.0f;
    gamePaused = true;
  }

  // quit game
  void QuitGame()
  {
    if (debugging)
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  // control the game via user inputs
  void DebugControls()
  {
    if (Input.GetKeyDown("escape"))
    {
      QuitGame();
      Debug.Log("[DEBUG] game closed");
    }

    if (Input.GetKeyDown("2") && !circleStart)
    {
      circleStart = true;
      Debug.Log("[DEBUG] circle started");
    }

    if (Input.GetKeyDown("4"))
    {
      Instantiate(enemies[0], spawnPoints[0].position, new Quaternion(0,0,0,0));
      Debug.Log("[DEBUG] enemy spawned at: " + spawnPoints[0].position);
    }

    if (Input.GetKeyDown("5"))
    {
      Instantiate(boss, bossSpawnPoint, transform);
      Debug.Log("[DEBUG] boss spawned at: " + bossSpawnPoint.position);
    }
  }
}
