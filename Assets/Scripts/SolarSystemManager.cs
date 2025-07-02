using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    public Material planetMaterial;    // Assign normal planet material in Inspector
    public Material sunMaterial;       // Assign emissive sun material in Inspector

    public ShapeSettings sunSettings;
    public ShapeSettings planetSettings;
    public ShapeSettings moonSettings;

    void Start()
    {
        // Create Sun
        GameObject sun = CreateBody("Sun", Vector3.zero, sunSettings, sunMaterial, Color.yellow);
        sun.AddComponent<PlanetFaceCamera>();

        var sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Point;
        sunLight.color = Color.yellow;
        sunLight.range = 20f;
        sunLight.intensity = 2f;
        sunLight.shadows = LightShadows.Soft;

        sun.AddComponent<Rotate>().rotationSpeed = 5f;

        // Create Planet
        GameObject planet = CreateBody("Planet", new Vector3(10f, 0, 0), planetSettings, planetMaterial, Color.cyan);
        var planetOrbit = planet.AddComponent<Orbit>();
        planetOrbit.center = sun.transform;
        planetOrbit.orbitRadius = 10f;
        planetOrbit.orbitSpeed = 0.5f;

        planet.AddComponent<Rotate>().rotationSpeed = 30f;

        // Create Moon
        GameObject moon = CreateBody("Moon", Vector3.zero, moonSettings, planetMaterial, Color.gray);
        var moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.center = planet.transform;

        moon.AddComponent<Rotate>().rotationSpeed = 60f;

        float planetRadius = planetSettings.radius;
        float moonRadius = moonSettings.radius;

        moonOrbit.orbitRadius = planetRadius + moonRadius + 1f;
        moonOrbit.orbitSpeed = 2f;
    }

    GameObject CreateBody(string name, Vector3 position, ShapeSettings settings, Material material, Color color)
    {
        GameObject body = new GameObject(name);
        body.transform.position = position;

        var cb = body.AddComponent<CelestialBody>();
        cb.shapeSettings = settings;
        cb.templateMaterial = material;
        cb.color = color;

        return body;
    }
}
