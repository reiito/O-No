using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickByBoundry : MonoBehaviour
{
  GameController gameController;

  private void Start()
  {
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "Player")
      gameController.SetDying(true);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Player")
      gameController.SetDying(false);
  }
}