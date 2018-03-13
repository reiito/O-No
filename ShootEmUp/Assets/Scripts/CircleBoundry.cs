﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundry : MonoBehaviour
{
  GameController gameController;

  private void Start()
  {
    // set game controller reference
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    // player leaves the circle, KILL(slowly)
    if (collision.tag == "Player")
      gameController.SetDying(true);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // player re-enters the circle, ~safe
    if (collision.tag == "Player")
      gameController.SetDying(false);
  }
}