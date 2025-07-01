using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public ShapeSettings shapeSettings;
    public Material templateMaterial;
    public Color color = Color.white;

    private ShapeGenerator shapeGenerator;

    void Start()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = GenerateMesh();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = new Material(templateMaterial);
        mr.material.color = color;

    }

    Mesh GenerateMesh()
    {
        int resolution = shapeSettings.meshResolution;
        Mesh mesh = new Mesh();
        List<UnityEngine.Vector3> vertices = new List<UnityEngine.Vector3>();
        List<int> triangles = new List<int>();

        UnityEngine.Vector3[] directions = {
            UnityEngine.Vector3.up,
            UnityEngine.Vector3.down,
            UnityEngine.Vector3.left,
            UnityEngine.Vector3.right,
            UnityEngine.Vector3.forward,
            UnityEngine.Vector3.back
        };

        for (int i = 0; i < 6; i++)
        {
            ConstructFace(directions[i], vertices, triangles, resolution);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        return mesh;
    }

    void ConstructFace(UnityEngine.Vector3 localUp, List<UnityEngine.Vector3> vertices, List<int> triangles, int resolution)
    {
        UnityEngine.Vector3 axisA = new UnityEngine.Vector3(localUp.y, localUp.z, localUp.x);
        UnityEngine.Vector3 axisB = UnityEngine.Vector3.Cross(localUp, axisA);
        int startIndex = vertices.Count;

        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                UnityEngine.Vector2 percent = new UnityEngine.Vector2(x, y) / resolution;
                UnityEngine.Vector3 pointOnUnitCube = localUp
                    + (percent.x - 0.5f) * 2f * axisA
                    + (percent.y - 0.5f) * 2f * axisB;

                UnityEngine.Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                UnityEngine.Vector3 pointOnPlanet = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

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
