using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{    
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;

    private int isWalkingHash;
    private int isRunningHash;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;

    private float rotationFactorPerFrame = 15.0f;
    private float runMultiplier = 2.0f;

    void Awake() {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        playerInput.CharacterControls.Movement.started += OnMovementInput;
        playerInput.CharacterControls.Movement.performed += OnMovementInput;
        playerInput.CharacterControls.Movement.canceled += OnMovementInput;

        playerInput.CharacterControls.Run.started += OnRunInput;
        playerInput.CharacterControls.Run.performed += OnRunInput;
        playerInput.CharacterControls.Run.canceled += OnRunInput;
    }

    void OnMovementInput(InputAction.CallbackContext context) {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x / 2.5f;
        currentMovement.z = currentMovementInput.y / 2.5f;

        currentRunMovement.x = currentMovementInput.x;
        currentRunMovement.z = currentMovementInput.y;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnRunInput(InputAction.CallbackContext context) {
        isRunPressed = context.ReadValueAsButton();
        animator.SetBool(isRunningHash, isRunPressed);
    }

    void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (isRunPressed) {
            characterController.Move(currentRunMovement * Time.deltaTime);
        } else {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    void HandleAnimation() {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking) {
            animator.SetBool(isWalkingHash, true);
        } else if (!isMovementPressed && isWalking) {
            animator.SetBool(isWalkingHash, false);
        }

        if (isRunPressed && !isRunning) {
            animator.SetBool(isRunningHash, true);
        } else if (!isRunPressed && isRunning) {
            animator.SetBool(isRunningHash, false);
        }
    }

    void HandleRotation() {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed) {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);    
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnEnable() {
        playerInput.CharacterControls.Enable();
    }
}
