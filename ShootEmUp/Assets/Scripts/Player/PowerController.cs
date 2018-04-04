using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerController : MonoBehaviour
{
  Vector3 maxScale;
  Vector3 minScale;
  float expandRate;

  private void Start()
  {
    maxScale = new Vector3(15.0f, 15.0f, 15.0f);
    minScale = new Vector3(0.1f, 0.1f, 0.1f);
    expandRate = 0.25f;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Enemy")
        GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    else if (collision.tag == "Shot")
      Destroy(collision.gameObject);
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "Enemy")
      GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
  }

  public bool PowerShoot()
  {
    transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime / expandRate);
    if (transform.localScale.x >= maxScale.x - 1.0f)
    {
      transform.localScale = minScale;
      return false;
    }

    return true;
  }
}