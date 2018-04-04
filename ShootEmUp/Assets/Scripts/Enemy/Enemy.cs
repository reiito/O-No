using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  protected GameController gameController;

  protected GameObject player;

  // enemy specific
  protected float speed;
  protected float scoreWorth;
  protected float damageToPlayer;
  protected int hitsLeft;

  // health reference
  protected float healthRate;
  protected float playerHealth;
  protected float totalPlayerHealth;

  public float GetScoreWorth() { return scoreWorth; }
  public float GetDamageToPlayer() { return damageToPlayer; }
  public int GetHitsLeft() { return hitsLeft; }

  // MonoBehaviour functions

  private void Start()
  {
    // set up references
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    player = GameObject.FindWithTag("Player");
    totalPlayerHealth = playerHealth = player.GetComponent<PlayerController>().GetPlayerHealth();
  }

  private void Update()
  {
    // follow player
    if (player && !gameController.GetGamePaused())
      Follow();
    else if (gameController.GetGameOver())
      Destroy(gameObject);

    if (playerHealth > 0)
      playerHealth = player.GetComponent<PlayerController>().GetPlayerHealth();
  }


  // Non-MonoBehaviour Functions

  public void DamageEnemy()
  {
    hitsLeft--;
  }

  // movement behaviour
  protected virtual void Follow()
  {
    // face player
    Vector3 difference = player.GetComponent<Transform>().position - transform.position;
    float rotationZ = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) + -90.0f;
    transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

    // static follow
    transform.position = Vector3.MoveTowards(transform.position, player.GetComponent<Transform>().position, speed);
    // lerp follow
    //transform.position = Vector3.Lerp(transform.position, playerTransform.position, Time.deltaTime / speed);
  }
}
