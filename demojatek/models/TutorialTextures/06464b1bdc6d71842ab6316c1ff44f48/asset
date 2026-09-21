using UnityEngine;

/// <summary>
/// Orbit camera that rotates around the player character.
/// The camera maintains a fixed distance from the player and always faces them.
/// Mouse input controls the orbit rotation (horizontal and/or vertical).
/// </summary>
public class CameraMove : MonoBehaviour
{
    [Tooltip("The player/target object the camera orbits around and looks at")]
    public Transform player;

    [Tooltip("Distance the camera maintains from the player")]
    public float orbitDistance = 10f;

    [Tooltip("Sensitivity for horizontal mouse movement (rotation around Y)")]
    public float horizontalSensitivity = 3f;

    [Tooltip("Sensitivity for vertical mouse movement (rotation around player's right axis)")]
    public float verticalSensitivity = 3f;

    [Tooltip("Minimum vertical angle (pitch) in degrees")]
    public float minVerticalAngle = -30f;

    [Tooltip("Maximum vertical angle (pitch) in degrees")]
    public float maxVerticalAngle = 60f;

    [Tooltip("If true, invert vertical mouse input")]
    public bool invertVertical = false;

    [Tooltip("Vertical offset for the look-at point (camera focuses above the player)")]
    public float lookAtOffset = 2f;

    [Tooltip("Minimum orbit distance when zoomed in")]
    public float minOrbitDistance = 10f;

    [Tooltip("Maximum orbit distance when zoomed out")]
    public float maxOrbitDistance = 15f;

    [Tooltip("Scroll sensitivity for zooming the camera")]
    public float scrollSensitivity = 2f;

    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;
    private bool isLocked = false;

    public static CameraMove Instance { get; private set; }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        if (!isLocked)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Update rotation angles
            horizontalAngle += mouseX * horizontalSensitivity;

            float verticalDelta = invertVertical ? -mouseY : mouseY;
            verticalAngle += verticalDelta * verticalSensitivity;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

            // Handle scroll wheel zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                // scroll > 0 -> zoom in (decrease distance), scroll < 0 -> zoom out
                orbitDistance -= scroll * scrollSensitivity;
                orbitDistance = Mathf.Clamp(orbitDistance, minOrbitDistance, maxOrbitDistance);
            }
        }

        // Calculate camera position relative to player
        // Start with looking forward (along player's forward)
        Quaternion horizontalRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
        Quaternion verticalRotation = Quaternion.Euler(-verticalAngle, 0f, 0f);

        // Combine rotations
        Quaternion finalRotation = horizontalRotation * verticalRotation;

        // Camera offset: straight back from player at orbit distance
        Vector3 cameraOffset = finalRotation * new Vector3(0f, 0f, -orbitDistance);

        // Set camera position
        transform.position = player.position + cameraOffset;

        // Always look at the player, with offset above
        Vector3 lookAtPoint = player.position + Vector3.up * lookAtOffset;
        transform.LookAt(lookAtPoint);
    }

    /// <summary>
    /// Set the player/target the camera orbits around.
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    /// <summary>
    /// Reset camera angles to default (looking slightly down at player from behind).
    /// </summary>
    public void ResetAngles()
    {
        horizontalAngle = 0f;
        verticalAngle = 20f;
    }

    public void lockState(bool state = true)
    {
        isLocked = state;
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
