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
    public float groundCheckDistance = 1.5f;

    Rigidbody rb;
    GravityBody gravityBody;

    bool isGrounded;
    bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();

        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleYaw();

        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
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

        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 desiredVelocity = transform.TransformDirection(input) * moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        // Separate surface & vertical velocity
        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(velocity, gravityDir);
        Vector3 verticalVelocity = Vector3.Project(velocity, gravityDir);

        Vector3 velocityChange = desiredVelocity - surfaceVelocity;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        // Preserve vertical motion (jump / gravity)
        rb.linearVelocity = surfaceVelocity + verticalVelocity;
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

        Debug.DrawRay(transform.position, -gravityDir * groundCheckDistance,
            isGrounded ? Color.green : Color.red);
    }

    // =============================
    // SEB-STYLE JUMP
    // =============================
    void HandleJump()
    {
        if (!jumpRequested) return;
        jumpRequested = false;

        if (!isGrounded) return;

        Vector3 gravityDir = (transform.position - gravityBody.planet.transform.position).normalized;

        // Clear downward gravity velocity
        rb.linearVelocity -= Vector3.Project(rb.linearVelocity, gravityDir);

        // Apply jump
        rb.AddForce(gravityDir * jumpForce, ForceMode.VelocityChange);
    }
}
