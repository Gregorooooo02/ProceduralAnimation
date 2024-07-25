using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    AnimationManager animationManager;
    PlayerManager playerManager;
    InputManager inputManager;

    private Transform cameraObject;
    private Rigidbody playerRigidbody;

    private Vector3 moveDirection;

    [Header("Falling")]
    [SerializeField] float inAirTimer;
    [SerializeField] float leapingVelocity;
    [SerializeField] float fallingVelocity;
    [SerializeField] float rayCastHeightOffset = 0.5f;
    [SerializeField] float maxDistance = 1.0f;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;

    [Header("Movement Speeds")]
    [SerializeField] float walkingSpeed = 1.5f;
    [SerializeField] float runningSpeed = 5.0f;
    [SerializeField] float sprintingSpeed = 7.0f;
    [SerializeField] float rotationSpeed = 15.0f;

    private void Awake() {
        animationManager = GetComponent<AnimationManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();

        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement() {
        HandleFallingAndLanding();
        if (playerManager.isInteracting) {
            return;
        }
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement() {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        // If we are sprinting, select the sprinting speed
        // If we are running, select the running speed;
        // If we are walking, select the walking speed
        if (isSprinting) {
            moveDirection *= sprintingSpeed;
        }
        else {
            if (inputManager.moveAmount >= 0.75f) {
                moveDirection *= runningSpeed;
            }
            else {
                moveDirection *= walkingSpeed;
            }    
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation() {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();

        targetDirection.y = 0;

        if (targetDirection == Vector3.zero) {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding() {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffset;

        if (!isGrounded) {
            if (!playerManager.isInteracting) {
                animationManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, maxDistance, groundLayer)) {
            if (!isGrounded && playerManager.isInteracting) {
                animationManager.PlayTargetAnimation("Landing", true);
            }

            inAirTimer = 0.0f;
            isGrounded = true;
            playerManager.isInteracting = false;
        }
        else {
            isGrounded = false;
        }
    }
}
