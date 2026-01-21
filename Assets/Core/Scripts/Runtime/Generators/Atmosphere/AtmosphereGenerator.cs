using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class AtmosphereGenerator
    {
        public static GameObject GenerateAtmosphere(
            Transform planetTransform,
            int resolution,
            float planetRadius,
            float maxElevation,
            AtmosphereSettings settings,
            Light sunLight,
            ref GameObject atmosphereGO,
            ref MeshFilter meshFilter,
            ref MeshRenderer meshRenderer,
            ref Mesh mesh,
            float manualAtmosphereSize = 0f // optional override
        )
        {
            // Destroy duplicates
            for (int i = planetTransform.childCount - 1; i >= 0; i--)
            {
                Transform child = planetTransform.GetChild(i);
                if (child.name == "Atmosphere" && child.gameObject != atmosphereGO)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            // Create new atmosphere object if needed
            if (atmosphereGO == null)
            {
                Transform existing = planetTransform.Find("Atmosphere");
                if (existing != null)
                {
                    atmosphereGO = existing.gameObject;
                }
                else
                {
                    atmosphereGO = new GameObject("Atmosphere");
                    atmosphereGO.transform.SetParent(planetTransform, false);
                }

                // Ensure MeshFilter exists
                meshFilter = atmosphereGO.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = atmosphereGO.AddComponent<MeshFilter>();

                // Ensure MeshRenderer exists
                meshRenderer = atmosphereGO.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    meshRenderer = atmosphereGO.AddComponent<MeshRenderer>();
            }

            // Calculate atmosphere radius
            float atmosphereRadius;
            if (manualAtmosphereSize > 0f)
            {
                atmosphereRadius = manualAtmosphereSize;
            }
            else
            {
                atmosphereRadius = planetRadius * (1f + settings.thicknessMultiplier) +
                                   Mathf.Max(0f, maxElevation - planetRadius) * 1.05f;
            }

            // Create or clear mesh
            if (mesh == null)
            {
                mesh = new Mesh { name = "Generated Atmosphere Mesh" };
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
                meshFilter.sharedMesh = mesh;
            }

            // Generate sphere mesh
            SphereCreator.CreateSphereMesh(resolution, atmosphereRadius, out Vector3[] vertices, out int[] triangles, out Vector2[] uv);
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Assign material safely
            if (settings.atmosphereMaterial != null)
            {
                meshRenderer.sharedMaterial = settings.atmosphereMaterial;
                meshRenderer.sharedMaterial.SetColor("_Color", settings.atmosphereColor);
            }

            // Disable shadows
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            // Reset transform
            atmosphereGO.transform.localPosition = Vector3.zero;
            atmosphereGO.transform.localRotation = Quaternion.identity;
            atmosphereGO.transform.localScale = Vector3.one;

            // Add or update sun controller
            AtmosphereSunController controller = atmosphereGO.GetComponent<AtmosphereSunController>();
            if (controller == null)
                controller = atmosphereGO.AddComponent<AtmosphereSunController>();

            controller.sunLight = sunLight;
            controller.atmosphereRenderer = meshRenderer;

            Debug.Log($"Atmosphere Radius: {atmosphereRadius}, Transform Scale: {atmosphereGO.transform.localScale}");

            return atmosphereGO;
        }
    }
}
