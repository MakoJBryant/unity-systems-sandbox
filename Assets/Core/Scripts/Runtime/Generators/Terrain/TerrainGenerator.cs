using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class TerrainGenerator
    {
        public static void ApplyTerrainDeformation(Vector3[] baseVertices, TerrainSettings settings, out Vector3[] displacedVertices, out float minElevation, out float maxElevation)
        {
            displacedVertices = new Vector3[baseVertices.Length];
            minElevation = float.MaxValue;
            maxElevation = float.MinValue;

            for (int i = 0; i < baseVertices.Length; i++)
            {
                Vector3 normal = baseVertices[i].normalized;
                float displacement = settings.globalHeightOffset;
                float firstValue = 0f;

                for (int layerIndex = 0; layerIndex < settings.noiseLayers.Length; layerIndex++)
                {
                    var layer = settings.noiseLayers[layerIndex];
                    if (layer == null || !layer.enabled) continue;

                    float noise = 0f;
                    float frequency = layer.roughness;
                    float amplitude = 1f;
                    float totalAmplitude = 0f;

                    for (int o = 0; o < layer.octaves; o++)
                    {
                        Vector3 p = (normal + layer.offset) * frequency;
                        float v = PerlinNoise3D.GenerateNoise(p.x, p.y, p.z);

                        // Apply noise transformation
                        if (layer.noiseType == NoiseType.Ridge)
                        {
                            v = 1 - Mathf.Abs(v * 2 - 1);
                        }
                        else
                        {
                            v = v * 2 - 1;
                        }

                        noise += v * amplitude;
                        totalAmplitude += amplitude;

                        amplitude *= layer.persistence;
                        frequency *= layer.lacunarity;
                    }

                    float final = (totalAmplitude == 0f ? 0f : noise / totalAmplitude) + layer.minValue;

                    if (layerIndex == 0)
                        firstValue = final;

                    if (layer.useFirstLayerAsMask && firstValue <= 0f)
                        final = 0f;

                    displacement += final * layer.strength;
                }

                displacedVertices[i] = normal * (baseVertices[i].magnitude + displacement);

                float height = displacedVertices[i].magnitude;
                minElevation = Mathf.Min(minElevation, height);
                maxElevation = Mathf.Max(maxElevation, height);

                if (i < 10)
                {
                    Debug.Log($"[TerrainGenerator] Vertex {i}: Displacement = {displacement}, New Pos = {displacedVertices[i]}");
                }
            }
        }
    }
}
