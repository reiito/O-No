using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  public Boundry boundry;
  public float thrustForce = 50.0f;

  public GameObject shot;
  public Transform shotSpawn;
  public float fireRate = 1.0f;
  public int powerShot = 3;

  GameController gameController;
  PowerController powerController;
  float rotationSize;
  float rotationSpeed;
  float playerHealth;
  float dyingSpeed = 2.0f;
  bool powerInUse;
  bool staticControls;
  float nextFire = 0.0f;

  // getters
  //public int GetPowerShot() { return powerShot; }
  public float GetPlayerHealth() { return playerHealth; }

  private void Start()
  {
    // set game controller reference
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    powerController = GameObject.Find("PowerCircle").GetComponent<PowerController>();

    rotationSize = 250.0f;
    rotationSpeed = 0.75f;

    powerInUse = false;

    // set control type from menu to use on player
    staticControls = Convert.ToBoolean(PlayerPrefs.GetInt("staticControls", 0));

    // initialise player health & slider
    playerHealth = 100.0f;
    gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
  }

  private void Update()
  {
    // fire shot with the given fire rate
    if (Input.GetButton("Jump") && Time.time > nextFire)
    {
      nextFire = Time.time + fireRate; //adjust fire rate
      Instantiate(shot, new Vector3(shotSpawn.transform.position.x, shotSpawn.transform.position.y, shotSpawn.transform.position.z), transform.rotation); //spawn shot
    }

    if (Input.GetKeyDown("z") && powerShot != 0)
    {
      powerInUse = true;
      powerShot--;
      gameController.uiManager.UpdatePowerShotText(powerShot);
    }

    if (powerInUse)
      powerInUse = powerController.PowerShoot();

    // chip away at the player's health while they're outside the circle
    if (gameController.GetDying())
    {
      playerHealth -= Time.deltaTime * dyingSpeed;
      gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
    }

    // game over check
    if (playerHealth <= 0)
      KillPlayer();

    // debug mode check
    if (gameController.GetDebug())
      DebugControls();
  }

  private void FixedUpdate()
  {
    // rotate player
    transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * rotationSize * (Time.deltaTime / rotationSpeed));
    // switch control type
    if (!staticControls) //move player
      GetComponent<Rigidbody2D>().AddForce(transform.up * thrustForce * Input.GetAxis("Vertical"));
    else
      GetComponent<Rigidbody2D>().velocity = transform.up * thrustForce * Input.GetAxis("Vertical");

    // Set boundries
    GetComponent<Rigidbody2D>().position = new Vector2
    (
      Mathf.Clamp(GetComponent<Rigidbody2D>().position.x, boundry.leftSide, boundry.rightSide),
      Mathf.Clamp(GetComponent<Rigidbody2D>().position.y, boundry.botSide, boundry.topSide)
    );
  }

  void KillPlayer()
  {
    gameController.SetGameOver(true);
    Destroy(gameObject);
  }

  void DebugControls()
  {
    // kill switch
    if (Input.GetKeyDown("1") && playerHealth > 0.0f)
    {
      playerHealth = 0.0f;
      gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
      Debug.Log("[DEBUG] Player Manually Killed");
    }

    // control type toggle
    if (Input.GetKeyDown("3"))
    {
      staticControls = !staticControls;
      Debug.Log("[DEBUG] Control Type: " + (staticControls ? "STATIC" : "FLOATY"));
    }
  }

  public void DamagePlayer(float damage)
  {
    playerHealth -= damage;
    gameController.uiManager.UpdatePlayerHealthSlider(playerHealth);
  }
}
