using UnityEngine;

public class WindManger : MonoBehaviour
{
    /// <summary>
    /// Normalized horizontal direction of the wind (y == 0)
    /// </summary>
    public Vector3 Direction { get; private set; } = Vector3.forward;

    /// <summary>
    /// Magnitude of the wind
    /// </summary>
    public float Force { get; private set; } = 1f;

    /// <summary>
    /// Combined wind vector = Direction * Force
    /// </summary>
    public Vector3 Wind => Direction * Force;

    /// <summary>
    /// Event invoked whenever the wind changes. Use this to notify the player so only the player responds to wind.
    /// </summary>
    public event System.Action OnWindChanged;

    [Header("Wind Configuration")]
    [Tooltip("Scale applied to wind when affecting the player.")]
    public float windMultiplier = 1f;

    [Tooltip("If true, uses Rigidbody.AddForce; otherwise wind is applied as a positional displacement.")]
    public bool applyWindAsForce = false;

    [Tooltip("If true the wind direction and force will be randomized automatically in Start")]
    [SerializeField] private bool randomizeOnStart = true;

    void Start()
    {
        if (randomizeOnStart) SetRandomWind();
    }

    /// <summary>
    /// Randomize both direction (horizontal) and force (5..20)
    /// </summary>
    public void SetRandomWind()
    {
        SetRandomHorizontalDirection();
        SetRandomForce();
        OnWindChanged?.Invoke();
        Debug.Log($"WindManger: New wind set. Direction={Direction}, Force={Force}, Wind={Wind}");
    }

    /// <summary>
    /// Sets a random horizontal (y=0) normalized direction.
    /// </summary>
    public void SetRandomHorizontalDirection()
    {
        var dir = Random.onUnitSphere;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;
        Direction = dir.normalized;
    }

    /// <summary>
    /// Sets a random force between 5 and 20.
    /// </summary>
    public void SetRandomForce()
    {
        Force = Random.Range(0f, 10f);
    }
    /// <summary>
    /// Applies wind to the rigidbody. 
    /// If configured as force, adds force and returns Vector3.zero.
    /// If configured as displacement, returns the displacement vector to be added to MovePosition.
    /// </summary>
    public Vector3 ApplyWind(Rigidbody rb, float deltaTime)
    {
        if (!this.enabled) return Vector3.zero;

        Vector3 currentWind = Wind;
        currentWind.y = 0f;

        if (applyWindAsForce)
        {
            rb.AddForce(currentWind, ForceMode.Force);
            return Vector3.zero;
        }
        else
        {
            return currentWind * windMultiplier * deltaTime;
        }
    }
}