using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform center;
    public float orbitRadius = 10f;
    public float orbitSpeed = 10f;
    private float angle;

    void Update()
    {
        if (center == null) return;

        angle += orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius;
        transform.position = center.position + new Vector3(x, 0, z);
    }
}
