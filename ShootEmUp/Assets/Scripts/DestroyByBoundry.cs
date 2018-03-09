using UnityEngine;

public class DestroyByBoundry : MonoBehaviour
{
  private void OnTriggerExit2D(Collider2D collision)
  {
    Destroy(collision.gameObject);
  }
}