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
        GameObject sun = CreateBody("Sun", Vector3.zero, sunSettings, Color.yellow);

        // Assign emissive sun material
        var sunRenderer = sun.GetComponent<MeshRenderer>();
        sunRenderer.material = sunMaterial;

        sun.AddComponent<PlanetFaceCamera>();

        var sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Point;
        sunLight.color = Color.yellow;
        sunLight.range = 20f;
        sunLight.intensity = 2f;
        sunLight.shadows = LightShadows.Soft;

        // Rotate the sun slowly
        var sunRotate = sun.AddComponent<Rotate>();
        sunRotate.rotationSpeed = 5f;

        // Create Planet
        GameObject planet = CreateBody("Planet", new Vector3(10f, 0, 0), planetSettings, Color.cyan);
        var planetOrbit = planet.AddComponent<Orbit>();
        planetOrbit.center = sun.transform;
        planetOrbit.orbitRadius = 10f;
        planetOrbit.orbitSpeed = 0.5f;

        // Rotate the planet moderately
        var planetRotate = planet.AddComponent<Rotate>();
        planetRotate.rotationSpeed = 30f;

        // Create Moon
        GameObject moon = CreateBody("Moon", Vector3.zero, moonSettings, Color.gray);
        var moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.center = planet.transform;

        // Rotate the moon faster
        var moonRotate = moon.AddComponent<Rotate>();
        moonRotate.rotationSpeed = 60f;

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

        // Add mesh components
        body.AddComponent<MeshFilter>();
        body.AddComponent<MeshRenderer>();

        // Add CelestialBody
        var cb = body.AddComponent<CelestialBody>();
        cb.shapeSettings = shapeSettings;
        cb.color = color;
        cb.templateMaterial = planetMaterial;

        /*
#if UNITY_EDITOR
        // Add DrawNormals only in editor for debugging
        body.AddComponent<DrawNormals>();
#endif
        */

        return body;
    }

}
