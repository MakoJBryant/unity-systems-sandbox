using UnityEngine;

public class NoiseFilter
{
    private NoiseLayer settings;

    public NoiseFilter(NoiseLayer settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0f;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = Mathf.PerlinNoise(
                point.x * frequency + settings.center.x,
                point.y * frequency + settings.center.y
            );

            noiseValue += v * amplitude;
            frequency *= 2f;
            amplitude *= settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }
}
