using UnityEngine;

public class NoiseFilter
{
    private float strength;
    private float baseRoughness;
    private int numLayers;
    private float persistence;
    private Vector3 center;

    public NoiseFilter(float strength, float baseRoughness, int numLayers, float persistence, Vector3 center)
    {
        this.strength = strength;
        this.baseRoughness = baseRoughness;
        this.numLayers = numLayers;
        this.persistence = persistence;
        this.center = center;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0f;
        float frequency = baseRoughness;
        float amplitude = 1f;

        for (int i = 0; i < numLayers; i++)
        {
            // Use 3D Perlin noise approximation by sampling multiple 2D noises
            float v = Mathf.PerlinNoise(point.x * frequency + center.x, point.y * frequency + center.y);
            noiseValue += v * amplitude;

            frequency *= 2f;
            amplitude *= persistence;
        }

        noiseValue = noiseValue * strength;
        return noiseValue;
    }
}
