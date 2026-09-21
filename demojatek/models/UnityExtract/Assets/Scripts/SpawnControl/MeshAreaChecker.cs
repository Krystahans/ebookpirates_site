using UnityEngine;

public static class MeshAreaChecker
{
    // Check if the X/Z point is inside the provided mesh object's area (projected on XZ plane).
    public static bool IsWithinMesh(GameObject meshObject, float x, float z)
    {
        if (meshObject == null)
            return false;

        Mesh mesh = null;
        var mf = meshObject.GetComponent<MeshFilter>();
        if (mf != null)
            mesh = mf.sharedMesh;
        else
        {
            var mc = meshObject.GetComponent<MeshCollider>();
            if (mc != null)
                mesh = mc.sharedMesh;
        }

        if (mesh == null)
            return false;

        return IsWithinMesh(mesh, meshObject.transform, x, z);
    }

    // Check if the X/Z point is inside the provided Mesh (with a Transform applied), projected on XZ plane.
    public static bool IsWithinMesh(Mesh mesh, Transform transform, float x, float z)
    {
        if (mesh == null || transform == null)
            return false;

        Vector2 p = new Vector2(x, z);
        var verts = mesh.vertices;
        var tris = mesh.triangles;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = transform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = transform.TransformPoint(verts[tris[i + 2]]);

            Vector2 p0 = new Vector2(v0.x, v0.z);
            Vector2 p1 = new Vector2(v1.x, v1.z);
            Vector2 p2 = new Vector2(v2.x, v2.z);

            if (PointInTriangle(p, p0, p1, p2))
                return true;
        }

        return false;
    }

    // 2D point-in-triangle test (barycentric coordinates). Works in XZ plane.
    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denom = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
        if (Mathf.Approximately(denom, 0f))
            return false; // degenerate triangle

        float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / denom;
        float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / denom;
        float gamma = 1f - alpha - beta;

        // Accept points on the edge as 'inside'
        return alpha >= 0f && beta >= 0f && gamma >= 0f;
    }
}
