using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Atmosphere Settings", menuName = "Solar System/Atmosphere Settings")]
    public class AtmosphereSettings : ScriptableObject
    {
        [Header("Visual Settings")]
        public Material atmosphereMaterial;

        [Header("Atmosphere Properties")]
        [Tooltip("How thick the atmosphere is relative to planet radius")]
        public float thicknessMultiplier = 0.3f;

        [Tooltip("Optional tint for atmosphere shader")]
        public Color atmosphereColor = new Color(0.4f, 0.6f, 1f, 1f);
    }
}
