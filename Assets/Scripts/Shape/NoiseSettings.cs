using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Ridged }

    public FilterType filterType = FilterType.Simple;

    public SimpleNoiseSettings simpleNoiseSettings = new SimpleNoiseSettings();
    public RidgedNoiseSettings ridgedNoiseSettings = new RidgedNoiseSettings();

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1f;
        public float baseRoughness = 1f;
        public float roughness = 2f;
        public int numLayers = 1;
        public float persistence = 0.5f;
        public Vector3 center = Vector3.zero;
        public float minValue = 0f;
    }

    [System.Serializable]
    public class RidgedNoiseSettings
    {
        public float strength = 1f;
        public float baseRoughness = 1f;
        public float roughness = 2f;
        public int numLayers = 1;
        public float persistence = 0.5f;
        public Vector3 center = Vector3.zero;
        public float minValue = 0f;
        public float weightMultiplier = 0.8f;
    }
}

[System.Serializable]
public class NoiseLayer
{
    public bool enabled = true;
    public bool useFirstLayerAsMask = false;
    public NoiseSettings noiseSettings = new NoiseSettings();
}
