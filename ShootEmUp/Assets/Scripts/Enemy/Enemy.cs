using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  public GameObject[] pickUps = new GameObject[2];
  // enemy specific
  public float speed;
  public float dropRate;
  public float worth;
  public int hitsLeft;

  GameController gameController;
  protected bool collided = false;

  protected GameObject player;

  // health reference
  protected float healthRate;
  protected float playerHealth;
  protected float totalPlayerHealth;

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
      FollowPlayer();
    else if (gameController.GetGameOver())
      Destroy(gameObject);

    if (playerHealth > 0)
      playerHealth = player.GetComponent<PlayerController>().GetPlayerHealth();
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Player") //die on player contact
    {
      collided = true;
      Destroy(gameObject);
    }
  }


  // Non-MonoBehaviour Functions

  // movement behaviour
  void FollowPlayer()
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
