using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Biome Settings", menuName = "Solar System/Biome Settings")]
    public class BiomeSettings : ScriptableObject
    {
        public BiomeLayer[] biomes;

        [System.Serializable]
        public struct BiomeLayer
        {
            public string name;
            public Color color;
            [Range(0, 1)]
            public float startHeight;
            [Range(0.01f, 1)]
            public float blendAmount;
        }
    }
}
