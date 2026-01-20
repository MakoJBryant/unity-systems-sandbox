using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetMover : MonoBehaviour
{
    [Header("References")]
    public Transform planet;

    [Header("Movement")]
    public float moveSpeed = 24f;
    public float acceleration = 20f;
    public float jumpForce = 24f;

    [Header("Gravity")]
    public float gravityStrength = 30f;
    public float gravityAlignSpeed = 10f;

    [Header("Look")]
    public float mouseSensitivity = 5f;

    [Header("Grounding")]
    public LayerMask groundMask;
    public float groundCheckDistance = 1.5f;

    Rigidbody rb;
    bool isGrounded;
    bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = false;
    }

    void Update()
    {
        HandleYawInput();

        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        AlignToGravity();
        CheckGround();
        HandleMovement();
        HandleJump();
    }

    void ApplyGravity()
    {
        Vector3 gravityDir = (planet.position - transform.position).normalized;
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    void AlignToGravity()
    {
        Vector3 gravityUp = (transform.position - planet.position).normalized;

        Quaternion targetRotation =
            Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;

        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            gravityAlignSpeed * Time.fixedDeltaTime
        ));
    }

    // FIXED: RAW mouse input
    void HandleYawInput()
    {
        float mouseX =
            Input.GetAxisRaw("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;

        Vector3 gravityUp = (transform.position - planet.position).normalized;

        rb.MoveRotation(
            Quaternion.AngleAxis(mouseX, gravityUp) * rb.rotation
        );
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 gravityUp = (transform.position - planet.position).normalized;

        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 desiredVelocity = transform.TransformDirection(input) * moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(velocity, gravityUp);
        Vector3 verticalVelocity = Vector3.Project(velocity, gravityUp);

        Vector3 velocityChange = desiredVelocity - surfaceVelocity;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        rb.linearVelocity = surfaceVelocity + verticalVelocity;
    }

    void CheckGround()
    {
        Vector3 gravityDir = (transform.position - planet.position).normalized;

        isGrounded = Physics.Raycast(
            transform.position,
            -gravityDir,
            groundCheckDistance,
            groundMask
        );
    }

    void HandleJump()
    {
        if (!jumpRequested) return;
        jumpRequested = false;
        if (!isGrounded) return;

        Vector3 gravityUp = (transform.position - planet.position).normalized;

        rb.linearVelocity -= Vector3.Project(rb.linearVelocity, gravityUp);
        rb.AddForce(gravityUp * jumpForce, ForceMode.VelocityChange);
    }
}
