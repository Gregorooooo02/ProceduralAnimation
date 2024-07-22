using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeckController : MonoBehaviour
{
    // The target, which the head will follow
    [SerializeField] Transform target;

    void Update() {
        RootMotionUpdate();
    }

    // All animation code should be in LateUpdate
    // This allows other systems to update the environment first,
    // allowing the animation system to adapt to it before the frame is rendered.
    void LateUpdate() {
        HeadTrackingUpdate();
        EyeTrackingUpdate();
    }

    void Awake() {
        StartCoroutine(LegUpdateCoroutine());
    }

    #region Motion

    // Moving parameters
    // How fast we can turn and move full throttle
    [Header("Motion")]
    [SerializeField] float turnSpeed;
    [SerializeField] float moveSpeed;
    // How fast we will reach the above speeds
    [SerializeField] float turnAcceleration;
    [SerializeField] float moveAcceleration;
    // Try to stay in this range from the target
    [SerializeField] float minDistToTarget;
    [SerializeField] float maxDistToTarget;
    // If we are above this angle from the target, start turning
    [SerializeField] float maxAngleToTarget;

    // World space velocity
    Vector3 currentVelocity;
    float currentAngularVelocity;

    private void RootMotionUpdate() {
        Vector3 towardTarget = target.position - transform.position;
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, Vector3.up);

        float angleToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, Vector3.up);

        float targetAngularVelocity = 0.0f;

        // If we hit the max angle, leave the target velocity at 0
        if (Mathf.Abs(angleToTarget) > maxAngleToTarget) {
            if (angleToTarget > 0) {
                targetAngularVelocity = turnSpeed;
            }
            else {
                targetAngularVelocity = -turnSpeed;
            }
        }

        currentAngularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            targetAngularVelocity,
            1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
        );

        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);
    }
    #endregion

    #region Head Tracking

    [Header("Head Tracking")]
    [SerializeField] Transform headBone;
    [SerializeField] float headMaxTurnAngle = 45.0f;
    [SerializeField] float headTurnSpeed = 5.0f;

    private void HeadTrackingUpdate() {
        Quaternion currentLocalRotation = headBone.localRotation;
        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(
            Vector3.forward,
            targetLocalLookDir,
            headMaxTurnAngle * Mathf.Deg2Rad,
            0
        );

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        headBone.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation,
            1 - Mathf.Exp(-headTurnSpeed * Time.deltaTime)
        );
    }
    #endregion
    
    #region Eye Tracking

    [Header("Eye Tracking")]
    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    // Eye tracking parameters
    [SerializeField] float eyeTrackingSpeed = 10.0f;
    [SerializeField] float leftEyeMaxYRotation = 10.0f;
    [SerializeField] float leftEyeMinYRotation = -180.0f;
    [SerializeField] float rightEyeMaxYRotation = 180.0f;
    [SerializeField] float rightEyeMinYRotation = -10.0f;

    private void EyeTrackingUpdate() {
        Quaternion targetEyeRotation = Quaternion.LookRotation(target.position - leftEyeBone.position, Vector3.up);

        leftEyeBone.rotation = Quaternion.Slerp(
            leftEyeBone.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );
        
        rightEyeBone.rotation = Quaternion.Slerp(
            rightEyeBone.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );

        float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

        if (leftEyeCurrentYRotation > 180.0f) {
            leftEyeCurrentYRotation -= 360.0f;
        }
        if (rightEyeCurrentYRotation > 180.0f) {
            rightEyeCurrentYRotation -= 360.0f;
        }

        // Clamp the eye rotation to the min and max values
        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            leftEyeMinYRotation,
            leftEyeMaxYRotation
        );
        
        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            rightEyeMinYRotation,
            rightEyeMaxYRotation
        );

        // Apply the clamped rotation
        leftEyeBone.localEulerAngles = new Vector3(
            leftEyeBone.localEulerAngles.x,
            leftEyeClampedYRotation,
            leftEyeBone.localEulerAngles.z
        );

        rightEyeBone.localEulerAngles = new Vector3(
            rightEyeBone.localEulerAngles.x,
            rightEyeClampedYRotation,
            rightEyeBone.localEulerAngles.z
        );
    }
    #endregion

    #region Leg Stepping

    [Header("Legs")]
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;

    IEnumerator LegUpdateCoroutine() {
        while (true) {
            do {
                frontLeftLegStepper.TryMove();
                backRightLegStepper.TryMove();

                yield return null;
            }
            while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

            do {
                frontRightLegStepper.TryMove();
                backLeftLegStepper.TryMove();

                yield return null;
            }
            while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }
    #endregion
}
