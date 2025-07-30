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
            ref Mesh oceanMesh)
        {
            // Destroy stray duplicates
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
                oceanGO.transform.SetParent(parent, false);
                oceanMeshFilter = oceanGO.AddComponent<MeshFilter>();
                oceanMeshRenderer = oceanGO.AddComponent<MeshRenderer>();
                oceanMesh = new Mesh { name = "Generated Ocean Mesh" };
                oceanMeshFilter.sharedMesh = oceanMesh;
            }

            oceanMesh.Clear();
            float oceanRadius = Mathf.Lerp(minElevation, maxElevation * 0.999f, seaLevel);

            SphereCreator.CreateSphereMesh(resolution, oceanRadius, out Vector3[] v, out int[] t, out Vector2[] uv);

            oceanMesh.vertices = v;
            oceanMesh.triangles = t;
            oceanMesh.uv = uv;
            oceanMesh.RecalculateNormals();
            oceanMesh.RecalculateBounds();

            if (oceanMeshRenderer != null && oceanSettings.oceanMaterial != null)
            {
                oceanMeshRenderer.sharedMaterial = oceanSettings.oceanMaterial;
                oceanMeshRenderer.sharedMaterial.SetFloat("_Radius", oceanRadius);
                oceanMeshRenderer.sharedMaterial.SetColor("_Color", oceanSettings.oceanColor);
                oceanMeshRenderer.sharedMaterial.SetVector("_PlanetCenter", parent.position);
            }

            Debug.Log($"Ocean Transform - Position: {oceanGO.transform.position}, Rotation: {oceanGO.transform.rotation.eulerAngles}, Scale: {oceanGO.transform.localScale}");

            return oceanGO;
        }
    }
}
