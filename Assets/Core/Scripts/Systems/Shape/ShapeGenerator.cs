using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class ShapeGenerator : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution = 64;

        public float radius = 1000f;

        [Header("Settings")]
        public ShapeSettings shapeSettings;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private Mesh mesh;

        public float MinElevation { get; private set; }
        public float MaxElevation { get; private set; }

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            if (mesh == null)
            {
                mesh = new Mesh { name = "Planet Shape Mesh" };
                meshFilter.sharedMesh = mesh;
            }
        }

        public void GenerateShape()
        {
            if (shapeSettings == null || shapeSettings.noiseLayers == null || shapeSettings.noiseLayers.Length == 0)
                return;

            // 1. Create unit sphere
            SphereCreator.CreateSphereMesh(
                resolution,
                1f,
                out Vector3[] vertices,
                out int[] triangles,
                out Vector2[] uvs
            );

            // 2. Apply terrain deformation
            TerrainGenerator.ApplyTerrainDeformation(
                vertices,
                shapeSettings,
                out Vector3[] displaced,
                out float min,
                out float max
            );

            MinElevation = min;
            MaxElevation = max;

            // 3. Scale to radius
            for (int i = 0; i < displaced.Length; i++)
            {
                displaced[i] = displaced[i].normalized * displaced[i].magnitude * radius;
            }

            // 4. Build mesh
            mesh.Clear();
            mesh.vertices = displaced;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.sharedMesh = mesh;
        }
    }
}
