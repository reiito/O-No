using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotController : MonoBehaviour
{
  public float speed = 100.0f;
  public float shotDamage = 10.0f;

  GameController gameController;
  PlayerController playerController;
  bool playerHit = false;

  public void Start()
  {
    // set up references
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    if (!gameController.GetGameOver())
      playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    // move shot in its forward facing direction
    GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // skip when colliding with boundry objects
    if (collision.tag == "Boundry")
      return;

    // takes health away from player
    if (collision.tag == "Player" && !gameController.GetGameOver())
    {
      playerHit = true;
      Destroy(gameObject);
    }
  }

  // destroy shot on boundry exit
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "Boundry")
      Destroy(gameObject);
  }

  private void OnDestroy()
  {
    if (playerHit)
      playerController.DamagePlayer(shotDamage);
  }
}
