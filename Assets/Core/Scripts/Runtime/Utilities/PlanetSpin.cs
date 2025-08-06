using UnityEngine;

public class PlanetSpin : MonoBehaviour
{
    public float spinSpeed = 10f; // Degrees per second

    void Update()
    {
        // Rotate around local Y axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
