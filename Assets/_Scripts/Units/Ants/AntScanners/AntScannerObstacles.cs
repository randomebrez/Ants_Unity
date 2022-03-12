using mew;
using UnityEngine;

public class AntScannerObstacles : AntScannerBase
{
    private Mesh _mesh;

    protected override float ScannerAngle => _ant.Stats.VisionAngle;
    protected override bool CheckObtruction => true;

    protected override float ScannerRadius => _ant.Stats.VisionRadius;

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return !Physics.Linecast(from, to, OcclusionLayer);
    }

    public Mesh CreateWedgeMesh(float height)
    {
        var mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -ScannerAngle, 0) * Vector3.right * ScannerRadius;
        Vector3 bottomRight = Quaternion.Euler(0, ScannerAngle, 0) * Vector3.right * ScannerRadius;

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

        var currentAngle = -ScannerAngle;
        var deltaAngle = (2 * ScannerAngle) / segments;

        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.right * ScannerRadius;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.right * ScannerRadius;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

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

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i ++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _mesh = mesh;
        return mesh;
    }

    public override float GetPortionValue(int index)
    {
        var sum = 0f;
        foreach(var obj in Objects[index])
        {
            sum+= Vector3.Distance(_ant.transform.position, obj.transform.position) / ScannerRadius;
        }
        return sum == 0f ? 1 : sum;
    }

    protected override bool IsInSight(GameObject obj)
    {
        var position = transform.position;
        var objPosition = obj.transform.position;
        var direction = objPosition - position;

        if (direction.magnitude < _ant.PhysicalLength * 2)
            return true;

        return base.IsInSight(obj);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }

        if (_mesh != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }
    }
}
