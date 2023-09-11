using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody rigidBodyEnemy;
    private GameObject player;
    public float speed;
    void Start()
    {
        rigidBodyEnemy = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }
    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        rigidBodyEnemy.AddForce(lookDirection * speed);
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
