using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravityStrength = 9.8f;

    public Vector3 GetGravityDirection(Vector3 position)
    {
        return (transform.position - position).normalized;
    }
}
