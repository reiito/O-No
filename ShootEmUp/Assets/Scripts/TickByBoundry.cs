using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickByBoundry : MonoBehaviour
{
  GameController gameController;

  private void Start()
  {
    GameObject gameControllerObject = GameObject.FindWithTag("GameController");
    gameController = gameControllerObject.GetComponent<GameController>();
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    gameController.SetDying(true);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    gameController.SetDying(false);
  }
}