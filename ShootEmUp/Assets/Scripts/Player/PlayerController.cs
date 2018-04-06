using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Boundry boundry;
  public float thrustForce = 50.0f;

  public GameObject shot;
  public Transform[] shotSpawns = new Transform[3];
  public GameObject respawn;
  public Transform respawnPos;
  public float fireRate = 1.0f;
  public int powerShot = 3;
  public float shotTimeOut = 5.0f;

  GameController gameController;
  Animator playerAnimator;
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


  // MonoBehaviour Functions

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

    playerAnimator = GetComponent<Animator>();
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
    if (Input.GetAxis("Horizontal") > 0) //turing left
      playerAnimator.SetBool("TurningRight", true);
    else if (Input.GetAxis("Horizontal") < 0)
      playerAnimator.SetBool("TurningLeft", true);
    else
    {
      playerAnimator.SetBool("TurningLeft", false);
      playerAnimator.SetBool("TurningRight", false);
    }

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

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.tag == "Enemy")
    {
      DamagePlayer(collision.GetComponent<Enemy>().GetDamageToPlayer());
      if (!collision.GetComponent<BossEnemy>())
        Destroy(collision.gameObject);
      else
      {
        Instantiate(respawn, transform.position, transform.rotation);
        StartCoroutine(RespawnWait());
      }
    }
  }


  // Non-MonoBehaviour Functions

  //
  void KillPlayer()
  {
    gameController.SetGameOver(true);
    Destroy(gameObject);
  }

  //
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

  //
  public void DamagePlayer(float damage)
  {
    health -= damage;
    gameController.uiManager.UpdatePlayerHealthSlider(health);
  }

  //
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

  IEnumerator RespawnWait()
  {
    playerAnimator.SetTrigger("Shrink");
    Instantiate(respawn, transform.position, transform.rotation);
    yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
    transform.position = respawnPos.position;
    yield return new WaitForSeconds(0.5f);
    Instantiate(respawn, transform.position, transform.rotation);
    playerAnimator.SetTrigger("Expand");
  }
}
