using System.Collections;
using UnityEngine;

/// <summary>
/// Stabilizer records the object's initial local rotation and provides methods
/// to restore the X (pitch) and Z (roll) components while preserving the
/// current Y (yaw).
///
/// Use `RestorePitchRollImmediate()` to snap immediately, or
/// `RestorePitchRoll(speed)` to smoothly restore with a given speed (units/sec).
/// Set `autoRestore` to true to smooth-restore automatically in Update.
/// </summary>
public class Stabilizer : MonoBehaviour
{
    [Tooltip("Automatically restore pitch & roll each frame using autoRestoreSpeed when enabled.")]
    public bool autoRestore = false;

    [Tooltip("Speed (per second) used when smoothing restoration. Use 0 for immediate.")]
    public float autoRestoreSpeed = 3f;

    Vector3 initialEuler; // world-space initial rotation
    Coroutine restoreCoroutine;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // record initial world rotation so stabilization preserves heading changes
        initialEuler = transform.eulerAngles;
    }

    void Update()
    {
        if (autoRestore)
        {
            // Perform inline smoothing each frame instead of restarting coroutines every Update.
            if (autoRestoreSpeed <= 0f)
            {
                RestorePitchRollImmediate();
            }
            else
            {
                Vector3 cur = (rb != null) ? rb.rotation.eulerAngles : transform.eulerAngles;
                float curX = cur.x;
                float curZ = cur.z;
                float targetX = initialEuler.x;
                float targetZ = initialEuler.z;

                float nx = Mathf.LerpAngle(curX, targetX, Mathf.Clamp01(autoRestoreSpeed * Time.deltaTime));
                float nz = Mathf.LerpAngle(curZ, targetZ, Mathf.Clamp01(autoRestoreSpeed * Time.deltaTime));

                if (rb != null)
                    rb.MoveRotation(Quaternion.Euler(nx, cur.y, nz));
                else
                    transform.eulerAngles = new Vector3(nx, cur.y, nz);
            }
        }
    }

    /// <summary>
    /// Immediately sets local X and Z rotation to the recorded initial values,
    /// preserving the current local Y rotation.
    /// </summary>
    public void RestorePitchRollImmediate()
    {
        Vector3 cur = (rb != null) ? rb.rotation.eulerAngles : transform.eulerAngles;
        if (rb != null)
            rb.MoveRotation(Quaternion.Euler(initialEuler.x, cur.y, initialEuler.z));
        else
            transform.eulerAngles = new Vector3(initialEuler.x, cur.y, initialEuler.z);
    }

    /// <summary>
    /// Smoothly restores local X and Z to the initial values. If speed &lt;= 0 sets immediately.
    /// </summary>
    /// <param name="speed">Smoothing speed per second. 0 for immediate.</param>
    public void RestorePitchRoll(float speed = 0f)
    {
        if (speed <= 0f)
        {
            RestorePitchRollImmediate();
            return;
        }

        // Start smoothing coroutine only when called programmatically. The autoRestore path
        // uses an inline smooth step to ensure Update doesn't restart coroutines each frame.
        if (restoreCoroutine != null)
            StopCoroutine(restoreCoroutine);

        restoreCoroutine = StartCoroutine(RestoreSmoothCoroutine(speed));
    }

    IEnumerator RestoreSmoothCoroutine(float speed)
    {
        while (true)
        {
            // Smooth only X and Z angles using LerpAngle so Y is never modified.
            Vector3 cur = (rb != null) ? rb.rotation.eulerAngles : transform.eulerAngles;
            float yaw = cur.y;

            float nx = Mathf.LerpAngle(cur.x, initialEuler.x, Mathf.Clamp01(speed * Time.deltaTime));
            float nz = Mathf.LerpAngle(cur.z, initialEuler.z, Mathf.Clamp01(speed * Time.deltaTime));

            if (rb != null)
                rb.MoveRotation(Quaternion.Euler(nx, yaw, nz));
            else
                transform.eulerAngles = new Vector3(nx, yaw, nz);

            // If both angles are close enough to targets, snap and finish.
            float dx = Mathf.DeltaAngle(nx, initialEuler.x);
            float dz = Mathf.DeltaAngle(nz, initialEuler.z);
            if (Mathf.Abs(dx) < 0.5f && Mathf.Abs(dz) < 0.5f)
            {
                if (rb != null)
                    rb.MoveRotation(Quaternion.Euler(initialEuler.x, yaw, initialEuler.z));
                else
                    transform.eulerAngles = new Vector3(initialEuler.x, yaw, initialEuler.z);
                restoreCoroutine = null;
                yield break;
            }
            yield return null;
        }
    }
}
