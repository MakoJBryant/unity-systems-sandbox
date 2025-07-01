using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i]);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = 0f;

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere);
            }
        }

        return pointOnUnitSphere * (settings.radius + elevation);
    }
}
