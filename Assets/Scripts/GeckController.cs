using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeckController : MonoBehaviour
{
    // The target, which the head will follow
    [SerializeField] Transform target;
    
    [SerializeField] Transform headBone;
    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    // Head tracking parameters
    [SerializeField] float headMaxTurnAngle = 45.0f;
    [SerializeField] float headTurnSpeed = 5.0f;

    // Eye tracking parameters
    [SerializeField] float eyeTrackingSpeed = 10.0f;
    [SerializeField] float leftEyeMaxYRotation = 45.0f;
    [SerializeField] float leftEyeMinYRotation = -45.0f;
    [SerializeField] float rightEyeMaxYRotation = 45.0f;
    [SerializeField] float rightEyeMinYRotation = -45.0f;

    // All animation code should be in LateUpdate
    // This allows other systems to update the environment first,
    // allowing the animation system to adapt to it before the frame is rendered.
    void LateUpdate() {
        HeadTrackingUpdate();
        EyeTrackingUpdate();
    }

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

    
    private void EyeTrackingUpdate() {

    }
}
