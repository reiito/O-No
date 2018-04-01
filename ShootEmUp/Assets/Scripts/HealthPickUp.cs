using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
  public float healthValue = 10.0f;

  PlayerController playerReference;
  float destroyTime = 5f;
  bool pickedUp = false;

  float angle = 0;
  float timeLeft;
  float speed = 2 * Mathf.PI;
  float radius = 0.025f;
  float minSpeed = 0.5f;

  float timeToggle;
  float flickerRate = 0.15f;

  private void Awake()
  {
    playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    timeLeft = destroyTime;
    timeToggle = 1.0f;
    Destroy(gameObject, destroyTime);
  }

  private void Update()
  {
    if (timeLeft > 0.5)
      timeLeft -= Time.deltaTime;
    angle += Time.deltaTime * (speed / timeLeft);
    transform.position += new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

    if (timeLeft < destroyTime / 2.5)
    {
      timeToggle -= Time.deltaTime;
      if (timeToggle > 0)
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
      else
      {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        timeToggle = flickerRate;
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Player")
    {
      pickedUp = true;
      Destroy(gameObject);
    }
  }

  private void OnDestroy()
  {
    if (pickedUp)
      playerReference.AddHealth(healthValue);
  }
}
