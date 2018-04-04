using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrEnemy : Enemy
{
  public GameObject[] pickUps = new GameObject[2];

  float dropRate;

  private void Awake()
  {
    speed = 0.1f;
    dropRate = 0.4f;
    scoreWorth = 20.0f;
    damageToPlayer = 10.0f;
    hitsLeft = 3;
  }

  private void OnDestroy()
  {
    if (gameController.GetGameOver())
      return;

    // increase drop rate of health if player health is low
    if (playerHealth < totalPlayerHealth / 2)
      healthRate = 0.75f;
    else
      healthRate = 0.5f;

    // overall drop rate
    if (SpawnChance.SpawnRatio(dropRate))
    {
      if (SpawnChance.SpawnRatio(healthRate))
        Instantiate(pickUps[0], transform.position, new Quaternion(0, 0, 0, 0));
      else
        Instantiate(pickUps[1], transform.position, new Quaternion(0, 0, 0, 0));
    }
  }
}
