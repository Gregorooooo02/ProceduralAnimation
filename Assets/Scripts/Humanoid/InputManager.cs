using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerLocomotion playerLocomotion;
    private AnimationManager animationManager;

    [SerializeField] Vector2 movementInput;
    [SerializeField] Vector2 cameraInput;

    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool sprintInput;
    public bool jumpInput;
    
    private void Awake() {
        animationManager = GetComponent<AnimationManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    public void HandleAllInputs() {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpInput();
    }

    private void HandleMovementInput() {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animationManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput() {
        if (sprintInput && moveAmount > 0.5f) {
            playerLocomotion.isSprinting = true;
        } else {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpInput() {
        if (jumpInput) {
            jumpInput = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void OnEnable() {
        if (playerInput == null) {
            playerInput = new PlayerInput();
            
            playerInput.PlayerMovement.Movement.performed += ctx => {
                movementInput = ctx.ReadValue<Vector2>();
            };

            playerInput.PlayerMovement.Camera.performed += ctx => {
                cameraInput = ctx.ReadValue<Vector2>();
            };

            playerInput.PlayerActions.B.performed += ctx => {
                sprintInput = true;
            };

            playerInput.PlayerActions.B.canceled += ctx => {
                sprintInput = false;
            };

            playerInput.PlayerActions.Jump.performed += ctx => {
                jumpInput = true;
            };
        }

        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}
