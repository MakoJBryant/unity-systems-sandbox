using UnityEngine;

public class SunLightBinder : MonoBehaviour
{
    public Light sunLight;
    public float distance = 10000f;

    void LateUpdate()
    {
        if (!sunLight || !Camera.main) return;

        transform.position =
            Camera.main.transform.position
            - sunLight.transform.forward * distance;

        transform.forward = Camera.main.transform.forward;
    }
}
