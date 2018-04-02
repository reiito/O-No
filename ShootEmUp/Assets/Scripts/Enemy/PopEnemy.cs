﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopEnemy : Enemy
{
  private void Awake()
  {
    speed = 0.25f;
    dropRate = 0.2f;
    worth = 10.0f;
    hitsLeft = 1;
  }

  private void OnDestroy()
  {
    // increase drop rate of health if player health is low
    if (playerHealth < totalPlayerHealth / 2)
      healthRate = 0.75f;
    else
      healthRate = 0.5f;

    // overall drop rate
    if (Random.Range(0.0f, 1.0f) <= dropRate)
    {
      if (Random.Range(0.0f, 1.0f) <= healthRate)
        Instantiate(pickUps[0], transform.position, new Quaternion(0, 0, 0, 0));
      else
        Instantiate(pickUps[1], transform.position, new Quaternion(0, 0, 0, 0));
    }

    if (collided)
      player.GetComponent<PlayerController>().DamagePlayer(worth);
  }
}
