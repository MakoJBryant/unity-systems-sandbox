using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class PerlinNoise3D
    {
        /// <summary>
        /// Generates a pseudo 3D Perlin noise value by averaging multiple 2D Perlin noise samples.
        /// Note: This is not a true 3D Perlin noise, but a good approximation for spherical terrain displacement.
        /// </summary>
        public static float GenerateNoise(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float xz = Mathf.PerlinNoise(x, z);

            float yx = Mathf.PerlinNoise(y, x);
            float zy = Mathf.PerlinNoise(z, y);
            float zx = Mathf.PerlinNoise(z, x);

            // Average all samples to get a smooth value between 0 and 1
            return (xy + yz + xz + yx + zy + zx) / 6f;
        }
    }
}
