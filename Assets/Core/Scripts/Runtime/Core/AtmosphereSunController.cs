using UnityEngine;

[ExecuteAlways]
public class AtmosphereSunController : MonoBehaviour
{
    [Header("References")]
    public Light sunLight;                 // Assign your directional sun light
    public Renderer atmosphereRenderer;    // Assign the Atmosphere MeshRenderer
    public string sunDirectionProperty = "_SunDirection"; // Shader property name

    void Update()
    {
        if (sunLight == null || atmosphereRenderer == null || atmosphereRenderer.sharedMaterial == null)
            return;

        // Pass sun direction to shader (world space)
        Vector3 sunDir = sunLight.transform.forward;
        atmosphereRenderer.sharedMaterial.SetVector(sunDirectionProperty, sunDir);
    }
}
