using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
  GameController gameController;
  PlayerController playerController;
  // shot properties
  public float speed = 100.0f;
  public float shotDamage = 10.0f;

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

    // TODO: take health away from player
    if (collision.tag == "Player" && !gameController.GetGameOver())
    {
      Destroy(this.gameObject);
      playerController.DamagePlayer(shotDamage);
    }

    // TODO: enemy collision behaviour
    if (collision.tag == "Enemy")
    {
      // popcorn enemy collision action
      Destroy(this.gameObject);
      Destroy(collision.gameObject);
      gameController.scoreManager.AddScore(10);
      gameController.uiManager.UpdateScoreText(gameController.scoreManager.GetScore());

      // TODO: different actions for different enemy types
      //if (collision.gameObject.GetComponent<Enemy>().type == "pop")
      //{
      //  Destroy(collision.gameObject);
      //}
      //else
      //{
      //  collision.health -= shotDamage;
      //}
    }
  }

  // destroy shot on circle boundry exit
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "Boundry")
      Destroy(this.gameObject);
  }
}
