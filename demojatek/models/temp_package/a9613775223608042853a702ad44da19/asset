using UnityEngine;

namespace Farming_World.Demo.Resource
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 30f;

        void Start()
        {
            // Apply a random initial rotation while keeping the same up axis
            float randomAngle = Random.Range(0f, 360f);
            transform.localRotation = Quaternion.AngleAxis(randomAngle, rotationAxis);
        }

        void Update()
        {
            transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime), Space.Self);
        }
    }
}