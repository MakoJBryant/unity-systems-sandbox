using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 10f; // degrees per second

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
