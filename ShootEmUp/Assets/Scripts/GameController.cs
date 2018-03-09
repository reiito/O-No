using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ScoreManager
{
  int score;
  int highscore;

  public void InitScore()
  {
    score = 0;
    highscore = PlayerPrefs.GetInt("highscore", 0);
  }

  public void IncScore(Text scoreText)
  {
    score++;
    scoreText.text = "Score:" + score;
  }

  public void SaveHighScore(Text highScoreText)
  {
    highscore = score;
    PlayerPrefs.SetInt("highscore", highscore);
  }

  public int GetScore() { return score; }
  public int GetHighScore() { return highscore; }
}

[System.Serializable]
public class TextManager
{
  public Text scoreText;
  public Text highScoreText;
  public Text gameOverText;

  public void InitText(int score, int highScore)
  {
    scoreText.text = "Score: " + score;
    highScoreText.text = "High Score: " + highScore;
    highScoreText.enabled = false;
    gameOverText.enabled = false;
  }

  public void GameOverText()
  {
    highScoreText.enabled = true;
    gameOverText.enabled = true;
  }
}

public class GameController : MonoBehaviour
{
  public GameObject closingCircleObject;
  public float endScale = 0.75f;
  public TextManager textManager;

  ScoreManager scoreManager;
  ChangeScene sceneChanger;

  Collider2D circleCollider;
  Vector3 startCircleScale;
  Vector3 endCircleScale;
  float circleSpeed = 0.1f;
  float endCircleSpeed = 5.0f;
  float playerDyingSpeed = 0.1f;
  bool circleCloseStart;
  bool tickDying;
  bool gameOver;

  // Debugging
  bool debugging;

  private void Start()
  {
    InitialiseGame();
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
    }

    if (closingCircleObject.transform.localScale.x <= endScale + 0.05)
      gameOver = true;


    // TODO: menu/pause on escape
    if (Input.GetKey("escape"))
    {
      if (debugging)
        QuitGame();
      else //put menu here instead
        sceneChanger.ChangeToScene("Menu");
    }

    if (Input.GetKeyDown("`"))
      ToggleDebug();

    if (Input.GetKeyDown("2"))
      circleCloseStart = true;

    // On Game Over
    if (gameOver)
    {
      scoreManager.SaveHighScore(textManager.highScoreText);
      textManager.GameOverText();
    }
  }

  void InitialiseGame()
  {
    scoreManager = new ScoreManager();
    scoreManager.InitScore();
    textManager.InitText(scoreManager.GetScore(), scoreManager.GetHighScore());
    sceneChanger = this.GetComponent<ChangeScene>();
    circleCollider = closingCircleObject.GetComponent<Collider2D>();
    startCircleScale = closingCircleObject.transform.localScale;
    endCircleScale = new Vector3(endScale, endScale, 1.0f);
    circleCloseStart = false;
    tickDying = false;
    gameOver = false;
    debugging = true;
  }

  public bool GetDying() { return tickDying; }

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

  void ToggleDebug()
  {
    debugging = !debugging;
    Debug.Log("Debug: " + (debugging ? "ON" : "OFF"));
  }
}
