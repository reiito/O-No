using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShot : MonoBehaviour
{
  public GameObject shot;
  public Transform shotSpawn;
  public float fireRate = 1.0f;

  float nextFire = 0.0f;

  private void Update()
  {
    // fire shot with the given fire rate
    if (Input.GetKey("t") && Time.time > nextFire)
    {
      nextFire = Time.time + fireRate; //adjust fire rate
      Instantiate(shot, new Vector3(shotSpawn.transform.position.x, shotSpawn.transform.position.y, shotSpawn.transform.position.z), transform.rotation); //spawn shot
    }
  }
}
