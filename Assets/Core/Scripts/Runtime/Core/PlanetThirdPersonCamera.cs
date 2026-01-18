using UnityEngine;

public class PlanetThirdPersonCamera : MonoBehaviour
{
    public Transform player;

    [Header("Orbit")]
    public float distance = 5f;
    public float height = 1.8f;

    [Header("Mouse")]
    public float mouseSensitivity = 3f;
    public float minPitch = -40f;
    public float maxPitch = 70f;

    float pitch;

    void LateUpdate()
    {
        if (!player) return;

        // Mouse pitch (Seb-style)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Gravity-relative up (already stable)
        Vector3 up = player.up;

        // Build rotation
        Quaternion pitchRot = Quaternion.AngleAxis(pitch, transform.right);
        Quaternion targetRot = Quaternion.LookRotation(player.forward, up) * pitchRot;

        // Position
        Vector3 targetPos =
            player.position
            + up * height
            - targetRot * Vector3.forward * distance;

        transform.position = targetPos;
        transform.rotation = Quaternion.LookRotation(player.position + up * height - transform.position, up);
    }
}
