using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
  // load desired scene (button press)
  public void ChangeToScene(string scene)
  {
    Time.timeScale = 1.0f;
    SceneManager.LoadScene(scene);
  }

  public void QuitGame()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  // check control toggle setting to send to next scene 
  public void CheckControlToggle(Toggle toggle)
  {
    if (toggle.isOn)
      PlayerPrefs.SetInt("staticControls", 1);
    else
      PlayerPrefs.SetInt("staticControls", 0);
  }

  public void CheckMultiplayerToggle(Toggle toggle)
  {
    if (toggle.isOn)
      PlayerPrefs.SetInt("multiplayer", 1);
    else
      PlayerPrefs.SetInt("multiplayer", 0);
  }
}
