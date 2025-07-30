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
            AtmosphereSettings atmosphereSettings,
            Light sunLight,
            ref GameObject atmosphereGO,
            ref MeshFilter atmosphereMeshFilter,
            ref MeshRenderer atmosphereMeshRenderer,
            ref Mesh atmosphereMesh,
            ref AtmosphereController atmosphereController)
        {
            // Remove stray copies
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child.name == "Atmosphere" && child.gameObject != atmosphereGO)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            // Create atmosphere object if null
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

            // Apply settings
            if (atmosphereMeshRenderer != null && atmosphereSettings.atmosphereMaterial != null)
            {
                atmosphereMeshRenderer.sharedMaterial = atmosphereSettings.atmosphereMaterial;

#if UNITY_2023_1_OR_NEWER
                if (sunLight == null)
                    sunLight = Object.FindFirstObjectByType<Light>();
#else
                if (sunLight == null)
                    sunLight = Object.FindObjectOfType<Light>();
#endif

                atmosphereController.sunLight = sunLight;
                atmosphereController.atmosphereMaterial = atmosphereSettings.atmosphereMaterial;
                atmosphereController.atmosphereRadius = atmosphereRadius;
                atmosphereController.atmosphereSettings = atmosphereSettings;
            }

            return atmosphereGO;
        }
    }
}
