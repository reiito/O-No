using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopEnemy : Enemy
{
  public GameObject[] pickUps = new GameObject[2];

  float dropRate;

  private void Awake()
  {
    speed = 0.25f;
    dropRate = 0.2f;
    scoreWorth = 10.0f;
    damageToPlayer = 5.0f;
    hitsLeft = 1;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Shot")
      AvoidShot(collision.gameObject.transform);
  }

  private void OnDestroy()
  {
    // increase drop rate of health if player health is low
    if (playerHealth < totalPlayerHealth / 2)
      healthRate = 0.75f;
    else
      healthRate = 0.5f;

    // overall drop rate
    if (SpawnChance.SpawnRatio(dropRate))//Random.Range(0.0f, 1.0f) <= dropRate)
    {
      if (SpawnChance.SpawnRatio(healthRate))
        Instantiate(pickUps[0], transform.position, new Quaternion(0, 0, 0, 0));
      else
        Instantiate(pickUps[1], transform.position, new Quaternion(0, 0, 0, 0));
    }
  }

  void AvoidShot(Transform shot)
  {
    Vector3 heading = shot.position - transform.position;
    float distance = heading.magnitude;
    Vector3 directionToPlayer = heading / distance;
    Vector3 sideX = new Vector3(0.01f, 0, 0);
    Vector3 sideY = new Vector3(0, 0.01f, 0);

    Vector3 left;
    Vector3 right;
    // if enemy is higher on the y axis flip
    if (transform.position.y < player.transform.position.y && transform.position.x < player.transform.position.x)
    {
      //Debug.Log("enemy: bottom left");
      left = -transform.up + sideX + sideY;
      right = -transform.up - sideX  - sideY;
    }
    else if (transform.position.y > player.transform.position.y && transform.position.x < player.transform.position.x)
    {
      //Debug.Log("enemy: top left");
      left = transform.up - sideX + sideY;
      right = transform.up + sideX - sideY;
    }
    else if (transform.position.y < player.transform.position.y && transform.position.x > player.transform.position.x)
    {
      //Debug.Log("enemy: bottom right");
      left = -transform.up - sideX - sideY;
      right = -transform.up + sideX + sideY;
    }
    else if (transform.position.y > player.transform.position.y && transform.position.x > player.transform.position.x)
    {
      //Debug.Log("enemy: top right");
      left = transform.up - sideX - sideY;
      right = transform.up + sideX + sideY;
    }
    else //deal with 0's
    {
      //Debug.Log("else");
      left = sideX + sideY;
      right = -sideX + -sideY;
    }

    if (Vector3.Angle(left, directionToPlayer) > Vector3.Angle(right, directionToPlayer))
    {
      //Debug.Log("right");
      //transform.localPosition -= dodgeX + dodgeY;
    }
    else
    {
      //Debug.Log("left");
      //transform.localPosition += dodgeX + dodgeY;
    }

  }
}
