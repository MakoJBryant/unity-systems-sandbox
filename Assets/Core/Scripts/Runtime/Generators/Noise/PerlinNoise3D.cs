using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation // Namespace for noise-related classes
{
    public static class PerlinNoise3D
    {
        public static float GenerateNoise(float x, float y, float z)
        {
            // Combine multiple 2D Perlin noise samples to get a rough 3D effect.
            // This is not a true 3D Perlin noise but works for basic spherical displacement.
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float xz = Mathf.PerlinNoise(x, z);

            float yx = Mathf.PerlinNoise(y, x);
            float zy = Mathf.PerlinNoise(z, y);
            float zx = Mathf.PerlinNoise(z, x);

            return (xy + yz + xz + yx + zy + zx) / 6f; // Average them to get a value between 0 and 1
        }
    }
}