using MakoJBryant.SolarSystem.Generation;
using UnityEngine;

public static class BiomeGenerator
{
    public static Texture2D GenerateBiomeTexture(BiomeSettings biomeSettings, int resolution = 256)
    {
        if (biomeSettings == null || biomeSettings.biomes == null || biomeSettings.biomes.Length == 0)
            return null;

        var biomes = biomeSettings.biomes;
        Texture2D texture = new Texture2D(resolution, 1, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[resolution];
        System.Array.Sort(biomes, (a, b) => a.startHeight.CompareTo(b.startHeight));

        for (int i = 0; i < resolution; i++)
        {
            float h = i / (float)(resolution - 1);
            Color col = biomes[0].color;

            foreach (var biome in biomes)
            {
                float blend = Mathf.Clamp01((h - biome.startHeight) / biome.blendAmount);
                col = Color.Lerp(col, biome.color, blend);
            }

            pixels[i] = col;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
