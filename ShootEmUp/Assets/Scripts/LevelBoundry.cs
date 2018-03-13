using UnityEngine;

public class LevelBoundry : MonoBehaviour
{
  // destroy everything that exits the level boundry collider
  private void OnTriggerExit2D(Collider2D collision)
  {
    Destroy(collision.gameObject);
  }
}