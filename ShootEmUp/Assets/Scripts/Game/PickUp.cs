using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
  GameController gameController;

  protected PlayerController playerReference;
  protected bool pickedUp = false;

  float destroyTime = 5f;
  float angle = 0;
  float timeLeft;
  float speed = 2 * Mathf.PI;
  float radius = 0.025f;
  float minSpeed = 0.5f;

  float timeToggle;
  float flickerRate = 0.25f;

  private void Awake()
  {
    gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    timeLeft = destroyTime;
    Destroy(gameObject, destroyTime);
  }

  private void Update()
  {
    if (gameController.GetGameOver())
      Destroy(gameObject);

    if (timeLeft > minSpeed)
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
}
