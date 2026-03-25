using UnityEngine;

public class PowerupFloat : MonoBehaviour
{
    public float floatAmplitude = 0.1f;
    public float floatSpeed = 1.2f;
    public float rotationSpeed = 45f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
