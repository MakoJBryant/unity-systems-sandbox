using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Terrain Settings", menuName = "Solar System/Terrain Settings")]
    public class TerrainSettings : ScriptableObject
    {
        [Tooltip("Adjusts the overall height of the terrain relative to the base radius. Negative values will create oceans.")]
        public float globalHeightOffset = 0f;

        [Tooltip("Noise layers used for procedural terrain deformation.")]
        public NoiseLayer[] noiseLayers;
    }
}
