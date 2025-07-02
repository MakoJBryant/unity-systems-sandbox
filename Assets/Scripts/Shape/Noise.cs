using UnityEngine;

public class SimpleNoise
{
    private NoiseSettings.SimpleNoiseSettings settings;

    public SimpleNoise(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    // Evaluate noise at point - returns value between -1 and 1
    public float Evaluate(Vector3 point)
    {
        float noiseValue = Mathf.PerlinNoise(point.x, point.y);
        // PerlinNoise returns 0 to 1, so convert to -1 to 1
        return noiseValue * 2f - 1f;
    }
}

public class RidgedNoise
{
    private NoiseSettings.RidgedNoiseSettings settings;

    public RidgedNoise(NoiseSettings.RidgedNoiseSettings settings)
    {
        this.settings = settings;
    }

    // Evaluate ridged noise at point - returns value between -1 and 1
    public float Evaluate(Vector3 point)
    {
        // Simple implementation using PerlinNoise; you can make this more complex
        float noiseValue = Mathf.PerlinNoise(point.x, point.y);
        noiseValue = 1 - Mathf.Abs(noiseValue * 2f - 1f);
        return noiseValue * 2f - 1f;
    }
}
