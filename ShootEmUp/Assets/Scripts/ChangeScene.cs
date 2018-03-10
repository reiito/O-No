using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
  public void ChangeToScene(string scene)
  {
    SceneManager.LoadScene(scene);
  }

  public void CheckToggle(Toggle toggle)
  {
    if (toggle.isOn)
      PlayerPrefs.SetInt("staticControls", 1);
    else
      PlayerPrefs.SetInt("staticControls", 0);
  }
}
