using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public int meshResolution = 10;
    public float radius = 1f;
    public NoiseLayer[] noiseLayers;
}
