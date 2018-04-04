using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChance
{
  static public bool SpawnRatio(float rate)
  {
    if (Random.Range(0.0f, 1.0f) <= rate)
      return true;
    else
      return false;
  }
}
