using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    public Material planetMaterial; // assign this in Inspector

    void Start()
    {
        GameObject sun = CreateBody("Sun", Vector3.zero, 4f, Color.yellow);
        GameObject planet = CreateBody("Planet", new Vector3(10f, 0, 0), 1f, Color.cyan);
        var orbit = planet.AddComponent<Orbit>();
        orbit.center = sun.transform;
        orbit.orbitRadius = 10f;
        orbit.orbitSpeed = 0.5f;

        GameObject moon = CreateBody("Moon", planet.transform.position + new Vector3(2f, 0, 0), 0.5f, Color.gray);
        var moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.center = planet.transform;
        moonOrbit.orbitRadius = 2f;
        moonOrbit.orbitSpeed = 2f;
    }

    GameObject CreateBody(string name, Vector3 position, float radius, Color color)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = name;
        body.transform.position = position;

        var cb = body.AddComponent<CelestialBody>();
        cb.radius = radius;
        cb.color = color;
        cb.templateMaterial = planetMaterial; // assign it here

        return body;
    }
}
