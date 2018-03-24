using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
  public float speed = 100.0f;
  public float shotDamage = 10.0f;
  public int popEnemyWorth = 10;

  GameController gameController;
  PlayerController playerController;
  bool enemyHit;

  public void Start()
  {
    // set up references
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    if (!gameController.GetGameOver())
      playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    // move shot in its forward facing direction
    GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
    enemyHit = false;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // skip when colliding with boundry objects
    if (collision.tag == "Boundry")
      return;

    if (collision.tag == "Shot")
    {
      Destroy(gameObject);
      Destroy(collision.gameObject);
    }

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

      // make sure score and enemy is only updated once
      if (!enemyHit)
      {
        if (collision.GetComponent<EnemyController>().GetHitsLeft() > 0)
          collision.GetComponent<EnemyController>().Hit();

        if (collision.GetComponent<EnemyController>().GetHitsLeft() == 0)
        {
          Destroy(collision.gameObject);
          gameController.scoreManager.AddScore(popEnemyWorth);
          gameController.uiManager.UpdateScoreText(gameController.scoreManager.GetScore());
        }

        enemyHit = true;
      }
    }
  }

  // destroy shot on boundry exit
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "Boundry")
      Destroy(gameObject);
  }
}
