using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
  public float speed = 100.0f;

  public void Start()
  {
    GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
  }
}
