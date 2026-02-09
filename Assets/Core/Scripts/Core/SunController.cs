using UnityEngine;

public class SunController : MonoBehaviour
{
    [Header("Planet Reference")]
    public Transform planet; // center of rotation

    [Header("Time Settings")]
    [Tooltip("Degrees per second for full rotation")]
    public float rotationSpeed = 1f;

    private Vector3 rotationAxis;

    void Start()
    {
        if (planet == null)
        {
            Debug.LogWarning("SunController: Planet not assigned! Please assign the planet Transform.");
            // Default axis if no planet is assigned
            rotationAxis = Vector3.up;
        }
        else
        {
            // Use planet's up vector as rotation axis
            rotationAxis = planet.up;
        }
    }

    void Update()
    {
        if (planet == null)
            return;

        // Rotate the sun around the planet's center
        transform.RotateAround(
            planet.position,
            rotationAxis,
            rotationSpeed * Time.deltaTime
        );

        // Keep the sun pointing at the planet
        transform.LookAt(planet.position);
    }
}
