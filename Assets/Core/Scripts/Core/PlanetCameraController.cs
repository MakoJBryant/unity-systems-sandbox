using UnityEngine;

public class PlanetCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform planet;
    public Transform cameraPivot;
    public Camera cam;

    [Header("Settings")]
    public float mouseSensitivity = 5f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float cameraDistance = 4f;
    public float cameraCollisionRadius = 0.3f;

    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        Vector3 gravityUp = (player.position - planet.position).normalized;

        // FIXED: RAW mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotate player (yaw)
        player.rotation =
            Quaternion.AngleAxis(mouseX, gravityUp) * player.rotation;

        // Pitch
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Camera positioning + collision
        Vector3 desiredCamPos =
            cameraPivot.position - cameraPivot.forward * cameraDistance;

        if (Physics.SphereCast(
            cameraPivot.position,
            cameraCollisionRadius,
            -cameraPivot.forward,
            out RaycastHit hit,
            cameraDistance,
            ~0,
            QueryTriggerInteraction.Ignore))
        {
            desiredCamPos = hit.point + hit.normal * cameraCollisionRadius;
        }

        cam.transform.position = desiredCamPos;
        cam.transform.rotation =
            Quaternion.LookRotation(cameraPivot.position - cam.transform.position, gravityUp);
    }
}
