using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;

    [SerializeField] float acceleration = 2.0f;
    [SerializeField] float deceleration = 2.0f;

    [SerializeField] float maxWalkVelocity = .5f;
    [SerializeField] float maxRunVelocity = 2.0f;

    // Increase performance
    private int velocityXHash;
    private int velocityZHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        velocityXHash = Animator.StringToHash("Velocity X");
        velocityZHash = Animator.StringToHash("Velocity Z");
    }

    /// <summary>
    /// Handle acceleration and deceleration of the player
    /// </summary>
    /// <param name="forwardPressed"> Input to move forward </param>
    /// <param name="leftPressed"> Input to move left </param>
    /// <param name="rightPressed"> Input to move right </param>
    /// <param name="runPressed"> Input for running </param>
    /// <param name="currentMaxVelocity"> Current velocity, changes if the run is pressed </param>
    void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity) {
        if (forwardPressed && velocityZ < currentMaxVelocity) {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (leftPressed && velocityX > -currentMaxVelocity) {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed && velocityX < currentMaxVelocity) {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!forwardPressed && velocityZ > 0.0f) {
            velocityZ -= Time.deltaTime * deceleration;
        }

        if (!leftPressed && velocityX < 0.0f) {
            velocityX += Time.deltaTime * deceleration;
        }

        if (!rightPressed && velocityX > 0.0f) {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void LockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity) {
        if (!forwardPressed && velocityZ < 0.0f) {
            velocityZ = 0.0f;
        }

        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f)) {
            velocityX = 0.0f;
        }

        // Lock forward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity) {
            velocityZ = currentMaxVelocity;
        } else if (forwardPressed && velocityZ > currentMaxVelocity) {
            velocityZ -= Time.deltaTime * deceleration;

            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f)) {
                velocityZ = currentMaxVelocity;
            
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f)) {
            velocityZ = currentMaxVelocity;
        }

        // Lock left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity) {
            velocityX = -currentMaxVelocity;
        } else if (leftPressed && velocityX < -currentMaxVelocity) {
            velocityX += Time.deltaTime * deceleration;

            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f)) {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f)) {
            velocityX = -currentMaxVelocity;
        }

        // Lock right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity) {
            velocityX = currentMaxVelocity;
        } else if (rightPressed && velocityX > currentMaxVelocity) {
            velocityX -= Time.deltaTime * deceleration;

            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f)) {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f)) {
            velocityX = currentMaxVelocity;
        }
    }

    void Update()
    {
        // Get key input from player
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // Set current maxVelocity
        float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

        // Change velocity
        ChangeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        // Lock or reset velocity
        LockOrResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
    }
}
