using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CelestialBody : MonoBehaviour
{
    public float radius = 1f;
    public int resolution = 10; // Number of vertices per face edge
    public Material templateMaterial;
    public Color color = Color.white;

    private ShapeGenerator shapeGenerator;

    void Start()
    {
        var noiseFilter = new NoiseFilter(strength: 0.5f, baseRoughness: 1f, numLayers: 4, persistence: 0.5f, center: Vector3.zero);
        shapeGenerator = new ShapeGenerator(noiseFilter);

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = GenerateMesh();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = new Material(templateMaterial);
        mr.material.color = color;
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Create a cube and then normalize vertices to form a sphere (cube-sphere technique)

        // 6 faces of cube: up, down, left, right, forward, back
        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        int faceResolution = resolution;

        for (int i = 0; i < 6; i++)
        {
            ConstructFace(directions[i], vertices, triangles, faceResolution);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        return mesh;
    }

    void ConstructFace(Vector3 localUp, List<Vector3> vertices, List<int> triangles, int resolution)
    {
        Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        Vector3 axisB = Vector3.Cross(localUp, axisA);

        int startIndex = vertices.Count;

        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / resolution;
                Vector3 pointOnUnitCube = localUp
                    + (percent.x - 0.5f) * 2f * axisA
                    + (percent.y - 0.5f) * 2f * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                Vector3 pointOnPlanet = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere, radius);

                vertices.Add(pointOnPlanet);

                // Create triangles
                if (x != resolution && y != resolution)
                {
                    int current = startIndex + y * (resolution + 1) + x;
                    int next = current + resolution + 1;

                    triangles.Add(current);
                    triangles.Add(next + 1);
                    triangles.Add(next);

                    triangles.Add(current);
                    triangles.Add(current + 1);
                    triangles.Add(next + 1);
                }
            }
        }
    }
}
