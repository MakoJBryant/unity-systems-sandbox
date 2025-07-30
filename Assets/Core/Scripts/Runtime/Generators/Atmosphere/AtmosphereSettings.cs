using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Atmosphere Settings", menuName = "Solar System/Atmosphere Settings")]
    public class AtmosphereSettings : ScriptableObject
    {
        public Material atmosphereMaterial;

        [Header("Visual Settings")]
        public Color atmosphereColor = new Color(0.2f, 0.4f, 1.0f, 1.0f);
        [Range(0.1f, 10.0f)] public float density = 1.0f;
        [Range(1.0f, 50.0f)] public float power = 5.0f;
        [Range(0.0f, 1.0f)] public float ambientLightInfluence = 0.1f;
        [Range(1.0f, 20.0f)] public float rimPower = 3.0f;
    }
}
