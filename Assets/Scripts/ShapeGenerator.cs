using UnityEngine;
public class ShapeGenerator
{
    private INoiseFilter[] noiseFilters;
    private NoiseSettings[] noiseLayers;

    private ShapeSettings settings;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = 0;

        foreach (var filter in noiseFilters)
        {
            elevation += filter.Evaluate(pointOnUnitSphere);
        }

        return pointOnUnitSphere * settings.radius * (1 + elevation);
    }
}
