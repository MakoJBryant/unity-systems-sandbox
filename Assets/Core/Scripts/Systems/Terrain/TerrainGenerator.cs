using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    /// <summary>
    /// Applies procedural terrain deformation to a base sphere using ShapeSettings.
    /// This operates in normalized (radius = 1) space.
    /// </summary>
    public static class TerrainGenerator
    {
        public static void ApplyTerrainDeformation(
            Vector3[] baseVertices,
            ShapeSettings shapeSettings,
            out Vector3[] displacedVertices,
            out float minElevation,
            out float maxElevation
        )
        {
            displacedVertices = new Vector3[baseVertices.Length];
            minElevation = float.MaxValue;
            maxElevation = float.MinValue;

            if (shapeSettings == null)
            {
                Debug.LogError("[TerrainGenerator] ShapeSettings is null.");
                return;
            }

            for (int i = 0; i < baseVertices.Length; i++)
            {
                Vector3 normal = baseVertices[i].normalized;

                // Base displacement starts with global offset
                float displacement = shapeSettings.globalHeightOffset;
                float firstLayerValue = 0f;

                if (shapeSettings.noiseLayers != null)
                {
                    for (int layerIndex = 0; layerIndex < shapeSettings.noiseLayers.Length; layerIndex++)
                    {
                        NoiseLayer layer = shapeSettings.noiseLayers[layerIndex];
                        if (layer == null || !layer.enabled)
                            continue;

                        float noise = 0f;
                        float frequency = layer.roughness;
                        float amplitude = 1f;
                        float totalAmplitude = 0f;

                        // Octaves
                        for (int o = 0; o < layer.octaves; o++)
                        {
                            Vector3 p = (normal + layer.offset) * frequency;
                            float v = PerlinNoise3D.GenerateNoise(p.x, p.y, p.z);

                            // Convert Perlin [0,1] -> [-1,1] or ridge
                            if (layer.noiseType == NoiseType.Ridge)
                                v = 1f - Mathf.Abs(v * 2f - 1f);
                            else
                                v = v * 2f - 1f;

                            noise += v * amplitude;
                            totalAmplitude += amplitude;

                            amplitude *= layer.persistence;
                            frequency *= layer.lacunarity;
                        }

                        float layerValue =
                            (totalAmplitude == 0f ? 0f : noise / totalAmplitude) + layer.minValue;

                        if (layerIndex == 0)
                            firstLayerValue = layerValue;

                        // Masking by first layer
                        if (layer.useFirstLayerAsMask && firstLayerValue <= 0f)
                            layerValue = 0f;

                        displacement += layerValue * layer.strength;
                    }
                }

                // Apply displacement in normalized space
                Vector3 displaced = normal * (baseVertices[i].magnitude + displacement);
                displacedVertices[i] = displaced;

                float height = displaced.magnitude;
                minElevation = Mathf.Min(minElevation, height);
                maxElevation = Mathf.Max(maxElevation, height);
            }
        }
    }
}
