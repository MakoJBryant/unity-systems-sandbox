using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [ExecuteAlways]
    public class AtmosphereController : MonoBehaviour
    {
        [Tooltip("The material used for rendering the atmosphere.")]
        public Material atmosphereMaterial;

        [Tooltip("The main directional light in the scene, representing the Sun.")]
        public Light sunLight;

        [Tooltip("The radius of the atmosphere mesh. This should be slightly larger than the planet's radius.")]
        public float atmosphereRadius;

        [Tooltip("Atmosphere visual settings as a ScriptableObject.")]
        public AtmosphereSettings atmosphereSettings;

        private void Start()
        {
            if (atmosphereMaterial == null && GetComponent<MeshRenderer>() is MeshRenderer mr)
            {
                atmosphereMaterial = mr.sharedMaterial;
            }

            UpdateShaderProperties();
        }

        private void Update()
        {
            UpdateShaderProperties();
        }

        private void UpdateShaderProperties()
        {
            if (atmosphereSettings == null || atmosphereMaterial == null)
            {
                Debug.LogWarning("AtmosphereController: Missing AtmosphereSettings or Material.", this);
                return;
            }

            // Direction to sun
            Vector3 sunDirection = sunLight != null ? -sunLight.transform.forward : Vector3.forward;
            Vector3 planetCenter = transform.parent != null ? transform.parent.position : transform.position;

            atmosphereMaterial.SetVector("_SunDirection", sunDirection);
            atmosphereMaterial.SetVector("_PlanetCenter", planetCenter);
            atmosphereMaterial.SetFloat("_AtmosphereRadius", atmosphereRadius);
            atmosphereMaterial.SetColor("_AtmosphereColor", atmosphereSettings.atmosphereColor);
            atmosphereMaterial.SetFloat("_Density", atmosphereSettings.density);
            atmosphereMaterial.SetFloat("_Power", atmosphereSettings.power);
            atmosphereMaterial.SetFloat("_AmbientLightInfluence", atmosphereSettings.ambientLightInfluence);
            atmosphereMaterial.SetFloat("_RimPower", atmosphereSettings.rimPower);
        }
    }
}
