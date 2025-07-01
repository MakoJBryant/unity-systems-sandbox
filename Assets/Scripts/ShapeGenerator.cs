using UnityEngine;

public class ShapeGenerator
{
    private NoiseFilter noiseFilter;

    public ShapeGenerator(NoiseFilter noiseFilter)
    {
        this.noiseFilter = noiseFilter;
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere, float radius)
    {
        float elevation = noiseFilter.Evaluate(pointOnUnitSphere);
        return pointOnUnitSphere * (radius + elevation);
    }
}
