using UnityEngine;

public class PlanetFaceCamera : MonoBehaviour
{
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;
        transform.LookAt(mainCam.transform.position);
    }
}
