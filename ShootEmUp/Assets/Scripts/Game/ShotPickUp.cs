using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPickUp : PickUp
{
  private void OnDestroy()
  {
    if (pickedUp)
    {
      if (playerReference.GetShotType())
        playerReference.ResetShotTimeLeft();
      else
        playerReference.SetShotType(true);
    }
  }
}

