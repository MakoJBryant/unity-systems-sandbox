using UnityEngine;

public class NoiseFilter : INoiseFilter
{
    private NoiseLayer noiseLayer;
    private SimpleNoise simpleNoise;
    private RidgedNoise ridgedNoise;

    public NoiseFilter(NoiseLayer noiseLayer)
    {
        this.noiseLayer = noiseLayer;

        if (noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Simple)
        {
            simpleNoise = new SimpleNoise(noiseLayer.noiseSettings.simpleNoiseSettings);
        }
        else if (noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Ridged)
        {
            ridgedNoise = new RidgedNoise(noiseLayer.noiseSettings.ridgedNoiseSettings);
        }
    }

    public float Evaluate(Vector3 point)
    {
        if (!noiseLayer.enabled)
            return 0f;

        if (noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Simple)
        {
            return EvaluateSimpleNoise(point);
        }
        else if (noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Ridged)
        {
            return EvaluateRidgedNoise(point);
        }
        return 0f;
    }

    private float EvaluateSimpleNoise(Vector3 point)
    {
        var s = noiseLayer.noiseSettings.simpleNoiseSettings;

        float noiseValue = 0f;
        float frequency = s.baseRoughness;
        float amplitude = 1f;

        for (int i = 0; i < s.numLayers; i++)
        {
            Vector3 samplePoint = point * frequency + s.center;
            float v = simpleNoise.Evaluate(samplePoint);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= s.roughness;
            amplitude *= s.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - s.minValue);
        return noiseValue * s.strength;
    }

    private float EvaluateRidgedNoise(Vector3 point)
    {
        var r = noiseLayer.noiseSettings.ridgedNoiseSettings;

        float noiseValue = 0f;
        float frequency = r.baseRoughness;
        float amplitude = 1f;
        float weight = 1f;

        for (int i = 0; i < r.numLayers; i++)
        {
            Vector3 samplePoint = point * frequency + r.center;
            float v = 1 - Mathf.Abs(simpleNoise.Evaluate(samplePoint));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * r.weightMultiplier);

            noiseValue += v * amplitude;
            frequency *= r.roughness;
            amplitude *= r.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - r.minValue);
        return noiseValue * r.strength;
    }
}
