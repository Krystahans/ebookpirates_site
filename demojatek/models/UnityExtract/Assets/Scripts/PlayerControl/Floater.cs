using UnityEngine;

/// <summary>
/// Keeps the GameObject at a given world Y level using a "pressure" parameter.
/// - When `useRigidbody` is true the script applies forces in FixedUpdate so it works with physics.
///   The `pressure` value is interpreted as acceleration-per-meter (so it's mass-independent).
/// - When `useRigidbody` is false you can choose `hardLock` to snap exactly to the level or
///   let it smoothly approach the level using `pressure` as a stiffness factor.
/// </summary>
public class Floater : MonoBehaviour
{
    [Header("Target Level")]
    [Tooltip("Target world Y coordinate to hold the object at.")]
    public float targetY = 0f;

    [Tooltip("If true, the target is treated as an offset from the object's initial Y.")]
    public bool targetIsOffset = false;

    [Header("Control")]
    [Tooltip("If true the script applies forces to a Rigidbody; otherwise it moves the transform.")]
    public bool useRigidbody = true;

    [Tooltip("When not using Rigidbody, snap exactly to the target when true.")]
    public bool hardLock = false;

    [Tooltip("Stiffness: larger values make the object correct position faster. Interpreted as acceleration per meter when using Rigidbody.")]
    public float pressure = 30f;

    [Tooltip("Damping applied to vertical velocity (only used with Rigidbody).")]
    public float damping = 2f;

    Rigidbody rb;
    float initialY;

    void Awake()
    {
        initialY = transform.position.y;
        if (useRigidbody)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("Floater: useRigidbody is true but no Rigidbody was found on the GameObject. Falling back to transform control.");
                useRigidbody = false;
            }
        }
    }

    void Start()
    {
        if (targetIsOffset)
            targetY = initialY + targetY;
    }

    void FixedUpdate()
    {
        if (!useRigidbody) return;

        // Compute error in meters
        float error = targetY - transform.position.y;

        // Desired acceleration = error * pressure - current vertical velocity * damping
        float vertVel = rb.linearVelocity.y;
        float desiredAcc = error * pressure - vertVel * damping;

        // Force = mass * acceleration (makes pressure effectively mass-independent)
        Vector3 force = new Vector3(0f, desiredAcc * rb.mass, 0f);
        rb.AddForce(force, ForceMode.Force);
    }

    void Update()
    {
        if (useRigidbody) return;

        if (hardLock)
        {
            Vector3 p = transform.position;
            p.y = targetY;
            transform.position = p;
            return;
        }

        // Smoothly approach the target using a critically damped-like step.
        // Convert pressure (stiffness) into a lerp factor for frame-rate independence.
        float t = 1f - Mathf.Exp(-pressure * Time.deltaTime);
        Vector3 p2 = transform.position;
        p2.y = Mathf.Lerp(p2.y, targetY, t);
        transform.position = p2;
    }

    /// <summary>
    /// Update the target to a world Y coordinate at runtime.
    /// </summary>
    public void SetTargetY(float worldY, bool isOffset = false)
    {
        targetIsOffset = isOffset;
        if (isOffset)
            targetY = initialY + worldY;
        else
            targetY = worldY;
    }
}