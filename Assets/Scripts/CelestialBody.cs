using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CelestialBody : MonoBehaviour
{
    public ShapeSettings shapeSettings;
    public Material templateMaterial;
    public Color color = Color.white;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        InitializeComponents();
        GenerateMesh();
    }

    private void InitializeComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void GenerateMesh()
    {
        if (shapeSettings == null)
        {
            Debug.LogWarning($"{name} is missing ShapeSettings.");
            return;
        }

        var shapeGenerator = new ShapeGenerator(shapeSettings);
        var resolution = shapeSettings.meshResolution;

        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        Vector3[] directions = {
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right,
            Vector3.forward, Vector3.back
        };

        foreach (var dir in directions)
        {
            ConstructFace(dir, vertices, triangles, resolution, shapeGenerator);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        // Set material
        if (templateMaterial != null)
        {
            Material matInstance = new Material(templateMaterial);
            matInstance.color = color;
            meshRenderer.sharedMaterial = matInstance;
        }
    }

    private void ConstructFace(Vector3 localUp, List<Vector3> vertices, List<int> triangles, int resolution, ShapeGenerator shapeGenerator)
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
                Vector3 pointOnPlanet = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                vertices.Add(pointOnPlanet);

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
