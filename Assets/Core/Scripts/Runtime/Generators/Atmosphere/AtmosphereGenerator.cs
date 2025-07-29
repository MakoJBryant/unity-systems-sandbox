using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class AtmosphereGenerator
    {
        public static GameObject GenerateAtmosphere(
            Transform parent,
            int resolution,
            float maxElevation,
            float expansionFactor,
            ColorSettings colorSettings,
            Light sunLight,
            ref GameObject atmosphereGO,
            ref MeshFilter atmosphereMeshFilter,
            ref MeshRenderer atmosphereMeshRenderer,
            ref Mesh atmosphereMesh,
            ref AtmosphereController atmosphereController)
        {
            // Destroy any existing unreferenced atmosphere child
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child.name == "Atmosphere" && child.gameObject != atmosphereGO)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            if (atmosphereGO == null)
            {
                atmosphereGO = new GameObject("Atmosphere");
                atmosphereGO.transform.SetParent(parent, false);
                atmosphereMeshFilter = atmosphereGO.AddComponent<MeshFilter>();
                atmosphereMeshRenderer = atmosphereGO.AddComponent<MeshRenderer>();
                atmosphereController = atmosphereGO.AddComponent<AtmosphereController>();
                atmosphereMesh = new Mesh { name = "Generated Atmosphere Mesh" };
                atmosphereMeshFilter.sharedMesh = atmosphereMesh;
            }

            atmosphereMesh.Clear();
            float atmosphereRadius = maxElevation * expansionFactor;

            SphereCreator.CreateSphereMesh(resolution, atmosphereRadius, out Vector3[] v, out int[] t, out Vector2[] uv);

            atmosphereMesh.vertices = v;
            atmosphereMesh.triangles = t;
            atmosphereMesh.uv = uv;
            atmosphereMesh.RecalculateNormals();
            atmosphereMesh.RecalculateBounds();

            if (atmosphereMeshRenderer != null && colorSettings.atmosphereMaterial != null)
            {
                atmosphereMeshRenderer.sharedMaterial = colorSettings.atmosphereMaterial;

#if UNITY_2023_1_OR_NEWER
                if (sunLight == null)
                    sunLight = Object.FindFirstObjectByType<Light>();
#else
                if (sunLight == null)
                    sunLight = Object.FindObjectOfType<Light>();
#endif

                atmosphereController.sunLight = sunLight;
                atmosphereController.atmosphereMaterial = colorSettings.atmosphereMaterial;
                atmosphereController.atmosphereRadius = atmosphereRadius;
                atmosphereController.atmosphereColor = colorSettings.atmosphereColor;
                atmosphereController.density = colorSettings.atmosphereDensity;
                atmosphereController.power = colorSettings.atmospherePower;
                atmosphereController.ambientLightInfluence = colorSettings.atmosphereAmbientLightInfluence;
                atmosphereController.rimPower = colorSettings.atmosphereRimPower;
            }

            return atmosphereGO;
        }
    }
}
