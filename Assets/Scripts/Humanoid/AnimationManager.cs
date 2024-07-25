using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    private int horizontalHash;
    private int verticalHash;
    private int interactingHash;

    private void Awake() {
        animator = GetComponent<Animator>();

        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");

        interactingHash = Animator.StringToHash("isInteracting");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting) {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.75f) {
            snappedHorizontal = 0.7f;
        } else if (horizontalMovement > 0.75f) {
            snappedHorizontal = 1;
        } else if (horizontalMovement < 0 && horizontalMovement > -0.75f) {
            snappedHorizontal = -0.7f;
        } else if (horizontalMovement < -0.75f) {
            snappedHorizontal = -1;
        } else {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.75f) {
            snappedVertical = 0.7f;
        } else if (verticalMovement > 0.75f) {
            snappedVertical = 1;
        } else if (verticalMovement < 0 && verticalMovement > -0.75f) {
            snappedVertical = -0.7f;
        } else if (verticalMovement < -0.75f) {
            snappedVertical = -1;
        } else {
            snappedVertical = 0;
        }
        #endregion

        if (isSprinting) {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2.0f;
        }

        animator.SetFloat(horizontalHash, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalHash, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting) {
        animator.SetBool(interactingHash, isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}
