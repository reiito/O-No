using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUp
{
  public float healthValue = 10.0f;

  private void OnDestroy()
  {
    if (pickedUp)
      playerReference.AddHealth(healthValue);
  }
}
