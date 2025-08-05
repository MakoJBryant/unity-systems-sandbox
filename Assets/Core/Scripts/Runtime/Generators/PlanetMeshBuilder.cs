using MakoJBryant.SolarSystem.Generation;
using UnityEngine;

public static class PlanetMeshBuilder
{
    public static Mesh Build(int resolution, float radius, TerrainSettings terrainSettings, out float minElevation, out float maxElevation)
    {
        SphereCreator.CreateSphereMesh(resolution, radius, out Vector3[] vertices, out int[] triangles, out Vector2[] uvs);

        TerrainGenerator.ApplyTerrainDeformation(vertices, terrainSettings, out Vector3[] displacedVertices, out minElevation, out maxElevation);
        vertices = displacedVertices;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
            Vector3 faceNormal = Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]).normalized;
            normals[a] += faceNormal;
            normals[b] += faceNormal;
            normals[c] += faceNormal;
        }
        for (int i = 0; i < normals.Length; i++) normals[i].Normalize();

        var mesh = new Mesh { name = "Generated Planet Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        return mesh;
    }
}
