using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Utilities
{
    public static class StaticHelper
    {
        public static void SetRecursiveLayer(List<GameObject> objects, int layerId)
        {
            if (objects.Count == 0)
                return;

            var newList = new List<GameObject>();

            foreach (var obj in objects)
            {
                obj.layer = layerId;

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    newList.Add(obj.transform.GetChild(i).gameObject);
                }
            }

            SetRecursiveLayer(newList, layerId);
        }

        // Todo : to delete ?
        public static Mesh CreateDiagramMesh(float height, float alpha, float radius)
        {
            var mesh = new Mesh();

            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            Color[] colors = new Color[numVertices];
            int[] triangles = new int[numVertices];

            int vertice = 0;

            var currentAngle = - alpha;
            var deltaAngle = (2 * alpha) / segments;

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
            Vector3 bottomRight = Quaternion.Euler(0, 0, currentAngle + 2 * alpha) * Vector3.right * radius;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;

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

            for (int j = 0; j < segments; j++)
            {
                bottomLeft = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
                bottomRight = Quaternion.Euler(0, 0, currentAngle + deltaAngle) * Vector3.right * radius;

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

            for (int j = 0; j < numVertices; j++)
            {
                triangles[j] = j;
                colors[j] = Color.white;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh CreateFullWedgeMesh(float height, float alpha, float radius, int portionNumber)
        {
            var mesh = new Mesh();

            int segments = 10;
            int numTriangles = ((segments * 4) + 2 + 2) * portionNumber;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            int vertice = 0;

            for (int i = 0; i < portionNumber; i++)
            {
                var currentAngle = (i - 1) * alpha;
                var deltaAngle = (2 * alpha) / segments;

                Vector3 bottomCenter = Vector3.zero;
                Vector3 bottomLeft = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
                Vector3 bottomRight = Quaternion.Euler(0, 0, currentAngle + 2 * alpha) * Vector3.right * radius;

                Vector3 topCenter = bottomCenter + Vector3.up * height;
                Vector3 topLeft = bottomLeft + Vector3.up * height;
                Vector3 topRight = bottomRight + Vector3.up * height;

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

                for (int j = 0; j < segments; j++)
                {
                    bottomLeft = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
                    bottomRight = Quaternion.Euler(0, 0, currentAngle + deltaAngle) * Vector3.right * radius;

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

                for (int j = 0; j < numVertices/portionNumber; j++)
                {
                    triangles[(i * numVertices / portionNumber) + j] = (i * numVertices / portionNumber) + j;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
