using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(fileName = "New Planet Preset", menuName = "Solar System/Planet Preset")]
    public class PlanetPreset : ScriptableObject
    {
        [Header("Core Planet Settings")]
        [Range(2, 256)]
        public int resolution = 64;
        public float radius = 1f;

        [Header("Orbital Settings")]
        [Tooltip("The distance from the central star (Sun) this planet will be positioned at.")]
        public float orbitalRadius = 0f;

        [Header("Assigned Settings Assets")]
        public ShapeSettings shapeSettings;

        [Header("Prefab References (Optional)")]
        public GameObject planetPrefab;
    }
}
