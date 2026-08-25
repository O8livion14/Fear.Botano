using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera")]
    public float distance = 5f;
    public float height = 2f;

    [Header("Look")]
    public float sensitivity = 2f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.05f;

    [Header("Collision")]
    public float collisionRadius = 0.2f;
    public LayerMask collisionLayers;

    private Vector2 lookInput;

    private float yaw;
    private float pitch;

    private Vector3 positionVelocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Rotate camera
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera position
        Vector3 targetPosition =
            target.position + Vector3.up * height;

        Vector3 desiredPosition =
            targetPosition - rotation * Vector3.forward * distance;

        // Camera collision
        Vector3 direction = desiredPosition - targetPosition;

        float actualDistance = distance;

        if (Physics.SphereCast(
            targetPosition,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            collisionLayers))
        {
            actualDistance = hit.distance - collisionRadius;
            actualDistance = Mathf.Max(actualDistance, 0.5f);
        }

        desiredPosition =
            targetPosition -
            rotation * Vector3.forward * actualDistance;

        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        transform.rotation = rotation;
    }

    // Called by the Input System
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}