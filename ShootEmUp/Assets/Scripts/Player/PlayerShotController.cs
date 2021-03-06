﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotController : MonoBehaviour
{
  public float speed = 100.0f;
  public float shotDamage = 10.0f;

  GameController gameController;
  PlayerController playerController;
  bool playerHit = false;
  bool enemyHit = false;

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

    if (collision.tag == "Shot")
    {
      Destroy(gameObject);
      Destroy(collision.gameObject);
    }

    // enemy collision behaviour
    if (collision.tag == "Enemy")
    {
      // make sure score and enemy is only updated once
      if (!enemyHit)
      {
        // destroy shot
        Destroy(gameObject);

        if (collision.GetComponent<Enemy>().GetHitsLeft() > 0)
          collision.GetComponent<Enemy>().DamageEnemy();

        if (collision.GetComponent<BossEnemy>())
        {
          StartCoroutine(Blink(collision.GetComponent<SpriteRenderer>()));
        }

        if (collision.GetComponent<Enemy>().GetHitsLeft() == 0)
        {
          Destroy(collision.gameObject);
          gameController.scoreManager.AddScore((int)collision.GetComponent<Enemy>().GetScoreWorth());
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

  private void OnDestroy()
  {
    if (playerHit)
      playerController.DamagePlayer(shotDamage);
  }

  IEnumerator Blink(SpriteRenderer sprite)
  {
    if (sprite.color == Color.white)
    {
      sprite.color = Color.red;
      yield return new WaitForSeconds(0.2f);
      sprite.color = Color.white;
    }
    else
    {
      sprite.color = Color.white;
      yield return new WaitForSeconds(0.2f);
    }
    sprite.color = Color.white;
  }
}
