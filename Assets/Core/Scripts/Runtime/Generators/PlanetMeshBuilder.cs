using MakoJBryant.SolarSystem.Generation;
using UnityEngine;

public static class PlanetMeshBuilder
{
    public static Mesh Build(
        int resolution,
        float radius,
        TerrainSettings terrainSettings,
        out float minElevation,
        out float maxElevation)
    {
        // =========================================
        // 1. CREATE UNIT SPHERE (radius = 1)
        // =========================================
        SphereCreator.CreateSphereMesh(
            resolution,
            1f,
            out Vector3[] vertices,
            out int[] triangles,
            out Vector2[] uvs
        );

        // =========================================
        // 2. APPLY TERRAIN IN NORMALIZED SPACE
        // =========================================
        TerrainGenerator.ApplyTerrainDeformation(
            vertices,
            terrainSettings,
            out Vector3[] displacedVertices,
            out minElevation,
            out maxElevation
        );

        // =========================================
        // 3. SCALE RESULT TO FINAL RADIUS
        // =========================================
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            float elevation = displacedVertices[i].magnitude;
            displacedVertices[i] = displacedVertices[i].normalized * (elevation * radius);
        }

        vertices = displacedVertices;

        // =========================================
        // 4. RECALCULATE NORMALS (CUSTOM)
        // =========================================
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            Vector3 faceNormal =
                Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]).normalized;

            normals[a] += faceNormal;
            normals[b] += faceNormal;
            normals[c] += faceNormal;
        }

        for (int i = 0; i < normals.Length; i++)
            normals[i].Normalize();

        // =========================================
        // 5. BUILD MESH
        // =========================================
        Mesh mesh = new Mesh { name = "Generated Planet Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        return mesh;
    }
}
