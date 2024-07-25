using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;
    // The object the camera will follow
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform cameraTransform;
    [SerializeField] LayerMask collisionLayers;
    private float defaultPosition;

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    [Header("Camera Collision")]
    [SerializeField] float cameraCollisionOffset = 0.2f;
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] float minimumCollisionOffset = 0.2f;

    [Header("Camera Settings")]
    [SerializeField] float cameraFollowSpeed = 0.2f;
    [SerializeField] float cameraLookSpeed = 2.0f;
    [SerializeField] float cameraPivotSpeed = 2.0f;
    [SerializeField] float lookAngle;
    [SerializeField] float pivotAngle;
    [SerializeField] float minimumPivotAngle = -35.0f;
    [SerializeField] float maximumPivotAngle = 45.0f;

    private void Awake() {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;

        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.position.z;
    }

    public void HandleCameraMovement() {
        FollowPlayer();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowPlayer() {
        Vector3 targetPosition = Vector3.SmoothDamp(
            transform.position,
            targetTransform.position,
            ref cameraFollowVelocity,
            cameraFollowSpeed
            );

        transform.position = targetPosition;
    }

    private void RotateCamera() {
        lookAngle += inputManager.cameraHorizontalInput * cameraLookSpeed;
        pivotAngle -= inputManager.cameraVerticalInput * cameraPivotSpeed;

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);

        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions() {
        float targetPosition = defaultPosition;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(
            cameraPivot.transform.position,
            cameraCollisionRadius, direction,
            out hit, Mathf.Abs(targetPosition), collisionLayers)) {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
            targetPosition -= minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
