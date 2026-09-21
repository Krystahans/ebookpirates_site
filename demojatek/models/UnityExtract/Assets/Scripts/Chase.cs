using UnityEngine;

/// <summary>
/// Chases the player on a horizontal plane (X/Z axes only).
/// Does not move vertically (Y axis remains unchanged).
/// </summary>
public class Chase : MonoBehaviour
{
    public static Chase Instance { get; private set; }
    [Tooltip("Reference to the player object to chase")]
    public Transform player;

    public GameObject DeathZone;

    [Tooltip("Speed at which to move towards the player")]
    public float chaseSpeed = 10f;

    [Tooltip("Speed at which to turn towards the player")]
    public float turnSpeed = 2f;

    [Tooltip("If true, use Rigidbody for physics-driven movement; otherwise move transform directly")]
    public bool useRigidbody = true;

    Rigidbody rb;
    private static Vector3 startPosition = new Vector3(-386f, -7.5f, 758.6f);

    // When true the balloon is allowed to move; when false the balloon is stationary.
    [Tooltip("When true, the balloon is allowed to move; when false the balloon is stationary.")]
    public bool ChaseOn = false;

    [Tooltip("Combined threshold: Stored + Carried must be >= this to trigger ascent and chasing")]
    public int requiredTotal = 4;

    [Tooltip("Height to move up before horizontal chasing starts")]
    public float ascentHeight = 60f;

    [Tooltip("Vertical speed for the ascent (units per second)")]
    public float ascentSpeed = 10f;
    
    private TopBarManager topBarManager;
    // Internal ascent state
    private bool hasAscended = false;
    private bool isAscending = false;
    private float ascentTargetY = 0f;

    // Track whether we were previously in ChaseOn so we can capture stationary position
    private bool prevChaseOn = false;
    private Vector3 stationaryPosition = startPosition;

    private AscensionScene ascensionScene;

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        topBarManager = FindAnyObjectByType<TopBarManager>();
        if (useRigidbody)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("Chase: useRigidbody is true but no Rigidbody found. Falling back to transform control.");
                useRigidbody = false;
            }
        }
        ascensionScene = FindAnyObjectByType<AscensionScene>(FindObjectsInactive.Include);
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        bool isAscensionActive = ascensionScene != null && ascensionScene.gameObject.activeInHierarchy;

        // If ChaseOn is false, do not move (hold stationary)
        if (!ChaseOn)
        {
            // Reset ascent so it will ascend when movement becomes allowed
            hasAscended = false;
            isAscending = false;

            // If we just entered stationary, remember our current position so we can lock it
            if (!prevChaseOn)
            {
                prevChaseOn = true;
                stationaryPosition = transform.position;
            }

            if (useRigidbody && rb != null)
            {
                // Stop all velocities so the balloon is fully stationary
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                // Lock transform to stationary position in case other code would move it
                transform.position = stationaryPosition;
            }

            if (isAscensionActive && DeathZone.activeSelf)
            {
                DeathZone.SetActive(false);
            }
            return;
        }

        // Clear prevChaseOn when leaving stationary state
        if (prevChaseOn)
            prevChaseOn = false;

        // If we haven't ascended yet, perform ascent first (no horizontal movement while ascending)
        if (!hasAscended)
        {
            DeathZone.SetActive(false);
            if (!isAscending)
            {
                isAscending = true;
                ascentTargetY = transform.position.y + ascentHeight;
            }

            if (useRigidbody && rb != null)
            {
                // Ascend by setting vertical velocity until we reach target Y
                if (transform.position.y < ascentTargetY - 0.01f)
                {
                    rb.linearVelocity = new Vector3(0f, ascentSpeed, 0f);
                    return; // still ascending
                }
                else
                {
                    hasAscended = true;
                    isAscending = false;
                    // zero vertical velocity so horizontal movement can take over
                    rb.linearVelocity = Vector3.zero;
                }
            }
            else
            {
                // Move transform upwards smoothly
                float newY = Mathf.MoveTowards(transform.position.y, ascentTargetY, ascentSpeed * Time.fixedDeltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if (Mathf.Abs(transform.position.y - ascentTargetY) < 0.01f)
                {
                    hasAscended = true;
                    isAscending = false;
                }
                else
                {
                    return; // still ascending
                }
            }
        }

        // Get current positions
        Vector3 myPos = transform.position;
        Vector3 playerPos = player.position;

        // Project positions onto horizontal plane (ignore Y)
        Vector3 myPosHorizontal = new Vector3(myPos.x, 0f, myPos.z);
        Vector3 playerPosHorizontal = new Vector3(playerPos.x, 0f, playerPos.z);

        // Calculate direction towards player on horizontal plane
        Vector3 directionToPlayer = (playerPosHorizontal - myPosHorizontal).normalized;

        // Rotate towards the player
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            if (useRigidbody && rb != null)
            {
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            }
        }

        // Calculate desired velocity (horizontal only)
        Vector3 desiredVelocity = directionToPlayer * chaseSpeed;

        if (useRigidbody && rb != null)
        {
            // Preserve current vertical velocity
            Vector3 newVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);
            rb.linearVelocity = newVelocity;
        }
        else
        {
            // Move transform horizontally over time
            Vector3 horizontalMovement = directionToPlayer * chaseSpeed * Time.fixedDeltaTime;
            Vector3 newPos = myPos + horizontalMovement;
            transform.position = newPos;
        }

        // Manage DeathZone visibility based on AscensionScene state
        if (hasAscended)
        {
            if (isAscensionActive)
            {
                if (DeathZone.activeSelf) DeathZone.SetActive(false);
            }
            else
            {
                if (!DeathZone.activeSelf) DeathZone.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Reset this balloon to its starting position and clear motion/state.
    /// </summary>
    public void ResetToStart()
    {
        transform.position = startPosition;
        hasAscended = false;
        isAscending = false;
        prevChaseOn = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        Debug.Log("Chase: Reset to start position.");
    }

    /// <summary>
    /// Set the player target at runtime.
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
