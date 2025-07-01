using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    public Material planetMaterial; // assign this in Inspector

    void Start()
    {
        // Create Sun
        GameObject sun = CreateBody("Sun", Vector3.zero, 4f, Color.yellow);

        // Create Planet
        GameObject planet = CreateBody("Planet", new Vector3(10f, 0, 0), 1f, Color.cyan);
        var planetOrbit = planet.AddComponent<Orbit>();
        planetOrbit.center = sun.transform;
        planetOrbit.orbitRadius = 10f;
        planetOrbit.orbitSpeed = 0.5f;

        // Create Moon without manual position
        GameObject moon = CreateBody("Moon", Vector3.zero, 0.5f, Color.gray);
        var moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.center = planet.transform;

        // Access CelestialBody components for radius
        var planetBody = planet.GetComponent<CelestialBody>();
        var moonBody = moon.GetComponent<CelestialBody>();

        // Set orbit radius with safe margin to avoid clipping
        moonOrbit.orbitRadius = planetBody.radius + moonBody.radius + 1f;
        moonOrbit.orbitSpeed = 2f;
    }

    GameObject CreateBody(string name, Vector3 position, float radius, Color color)
    {
        GameObject body = new GameObject(name);
        body.transform.position = position;

        var cb = body.AddComponent<CelestialBody>();
        cb.radius = radius;
        cb.color = color;
        cb.templateMaterial = planetMaterial; // assign your material

        return body;
    }
}
