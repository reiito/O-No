using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
  /* TODO: have enemy follow player, shooting at time intervals
  * Process of making this enemy:
  * Action/Desire: Wants to chase/hunt player
  * Steering/Force: 
  * Locomation (physics sim): 
  */

  //TODO: different enemy types:

  public GameObject shot;
  public Transform shotSpawn;
  public float fireRate = 0.1f;

  GameController gameController;
  GameObject player;
  float deathDamage = 10.0f;
  float nextFire = 0.0f;
  float speed;
  int hitsLeft;
  bool playerHit;

  // getters
  public int GetHitsLeft() { return hitsLeft; }

  // utility
  public void Hit() { hitsLeft--; }

  private void Start()
  {
    // set up references
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    player = GameObject.FindWithTag("Player");
    speed = 0.15f;
    hitsLeft = 1;
    playerHit = false;
  }

  private void Update()
  {
    // fire shot with the given fire rate
    //if (Time.time > nextFire)
    //{
    //  nextFire = Time.time + fireRate; //adjust fire rate
    //  Instantiate(shot, new Vector3(shotSpawn.transform.position.x, shotSpawn.transform.position.y, shotSpawn.transform.position.z), transform.rotation); //spawn shot
    //}

    // follow player
    if (player && !gameController.GetGamePaused())
      FollowPlayer();
    else if (gameController.GetGameOver())
      Destroy(gameObject);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // TODO: damage depends on enemy type (virtual?)
    if (collision.tag == "Player") //die on player contact
    {
      Destroy(gameObject);
      if (!playerHit)
      {
        player.GetComponent<PlayerController>().DamagePlayer(deathDamage);
        playerHit = true;
      }
    }
  }

  void FollowPlayer()
  {
    // face player
    Vector3 difference = player.GetComponent<Transform>().position - transform.position;
    float rotationZ = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) + -90.0f;
    transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

    //static follow
    transform.position = Vector3.MoveTowards(transform.position, player.GetComponent<Transform>().position, speed);
    // lerp follow
    //transform.position = Vector3.Lerp(transform.position, playerTransform.position, Time.deltaTime / speed);
  }
}
