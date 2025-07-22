using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    // Enum for different noise types (Standard Perlin, Ridge)
    public enum NoiseType { Standard, Ridge }

    // Now a ScriptableObject instead of a struct
    [CreateAssetMenu(fileName = "NoiseLayer", menuName = "Planet Generation/Noise Layer")]
    public class NoiseLayer : ScriptableObject
    {
        public bool enabled = true;        // Whether this noise layer is active
        public float strength = 1f;        // How much this layer displaces the surface
        public float roughness = 2f;       // Frequency of the noise (higher = more jagged)
        public int octaves = 4;            // Number of noise layers combined (more = more detail)
        [Range(0, 1)]
        public float persistence = 0.5f;   // Amplitude decay per octave
        public float lacunarity = 2f;      // Frequency increase per octave
        public Vector3 offset = Vector3.zero; // Offset for randomness
        public float minValue = 0f;        // Minimum possible value
        public NoiseType noiseType = NoiseType.Standard; // Type of noise
        public bool useFirstLayerAsMask = false; // Use first layer as a mask
    }
}
