using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBodyPlayer;
    private GameObject focalPoint;
    public GameObject powerupAIndicator;
    public float speed = 10.0f;
    public float strength = 100.0f;
    public bool hasPowerupA;
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
        if (other.CompareTag("PowerupA"))
        {
            hasPowerupA = true;
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
            powerupAIndicator.gameObject.SetActive(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerupA)
        {
            Rigidbody rigidBodyEnemy = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 fromPlayer = (collision.gameObject.transform.position - transform.position); 
            Debug.Log("Collided with " + collision.gameObject.name + " with powerup set to " + hasPowerupA);
            rigidBodyEnemy.AddForce(fromPlayer * strength, ForceMode.Impulse);
        }
    }
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerupA = false;
        powerupAIndicator.gameObject.SetActive(false);
    }
}
