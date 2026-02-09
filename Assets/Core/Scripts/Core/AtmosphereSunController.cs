using UnityEngine;

[ExecuteAlways]
public class AtmosphereSunController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Directional Light used as the sun")]
    public Light sunLight;

    [Tooltip("Renderer on the atmosphere sphere")]
    public Renderer atmosphereRenderer;

    [Header("Shader Properties")]
    public string sunDirectionProperty = "_SunDirection";
    public string planetCenterProperty = "_PlanetCenter";

    private MaterialPropertyBlock propertyBlock;

    void OnEnable()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (sunLight == null || atmosphereRenderer == null)
            return;

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        // --- Calculate sun direction correctly (from planet center to sun) ---
        // For directional lights, invert forward
        Vector3 sunDir = -sunLight.transform.forward.normalized;

        // --- Planet center ---
        Vector3 planetCenter = transform.parent != null ? transform.parent.position : Vector3.zero;

        // --- Apply to shader ---
        atmosphereRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVector(sunDirectionProperty, sunDir);
        propertyBlock.SetVector(planetCenterProperty, planetCenter);
        atmosphereRenderer.SetPropertyBlock(propertyBlock);
    }
}
