using mew;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AntVision : MonoBehaviour
{
    private BaseAnt _ant;
    private ScriptableUnitBase.Stats _statistics;
    private float _height = 0.1f;

    public Color meshColor = Color.blue;
    private Mesh _mesh;


    public LayerMask ObjectsLayer;
    public LayerMask OcclusionLayer;

    public List<GameObject> Collectables
    {
        get
        {
            _collectables.RemoveAll(t => !t);
            return _collectables;
        }
    }
    public List<GameObject> Obstacles
    {
        get
        {
            _obstacles.RemoveAll(t => !t);
            return _obstacles;
        }
    }
    private List<GameObject> _collectables = new List<GameObject>();
    private List<GameObject> _obstacles = new List<GameObject>();
    private Collider[] _colliders = new Collider[50];
    private int _count;

    public int ScanFrequency = 100;
    private float _scanInterval;
    private float _scanTimer;

    private void Start()
    {
        _ant = GetComponentInParent<BaseAnt>();
        _statistics = _ant.Stats;
        _mesh = CreateWedgeMesh(_height);

        _scanInterval = 1.0f / ScanFrequency;
    }

    private void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0)
        {
            _scanTimer += _scanInterval;
            Scan();
            ScanObstables();
        }
    }

    private void Scan()
    {
        _count = Physics.OverlapSphereNonAlloc(transform.position, _statistics.VisionRadius, _colliders, ObjectsLayer, QueryTriggerInteraction.Collide);

        _collectables.Clear();
        for (int i = 0; i < _count; i++)
        {
            var obj = _colliders[i].gameObject;
            if (IsInsight(obj))
                _collectables.Add(obj);
        }
    }

    private void ScanObstables()
    {
        _count = Physics.OverlapSphereNonAlloc(transform.position, _statistics.VisionRadius, _colliders, OcclusionLayer, QueryTriggerInteraction.Collide);

        _obstacles.Clear();
        for (int i = 0; i < _count; i++)
        {
            var obj = _colliders[i].gameObject;
            if (IsInsight(obj, false))
                _obstacles.Add(obj);
        }
    }

    public bool IsInsight(GameObject obj, bool checkOcclusion = true)
    {
        var position = transform.position;
        var objPosition = obj.transform.position;
        var direction = objPosition - position;

        /*if (distance.y < 0 || distance.y > _height)
            return false;*/

        // Check on angle
        if (Mathf.Abs(Vector3.SignedAngle(_ant.BodyHeadAxis, direction, Vector3.up)) > _statistics.VisionAngle)
            return false;

        // Check if no obstruction
        position.y += _height / 2;
        direction.y = position.y;
        if (checkOcclusion && Physics.Linecast(position, direction, OcclusionLayer))
            return false;

        return true;
    }

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return !Physics.Linecast(from, to, OcclusionLayer);
    }

    private Mesh CreateWedgeMesh(float height)
    {
        var mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -_statistics.VisionAngle, 0) * Vector3.right * _statistics.VisionRadius;
        Vector3 bottomRight = Quaternion.Euler(0, _statistics.VisionAngle, 0) * Vector3.right * _statistics.VisionRadius;

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

        var currentAngle = -_statistics.VisionAngle;
        var deltaAngle = (2 * _statistics.VisionAngle) / segments;

        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.right * _statistics.VisionRadius;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.right * _statistics.VisionRadius;

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

        return mesh;
    }


    private void OnDrawGizmos()
    {
        if (_mesh != null)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + _ant.BodyHeadAxis);

        foreach(var obj in _collectables)
        {
            if (obj == null)
                continue;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(obj.transform.position, 1f);
        }

        foreach (var obj in _obstacles)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(obj.transform.position, Vector3.one);
        }
    }
}
