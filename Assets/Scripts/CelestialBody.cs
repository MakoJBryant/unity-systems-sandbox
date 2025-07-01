using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CelestialBody : MonoBehaviour
{
    public float radius = 1f;
    public Color color = Color.white;
    public Material templateMaterial; // assigned after creation

    void Start()
    {
        transform.localScale = Vector3.one * radius * 2f;

        var renderer = GetComponent<MeshRenderer>();

        if (templateMaterial != null)
        {
            Material matInstance = new Material(templateMaterial); // Make a clone
            matInstance.color = color; // Set unique color
            renderer.material = matInstance; // Assign instance
        }

        else
        {
            Debug.LogWarning($"No material assigned to {name}. Assign a material in SolarSystemManager.");
        }
    }
}
