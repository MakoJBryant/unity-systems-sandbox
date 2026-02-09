using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class OceanGenerator
    {
        public static GameObject GenerateOcean(
            Transform parent,
            int resolution,
            float minElevation,
            float maxElevation,
            float seaLevel,
            OceanSettings oceanSettings,
            ref GameObject oceanGO,
            ref MeshFilter oceanMeshFilter,
            ref MeshRenderer oceanMeshRenderer,
            ref Mesh oceanMesh,
            float manualOceanSize = 0f) // <-- NEW optional size
        {
            // Destroy duplicates
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child.name == "Ocean" && child.gameObject != oceanGO)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            if (oceanGO == null)
            {
                oceanGO = new GameObject("Ocean");
                oceanGO.name = "Ocean";
                oceanGO.transform.SetParent(parent, false);
                oceanMeshFilter = oceanGO.AddComponent<MeshFilter>();
                oceanMeshRenderer = oceanGO.AddComponent<MeshRenderer>();
                oceanMesh = new Mesh { name = "Generated Ocean Mesh" };
                oceanMeshFilter.sharedMesh = oceanMesh;
            }

            oceanMesh.Clear();

            float oceanRadius;

            if (manualOceanSize > 0f)
            {
                // Use manual size if specified
                oceanRadius = manualOceanSize;
            }
            else
            {
                // Old behavior: interpolate between min and max terrain
                oceanRadius = Mathf.Lerp(minElevation, maxElevation * 0.999f, seaLevel);
            }

            // Create the sphere mesh
            SphereCreator.CreateSphereMesh(resolution, oceanRadius, out Vector3[] vertices, out int[] triangles, out Vector2[] uv);

            oceanMesh.vertices = vertices;
            oceanMesh.triangles = triangles;
            oceanMesh.uv = uv;

            oceanMesh.RecalculateNormals();
            oceanMesh.RecalculateBounds();

            // Assign material
            if (oceanMeshRenderer != null && oceanSettings.oceanMaterial != null)
            {
                oceanMeshRenderer.sharedMaterial = oceanSettings.oceanMaterial;
                oceanMeshRenderer.sharedMaterial.SetFloat("_Radius", oceanRadius);
                oceanMeshRenderer.sharedMaterial.SetColor("_Color", oceanSettings.oceanColor);
                oceanMeshRenderer.sharedMaterial.SetVector("_PlanetCenter", parent.position);
            }

            // Reset transform
            oceanGO.transform.localPosition = Vector3.zero;
            oceanGO.transform.localRotation = Quaternion.identity;
            oceanGO.transform.localScale = Vector3.one;

            Debug.Log($"Ocean Radius: {oceanRadius}, Transform Scale: {oceanGO.transform.localScale}");

            return oceanGO;
        }
    }
}
