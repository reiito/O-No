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
