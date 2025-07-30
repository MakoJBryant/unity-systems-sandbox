using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Ocean Settings", menuName = "Solar System/Ocean Settings")]
    public class OceanSettings : ScriptableObject
    {
        [Header("Visual Settings")]
        public Material oceanMaterial;
        public Color oceanColor = new Color(0f, 0.2f, 0.6f, 1f);

        [Header("Surface Settings")]
        [Range(0f, 1f)]
        public float seaLevel = 0.5f;
    }
}
