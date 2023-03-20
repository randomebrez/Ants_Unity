using Assets._Scripts.Utilities;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    public Vector3[] Vertices;

    public void Awake()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    public void Start()
    {
        _mesh = CreateMesh();
        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
        Vertices = _mesh.vertices;
    }

    public Mesh CreateMesh(int sideNumber = 6)
    {
        var mesh = new Mesh();
        var numTriangles = 4 * sideNumber;
        var numVertices = 3 * numTriangles;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        Color[] colors = new Color[numVertices];

        var deltaAngle = 360 / sideNumber;

        Vector3 topCenter = Vector3.zero;
        Vector3 top = Quaternion.Euler(0, - deltaAngle / 2, 0) * (topCenter + Vector3.right);

        Vector3 botCenter = Vector3.zero - 2 * GlobalParameters.NodeRadius * Vector3.up;
        Vector3 bot = Quaternion.Euler(0, - deltaAngle / 2, 0) * (botCenter + Vector3.right);

        int verticeIndex = 0;
        for (int i = 0; i < sideNumber; i++)
        {
            // Face top
            vertices[verticeIndex++] = Quaternion.Euler(0, (i + 1) * deltaAngle, 0) * top;
            vertices[verticeIndex++] = topCenter;
            vertices[verticeIndex++] = Quaternion.Euler(0, i * deltaAngle, 0) * top;

            //Face verticale
            vertices[verticeIndex++] = Quaternion.Euler(0, i * deltaAngle, 0) * top;
            vertices[verticeIndex++] = Quaternion.Euler(0, (i + 1) * deltaAngle, 0) * top;
            vertices[verticeIndex++] = Quaternion.Euler(0, (i + 1) * deltaAngle, 0) * bot;

            vertices[verticeIndex++] = Quaternion.Euler(0, (i + 1) * deltaAngle, 0) * bot;            
            vertices[verticeIndex++] = Quaternion.Euler(0, i * deltaAngle, 0) * bot;
            vertices[verticeIndex++] = Quaternion.Euler(0, i * deltaAngle, 0) * top;

            // Face bot
            vertices[verticeIndex++] = botCenter;
            vertices[verticeIndex++] = Quaternion.Euler(0, i * deltaAngle, 0) * bot;
            vertices[verticeIndex++] = Quaternion.Euler(0, (i + 1) * deltaAngle, 0) * bot;
        }
            

        for (int j = 0; j < numVertices; j++)
        {
            triangles[j] = j;
            colors[j] = Color.green;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }

    /*
    private void OnDrawGizmos()
    {
        if (_mesh == null)
            return;

        //Gizmos.color = Color.green;
        //Gizmos.DrawMesh(_mesh);
    }*/
}
