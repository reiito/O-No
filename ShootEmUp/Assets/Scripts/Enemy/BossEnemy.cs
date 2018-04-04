using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
  public Transform[] spawnPoints = new Transform[4];

  Transform endPoint;

  public GameObject shot;
  float shotRate = 0.75f;
  public GameObject pop;
  float popRate = 0.5f;
  public GameObject str;
  float strRate = 0.25f;

  private void Awake()
  {
    endPoint = GameObject.Find("BossTarget").transform;
    speed = 0.01f;
    scoreWorth = 1000.0f;
    damageToPlayer = 20.0f;
    hitsLeft = 500;

    StartCoroutine(SpawnBossStuff());
  }

  protected override void Follow()
  {
    transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed);
  }

  IEnumerator SpawnBossStuff() //...idk
  {
    while (gameObject)
    {
      float total = shotRate + popRate + strRate;
      float random = Random.Range(0, total);
      GameObject spawnedObject;
      if (random >= shotRate)
        spawnedObject = Instantiate(shot, spawnPoints[Random.Range(0, spawnPoints.Length)].position, new Quaternion(0, 0, 180, 0), transform);
      else if (random >= strRate && random <= shotRate)
        spawnedObject = Instantiate(pop, spawnPoints[Random.Range(0, spawnPoints.Length)].position, new Quaternion(0, 0, 0, 0), transform);
      else
        spawnedObject = Instantiate(str, spawnPoints[Random.Range(0, spawnPoints.Length)].position, new Quaternion(0, 0, 0, 0), transform);

      if (spawnedObject.GetComponent<PopEnemy>() || spawnedObject.GetComponent<StrEnemy>())
        spawnedObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

      yield return new WaitForSeconds(1.0f);
    }
  }
}
