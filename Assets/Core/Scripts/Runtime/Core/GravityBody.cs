using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public PlanetGravity planet;
    public float alignSpeed = 10f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (!planet) return;

        // Gravity force
        Vector3 gravityDir = planet.GetGravityDirection(rb.position);
        rb.AddForce(gravityDir * planet.gravityStrength, ForceMode.Acceleration);

        // Align "up" to opposite of gravity
        Quaternion targetRotation =
            Quaternion.FromToRotation(transform.up, -gravityDir) * rb.rotation;

        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, targetRotation, alignSpeed * Time.fixedDeltaTime)
        );
    }
}
