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
    public bool hasPowerup;
    void Start()
    {
        rigidBodyPlayer = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rigidBodyPlayer.AddForce(focalPoint.transform.forward * speed * verticalInput);
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
}
