using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 20f;
    public float jumpForce = 6f;

    [Header("Look")]
    public float mouseSensitivity = 3f;

    [Header("Grounding")]
    public LayerMask groundMask;
    public float groundCheckDistance = 1.1f;

    Rigidbody rb;
    GravityBody gravityBody;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
    }

    void Update()
    {
        HandleYaw();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }

    // =============================
    // SEB-STYLE PLAYER YAW
    // =============================
    void HandleYaw()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        transform.Rotate(transform.up * mouseX, Space.World);
    }

    // =============================
    // MOVEMENT ALONG PLANET SURFACE
    // =============================
    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 gravityDir = (transform.position - gravityBody.planet.transform.position).normalized;

        // Movement relative to player orientation (Seb-style)
        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 desiredVelocity = transform.TransformDirection(input) * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(currentVelocity, gravityDir);

        Vector3 velocityChange = desiredVelocity - surfaceVelocity;

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    // =============================
    // GROUND CHECK
    // =============================
    void CheckGround()
    {
        Vector3 gravityDir = (transform.position - gravityBody.planet.transform.position).normalized;

        isGrounded = Physics.Raycast(
            transform.position,
            -gravityDir,
            groundCheckDistance,
            groundMask
        );
    }

    // =============================
    // JUMP
    // =============================
    void HandleJump()
    {
        if (!isGrounded) return;

        if (Input.GetButtonDown("Jump"))
        {
            Vector3 gravityDir = (transform.position - gravityBody.planet.transform.position).normalized;
            rb.AddForce(-gravityDir * jumpForce, ForceMode.VelocityChange);
        }
    }
}
