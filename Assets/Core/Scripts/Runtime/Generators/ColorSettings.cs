using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Color Settings", menuName = "Solar System/Color Settings")]
    public class ColorSettings : ScriptableObject
    {
        public Material planetMaterial;
        public Material oceanMaterial;
        public Color oceanColor;

        public BiomeSettings biomeSettings;

        [Header("Atmosphere Settings")]
        public Material atmosphereMaterial;
        public Color atmosphereColor = new Color(0.2f, 0.4f, 1.0f, 1.0f);
        [Range(0.1f, 10.0f)]
        public float atmosphereDensity = 1.0f;
        [Range(1.0f, 50.0f)]
        public float atmospherePower = 5.0f;
        [Range(0.0f, 1.0f)]
        public float atmosphereAmbientLightInfluence = 0.1f;
        [Range(1.0f, 20.0f)]
        public float atmosphereRimPower = 3.0f;
    }
}
