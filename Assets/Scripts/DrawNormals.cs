using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DrawNormals : MonoBehaviour
{
    public float normalLength = 0.5f;

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        var mesh = mf.sharedMesh;
        var vertices = mesh.vertices;
        var normals = mesh.normals;

        Gizmos.color = Color.red;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Vector3 worldNormal = transform.TransformDirection(normals[i]);
            Gizmos.DrawLine(worldPos, worldPos + worldNormal * normalLength);
        }
    }
}
