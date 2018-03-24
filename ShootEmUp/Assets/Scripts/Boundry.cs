using UnityEngine;

public class Boundry : MonoBehaviour
{
  [HideInInspector]
  public float leftSide, rightSide, botSide, topSide;

  private void Start()
  {
    // set boundy limits
    leftSide = -(transform.localScale.x / 2);
    rightSide = transform.localScale.x / 2;
    botSide = -(transform.localScale.y / 2);
    topSide = transform.localScale.y / 2;
  }

  // destroy everything that exits the level boundry collider
  private void OnTriggerExit2D(Collider2D collision)
  {
    Destroy(collision.gameObject);
  }
}