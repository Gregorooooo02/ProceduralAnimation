using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public bool isJumping;

    [Header("Movement Speeds")]
    [SerializeField] float walkingSpeed = 1.5f;
    [SerializeField] float runningSpeed = 5.0f;
    [SerializeField] float sprintingSpeed = 7.0f;
    [SerializeField] float rotationSpeed = 15.0f;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float gravityIntensity = -15.0f;

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
        if (isJumping) return;
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
        if (isJumping) return;
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
        Vector3 targetPosition;

        rayCastOrigin.y += rayCastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping) {
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

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0.0f;
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }

        if (isGrounded && !isJumping) {
            if (playerManager.isInteracting || inputManager.moveAmount > 0) {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
        } 
    }

    public void HandleJumping() {
        if (isGrounded) {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2.0f * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;

            playerRigidbody.velocity = playerVelocity;
        }
    }
}
