using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  public Boundry boundry;
  public float thrustForce = 50.0f;

  public GameObject shot;
  public Transform[] shotSpawns = new Transform[3];
  public float fireRate = 1.0f;
  public int powerShot = 3;
  public float shotTimeOut = 5.0f;

  GameController gameController;
  PowerController powerController;
  float rotationSize;
  float rotationSpeed;
  float health;
  float maxHealth;
  float dyingSpeed = 2.0f;
  bool powerInUse;
  bool staticControls;
  float nextFire = 0.0f;
  bool shotType = false;
  float shotTimeLeft;

  // getters
  public float GetPlayerHealth() { return health; }
  public bool GetShotType() { return shotType; }
  public float GetShotTimeLeft() { return shotTimeLeft; }

  // setters
  public void SetShotType(bool value) { shotType = value; }

  // utitlity
  public void ResetShotTimeLeft() { shotTimeLeft = shotTimeOut; }

  private void Start()
  {
    // set game controller reference
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    powerController = GameObject.Find("PowerCircle").GetComponent<PowerController>();

    rotationSize = 250.0f;
    rotationSpeed = 0.75f;

    powerInUse = false;

    shotTimeLeft = shotTimeOut;

    // set control type from menu to use on player
    staticControls = Convert.ToBoolean(PlayerPrefs.GetInt("staticControls", 0));

    // initialise player health & slider
    maxHealth = gameController.uiManager.playerHealthSlider.maxValue;
    health = maxHealth;
    gameController.uiManager.UpdatePlayerHealthSlider(health);
  }

  private void Update()
  {
    if (shotType)
    {
      shotTimeLeft -= Time.deltaTime;
      if (shotTimeLeft <= 0)
      {
        shotType = false;
        shotTimeLeft = shotTimeOut;
      }
    }

    // fire shot with the given fire rate
    if (Input.GetButton("Jump") && Time.time > nextFire)
    {
      nextFire = Time.time + fireRate; //adjust fire rate
      Instantiate(shot, shotSpawns[0].position, shotSpawns[0].rotation); //normal spawn shot
      if (shotType) //spawn special
      {
        Instantiate(shot, shotSpawns[1].position, shotSpawns[1].rotation);
        Instantiate(shot, shotSpawns[2].position, shotSpawns[2].rotation);
      }
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
      DamagePlayer(Time.deltaTime * dyingSpeed);

    // game over check
    if (health <= 0)
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
    if (Input.GetKeyDown("1") && health > 0.0f)
    {
      health = 0.0f;
      gameController.uiManager.UpdatePlayerHealthSlider(health);
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
    health -= damage;
    gameController.uiManager.UpdatePlayerHealthSlider(health);
  }

  public void AddHealth(float value)
  {
    if (health < maxHealth)
    {
      health += value;
      if (health > maxHealth)
        health = maxHealth;
      gameController.uiManager.UpdatePlayerHealthSlider(health);
    }
  }
}
