using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBodyPlayer;
    private GameObject focalPoint;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;
    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    public GameObject powerupIndicator;
    public float speed = 10.0f;
    public float strength = 100.0f;
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    public bool hasPowerup;
    bool smashing = false;
    float floorY;
    void Start()
    {
        rigidBodyPlayer = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rigidBodyPlayer.AddForce(focalPoint.transform.forward * speed * verticalInput);
        if (currentPowerUp == PowerUpType.B && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchRockets();
        }
        if (currentPowerUp == PowerUpType.C && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }
    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<MissileController>().Fire(enemy.transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerupController>().powerupType;
            Destroy(other.gameObject);
            powerupIndicator.gameObject.SetActive(true);
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.A)
        {
            Rigidbody rigidBodyEnemy = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 fromPlayer = (collision.gameObject.transform.position - transform.position);
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
            rigidBodyEnemy.AddForce(fromPlayer * strength, ForceMode.Impulse);
        }
    }
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
    }
    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<EnemyController>();
        floorY = transform.position.y;
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            rigidBodyPlayer.velocity = new Vector2(rigidBodyPlayer.velocity.x, smashSpeed);
            yield return null;
        }
        while (transform.position.y > floorY)
        {
            rigidBodyPlayer.velocity = new Vector2(rigidBodyPlayer.velocity.x, -smashSpeed * 2);
            yield return null;
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }
        smashing = false;
    }
}
