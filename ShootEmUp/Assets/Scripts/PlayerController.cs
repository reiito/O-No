using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundry
{
  public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
  public GameController gameController;
  public Boundry boundry;

  public GameObject shot;
  public Transform shotSpawn;
  public float fireRate = 1.0f;

  float playerHealth;
  float dyingSpeed = 2.0f;
  bool staticControls;
  float nextFire = 0.0f;
  float rotationSpeed = 150.0f;
  float thrustForce = 15.0f;

  private void Start()
  {
    // Get game controller reference
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

    // Set control type in menu to use on player
    staticControls = Convert.ToBoolean(PlayerPrefs.GetInt("staticControls", 0));

    playerHealth = 100.0f;
    gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
  }

  private void Update()
  {
    // Fire shot with the given fire rate
    if (Input.GetButton("Jump") && Time.time > nextFire)
    {
      nextFire = Time.time + fireRate; //Adjust fire rate
      Instantiate(shot, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation); //Spawn shot
    }

    // Chip away at the player's health while they're outside the circle
    if (gameController.GetDying())
    {
      playerHealth -= Time.deltaTime * dyingSpeed;
      gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
    }

    // Game over check
    if (playerHealth <= 0)
      KillPlayer();

    if (gameController.GetDebug())
      DebugControls();
  }

  private void FixedUpdate()
  {
    // Rotate player
    transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
    // Switch control type
    if (!staticControls) //Move player
      GetComponent<Rigidbody2D>().AddForce(transform.up * thrustForce * Input.GetAxis("Vertical"));
    else
      GetComponent<Rigidbody2D>().velocity = transform.up * thrustForce * Input.GetAxis("Vertical");

    // Set boundries
    GetComponent<Rigidbody2D>().position = new Vector2
    (
      Mathf.Clamp(GetComponent<Rigidbody2D>().position.x, boundry.xMin, boundry.xMax),
      Mathf.Clamp(GetComponent<Rigidbody2D>().position.y, boundry.yMin, boundry.yMax)
    );
  }

  void KillPlayer()
  {
    gameController.SetGameOver(true);
    Destroy(gameObject);
  }

  void DebugControls()
  {
    // Kill switch
    if (Input.GetKeyDown("1") && playerHealth > 0.0f)
    {
      playerHealth = 0.0f;
      gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
      Debug.Log("[DEBUG] Player Manually Killed");
    }

    // Control type toggle
    if (Input.GetKeyDown("3"))
    {
      staticControls = !staticControls;
      Debug.Log("[DEBUG] Control Type: " + (staticControls ? "STATIC" : "FLOATY"));
    }
  }

  public float GetPlayerHealth() { return playerHealth; }
}
