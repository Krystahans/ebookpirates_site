using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject areaMesh;
    public int spawnAttempts = 10;
    public int numberOfObjects = 10;
    public LayerMask overlapLayerMask = ~0;
    public QueryTriggerInteraction overlapQueryTrigger = QueryTriggerInteraction.Ignore;

    // Keep track of spawned objects so they can be cleared later by SceneManager
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public Vector2 gaussianCenter = Vector2.zero;
    public float gaussianStdX = 100f;
    public float gaussianStdZ = 100f;
    [Tooltip("Offset radius from the origin used to place the Gaussian peak. If zero, peak uses `gaussianCenter`.")]
    public float centralNoSpawnRadius = 0f;

    private float minX = -650f, maxX = 620f, minZ = -710f, maxZ = 790f;
    private float spawnY = -2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnRandomInsideMesh(spawnAttempts);
        }
    }

    public void SpawnRandomInsideMesh(int attempts)
    {
        for (int i = 0; i < attempts; i++)
        {
            float x, z;
            // Determine the Gaussian peak center. If centralNoSpawnRadius > 0,
            // place the peak on a circle of that radius from origin. If a direction
            // is provided via gaussianCenter, use that direction, otherwise pick a random direction.
            Vector2 sampleCenter = gaussianCenter;
            if (centralNoSpawnRadius > 0f)
            {
                if (sampleCenter.sqrMagnitude == 0f)
                {
                    float theta = Random.value * Mathf.PI * 2f;
                    sampleCenter = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * centralNoSpawnRadius;
                }
                else
                {
                    sampleCenter = sampleCenter.normalized * centralNoSpawnRadius;
                }
            }

            Vector3 pos = Gauss.NextGaussianXZ(sampleCenter, gaussianStdX, gaussianStdZ, minX, maxX, minZ, maxZ, spawnY);
            x = pos.x;
            z = pos.z;
            if (MeshAreaChecker.IsWithinMesh(areaMesh, x, z))
            {
                Vector3 position = new Vector3(x, spawnY, z);
                var spawned = Instantiate(objectToSpawn, position, transform.rotation);
                spawnedObjects.Add(spawned);
                if (HasAnyOverlap(spawned))
                {
                    Destroy(spawned);
                    spawnedObjects.Remove(spawned);
                    Debug.Log($"Spawner: Spawned object at ({x}, {spawnY}, {z}) overlaps existing objects, retrying.");
                    continue; // try again
                }
                //Debug.Log($"Spawner: Spawned object at ({x}, {spawnY}, {z}).");
                return;
            }
            else
            {
                i--;
                //Debug.Log($"Spawner: Position ({x}, {z}) is outside the mesh area, retrying.");
            }
        }
        
    }

    private bool HasAnyOverlap(GameObject spawned)
    {
        if (spawned == null) return false;

        var spawnColliders = spawned.GetComponentsInChildren<Collider>(true);
        foreach (var c in spawnColliders)
        {
            Collider[] hits = null;
            if (c is CapsuleCollider capsule)
            {
                Vector3 center = capsule.transform.TransformPoint(capsule.center);
                float maxScale = Mathf.Max(capsule.transform.lossyScale.x, capsule.transform.lossyScale.y, capsule.transform.lossyScale.z);
                float radius = capsule.radius * maxScale;
                // Determine capsule direction
                Vector3 dir;
                switch (capsule.direction)
                {
                    case 0: dir = capsule.transform.right; break; // X
                    case 1: dir = capsule.transform.up; break; // Y
                    default: dir = capsule.transform.forward; break; // Z
                }
                float halfHeight = Mathf.Max(0f, (capsule.height * 0.5f - capsule.radius));
                Vector3 point0 = capsule.transform.TransformPoint(capsule.center + dir * halfHeight);
                Vector3 point1 = capsule.transform.TransformPoint(capsule.center - dir * halfHeight);
                hits = Physics.OverlapCapsule(point0, point1, radius, overlapLayerMask, overlapQueryTrigger);
            }
            else
            {
                Bounds b = c.bounds;
                hits = Physics.OverlapBox(b.center, b.extents, c.transform.rotation, overlapLayerMask, overlapQueryTrigger);
            }

            if (hits != null && hits.Length > 0)
            {
                foreach (var h in hits)
                {
                    if (h == null) continue;
                    if (h.transform.IsChildOf(spawned.transform))
                        continue;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Spawns a single object at the exact specified position.
    /// </summary>
    public void SpawnAt(Vector3 position)
    {
        var spawned = Instantiate(objectToSpawn, position, transform.rotation);
        spawnedObjects.Add(spawned);
    }

    /// <summary>
    /// Destroys and clears all objects previously spawned by this spawner.
    /// </summary>
    public void ClearSpawned()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            var go = spawnedObjects[i];
            if (go != null) Destroy(go);
        }
        spawnedObjects.Clear();
    }

    /// <summary>
    /// Set the count and respawn that many objects using the existing spawnAttempts value.
    /// </summary>
    public void RespawnCount(int count)
    {
        numberOfObjects = count;
        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnRandomInsideMesh(spawnAttempts);
        }
        Debug.Log($"Spawner: Respawned {numberOfObjects} objects.");
    }
}
