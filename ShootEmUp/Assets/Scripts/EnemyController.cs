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
  //float health

  public GameObject shot;
  public Transform shotSpawn;
  public float fireRate = 1.0f;

  PlayerController playerController;
  float deathDamage = 10.0f;
  float nextFire = 0.0f;

  bool comeBack;

  private void Start()
  {
    // set player controller reference
    playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    comeBack = false;
  }

  private void Update()
  {
    // fire shot with the given fire rate
    if (Time.time > nextFire)
    {
      nextFire = Time.time + fireRate; //adjust fire rate
      Instantiate(shot, new Vector3(shotSpawn.transform.position.x, shotSpawn.transform.position.y, shotSpawn.transform.position.z), transform.rotation); //spawn shot
    }
  }

  private void FixedUpdate()
  {
    if (comeBack)
      GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0) * 10;
    else
      GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0) * -10;

    if (transform.position.x <= -34)
      comeBack = true;
    else if (transform.position.x >= 34)
      comeBack = false;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // skip when colliding with boundry objects
    if (collision.tag == "Boundry")
      return;

    // TODO: damage depends on enemy type (virtual?)
    if (collision.tag == "Player") //die on player contact
    {
      Destroy(this.gameObject);
      playerController.DamagePlayer(deathDamage);
    }
  }
}
