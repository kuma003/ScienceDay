using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Polygon2DMesh : MonoBehaviour
{
    [SerializeField] public List<Vector2> polygonPoints = new List<Vector2>();

    public Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Polygon2DMesh";
        Vector3[] vertices = new Vector3[polygonPoints.Count];
        for (int i = 0; i < polygonPoints.Count; i++)
            vertices[i] = new Vector3(polygonPoints[i].x, polygonPoints[i].y, 0);

        List<int> triangles = Triangulate(polygonPoints);

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private List<int> Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();
        List<int> verts = new List<int>();
        for (int i = 0; i < points.Count; i++)
            verts.Add(i);

        int guard = 0;
        while (verts.Count > 3 && guard++ < 1000)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                int prev = verts[(i - 1 + verts.Count) % verts.Count];
                int curr = verts[i];
                int next = verts[(i + 1) % verts.Count];

                Vector2 a = points[prev];
                Vector2 b = points[curr];
                Vector2 c = points[next];

                if (Vector3.Cross(b - a, c - a).z > 0)
                {
                    bool isEar = true;
                    for (int j = 0; j < verts.Count; j++)
                    {
                        if (j == prev || j == curr || j == next) continue;
                        if (PointInTriangle(points[verts[j]], a, b, c))
                        {
                            isEar = false;
                            break;
                        }
                    }

                    if (isEar)
                    {
                        indices.Add(prev);
                        indices.Add(curr);
                        indices.Add(next);
                        verts.RemoveAt(i);
                        break;
                    }
                }

            }
        }
        return indices;
    }

    private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float s = 1f / (2f * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
        float t = 1f / (2f * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
        return s >= 0 && t >= 0 && (s + t) <= 1;
    }
}
