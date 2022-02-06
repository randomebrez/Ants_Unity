using mew;
using UnityEngine;

public class AntVision : MonoBehaviour
{
    public Color meshColor = Color.blue;

    private Mesh _mesh;

    private void Start()
    {
        var ant = GetComponentInParent<BaseAnt>();
        _mesh = CreateWedgeMesh(ant.Stats.VisionRadius, ant.Stats.VisionAngle, 0.1f);
    }

    private Mesh CreateWedgeMesh(float distance, float angle, float height)
    {
        var mesh = new Mesh();

        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.right * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.right * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vertice = 0;

        // left side
        vertices[vertice++] = bottomCenter;
        vertices[vertice++] = bottomLeft;
        vertices[vertice++] = topLeft;

        vertices[vertice++] = topLeft;
        vertices[vertice++] = topCenter;
        vertices[vertice++] = bottomCenter;

        // right side 
        vertices[vertice++] = bottomCenter;
        vertices[vertice++] = topCenter;
        vertices[vertice++] = topRight;

        vertices[vertice++] = topRight;
        vertices[vertice++] = bottomRight;
        vertices[vertice++] = bottomCenter;

        // far side
        vertices[vertice++] = bottomLeft;
        vertices[vertice++] = bottomRight;
        vertices[vertice++] = topRight;

        vertices[vertice++] = topRight;
        vertices[vertice++] = topLeft;
        vertices[vertice++] = bottomLeft;

        // top side
        vertices[vertice++] = topCenter;
        vertices[vertice++] = topLeft;
        vertices[vertice++] = topRight;

        // bot side
        vertices[vertice++] = topCenter;
        vertices[vertice++] = bottomRight;
        vertices[vertice++] = bottomLeft;

        for (int i = 0; i < numVertices; i ++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnDrawGizmos()
    {
        if (_mesh != null)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }
    }
}
