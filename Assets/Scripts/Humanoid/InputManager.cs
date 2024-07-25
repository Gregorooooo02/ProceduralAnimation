using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private AnimationManager animationManager;

    [SerializeField] Vector2 movementInput;
    [SerializeField] Vector2 cameraInput;

    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    private float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    
    private void Awake() {
        animationManager = GetComponent<AnimationManager>();
    }

    public void HandleAllInputs() {
        HandleMovementInput();
    }

    private void HandleMovementInput() {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animationManager.UpdateAnimatorValues(0, moveAmount);
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
        }

        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}
