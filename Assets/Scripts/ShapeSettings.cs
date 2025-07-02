using UnityEngine;

[CreateAssetMenu(menuName = "Planet/Shape Settings")]
public class ShapeSettings : ScriptableObject
{
    public float radius = 1f;
    public int meshResolution = 10;
    public NoiseLayer[] noiseLayers;
}

[System.Serializable]
public class NoiseLayer
{
    public bool enabled = true;
    public float strength = 1f;
    public float baseRoughness = 1f;
    public int numLayers = 1;
    public float persistence = 0.5f;
    public float minValue = 0f;
    public Vector3 center = Vector3.zero;
    public bool useFirstLayerAsMask = false;
}

