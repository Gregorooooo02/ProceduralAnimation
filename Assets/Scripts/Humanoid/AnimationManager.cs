using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    private int horizontalHash;
    private int verticalHash;

    private void Awake() {
        animator = GetComponent<Animator>();

        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement) {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f) {
            snappedHorizontal = 0.5f;
        } else if (horizontalMovement > 0.55f) {
            snappedHorizontal = 1;
        } else if (horizontalMovement < 0 && horizontalMovement > -0.55f) {
            snappedHorizontal = -0.5f;
        } else if (horizontalMovement < -0.55f) {
            snappedHorizontal = -1;
        } else {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f) {
            snappedVertical = 0.5f;
        } else if (verticalMovement > 0.55f) {
            snappedVertical = 1;
        } else if (verticalMovement < 0 && verticalMovement > -0.55f) {
            snappedVertical = -0.5f;
        } else if (verticalMovement < -0.55f) {
            snappedVertical = -1;
        } else {
            snappedVertical = 0;
        }
#endregion

        animator.SetFloat(horizontalHash, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalHash, snappedVertical, 0.1f, Time.deltaTime);
    }
}
