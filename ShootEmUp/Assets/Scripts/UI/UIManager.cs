using UnityEngine.UI;

[System.Serializable]
public class UIManager
{
  public Text scoreText;
  public Text powerShotText;
  public Slider playerHealthSlider;
  public Slider shotTimeOutSlider;
  public Text titleText;
  public Text endText;
  public Button restartButton;
  public Button menuButton;
  public Image pausePanel;

  // updaters
  public void UpdateScoreText(int score) { scoreText.text = "Score: " + score; }
  public void UpdatePowerShotText(int amount) { powerShotText.text = "Shockwave: " + amount; }
  public void UpdatePlayerHealthSlider(float newHealth) { playerHealthSlider.value = newHealth; }
  public void UpdateShotTimeOutSlider(float timeLeft) { shotTimeOutSlider.value = timeLeft; }

  public void InitUI(float playerHealth, int powerAmount, float timeLimit)
  {
    // game information
    UpdateScoreText(0);
    UpdatePowerShotText(powerAmount);
    UpdatePlayerHealthSlider(playerHealth);

    // shot metre
    shotTimeOutSlider.maxValue = timeLimit;
    shotTimeOutSlider.value = shotTimeOutSlider.maxValue;
    shotTimeOutSlider.gameObject.SetActive(false);

    // menu items
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
