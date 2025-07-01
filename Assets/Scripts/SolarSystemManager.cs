using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    public Material planetMaterial; // Assign in Inspector
    public ShapeSettings sunSettings;
    public ShapeSettings planetSettings;
    public ShapeSettings moonSettings;

    void Start()
    {
        // Create Sun
        GameObject sun = CreateBody("Sun", Vector3.zero, sunSettings, Color.yellow);

        // Create Planet
        GameObject planet = CreateBody("Planet", new Vector3(10f, 0, 0), planetSettings, Color.cyan);
        var planetOrbit = planet.AddComponent<Orbit>();
        planetOrbit.center = sun.transform;
        planetOrbit.orbitRadius = 10f;
        planetOrbit.orbitSpeed = 0.5f;

        // Create Moon
        GameObject moon = CreateBody("Moon", Vector3.zero, moonSettings, Color.gray);
        var moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.center = planet.transform;

        // Access ShapeSettings radius safely
        float planetRadius = planetSettings.radius;
        float moonRadius = moonSettings.radius;

        moonOrbit.orbitRadius = planetRadius + moonRadius + 1f;
        moonOrbit.orbitSpeed = 2f;
    }

    GameObject CreateBody(string name, Vector3 position, ShapeSettings shapeSettings, Color color)
    {
        GameObject body = new GameObject(name);
        body.transform.position = position;

        var cb = body.AddComponent<CelestialBody>();
        cb.shapeSettings = shapeSettings;
        cb.color = color;
        cb.templateMaterial = planetMaterial;

        // Required mesh components
        body.AddComponent<MeshFilter>();
        body.AddComponent<MeshRenderer>();

        return body;
    }
}
