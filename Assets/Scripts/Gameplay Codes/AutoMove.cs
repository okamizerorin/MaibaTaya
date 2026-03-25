using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AutoMove : MonoBehaviour
{
    public float speed = 7f;
    public float accel = 0.02f;
    public float accelMultiplier = 1.002f;
    public float maxSpeed = 25f;

    //old script for platform spawning
    void Update()
    {
        // gradual acceleration epic fail
        accel *= accelMultiplier;
        speed += accel * Time.deltaTime;
        speed = Mathf.Min(speed, maxSpeed);

        transform.position += Vector3.back * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
