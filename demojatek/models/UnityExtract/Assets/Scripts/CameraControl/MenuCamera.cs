using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Tooltip("The player/target object the camera orbits around and looks at")]
    public Transform Boat;

    [Tooltip("Distance the camera maintains from the player")]
    public float orbitDistance = 30f;

    [Tooltip("Speed of rotation in degrees per second")]
    public float rotationSpeed = 10f;

    [Tooltip("Vertical angle (pitch) in degrees")]
    public float verticalAngle = 20f;

    [Tooltip("Vertical offset for the look-at point (camera focuses above the player)")]
    public float lookAtOffset = 10f;

    private float horizontalAngle = 0f;

    void LateUpdate()
    {
        if (Boat == null)
            return;

        // Update rotation angle
        horizontalAngle += rotationSpeed * Time.deltaTime;

        // Calculate camera position relative to player
        // Rotate around X (pitch) to get elevation, then Y (yaw) to orbit
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

        // Camera offset: straight back from player at orbit distance
        Vector3 cameraOffset = rotation * new Vector3(0f, 0f, -orbitDistance);

        // Set camera position
        transform.position = Boat.position + cameraOffset;

        // Always look at the player, with offset above
        Vector3 lookAtPoint = Boat.position + Vector3.up * lookAtOffset;
        transform.LookAt(lookAtPoint);
    }

}
