using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralCone : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    // How many triangles make up the curve of the fan. 
    // Higher = smoother curve, but more performance cost.
    [SerializeField] private int _segments = 20; 

    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
        _mesh.name = "ProceduralAimCone";
        _meshFilter.mesh = _mesh;
    }

    public void DrawCone(float spreadAngle, float range)
    {
        int numVertices = _segments + 2; // +1 for the center point, +1 for the end point
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[_segments * 3];

        // 1. Set the Origin (Player's center)
        vertices[0] = Vector3.zero;

        // 2. Calculate the start and end angles of the arc (centered on transform.right)
        float halfAngle = spreadAngle / 2f;
        float startAngle = -halfAngle;
        float endAngle = halfAngle;

        // 3. Generate the vertices along the arc
        for (int i = 0; i <= _segments; i++)
        {
            // Calculate what percentage of the way we are through the arc (0 to 1)
            float t = (float)i / _segments;
            
            // Linear interpolate between start and end angle
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            // Convert angle and range into (x, y) coordinate
            float angleRad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleRad) * range;
            float y = Mathf.Sin(angleRad) * range;

            // Assign the vertex position (assuming 2D XY plane)
            vertices[i + 1] = new Vector3(x, y, 0f);
        }

        // 4. Generate the triangle indices
        for (int i = 0; i < _segments; i++)
        {
            // All triangles connect back to the origin (vertex 0)
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.uv = uvs; // <--- MUST BE ASSIGNED HERE
        _mesh.triangles = triangles;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
    }
}