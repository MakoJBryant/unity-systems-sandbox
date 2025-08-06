using UnityEngine;

public class OrbitingBody : MonoBehaviour
{
    public Transform orbitCenter;
    public float orbitRadius = 10f;
    public float orbitSpeed = 10f;
    private float angle;

    void Start()
    {
        if (orbitCenter != null)
        {
            orbitRadius = Vector3.Distance(transform.position, orbitCenter.position);
        }
    }

    void Update()
    {
        if (!orbitCenter) return;

        angle += orbitSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
        transform.position = orbitCenter.position + offset;

        // Removed transform.LookAt(orbitCenter) to prevent planet from facing the sun
    }
}
