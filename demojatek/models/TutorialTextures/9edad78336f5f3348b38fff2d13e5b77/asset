using UnityEngine;

public class WindRotate : MonoBehaviour
{
    public WindManger windManager;

    void Update()
    {
        if (windManager != null && windManager.enabled)
        {
            // Ensure there is a wind direction to react to
            if (windManager.Direction != Vector3.zero)
            {
                // Calculate the direction opposite to the wind
                Vector3 oppositeDirection = windManager.Direction;

                // Create a rotation that looks in the opposite direction
                Quaternion targetRotation = Quaternion.LookRotation(oppositeDirection);

                // Smoothly rotate towards the target rotation
                float rotationSpeed = windManager.Force/10f * 4;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
