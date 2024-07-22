using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{    
    [SerializeField] Transform target;
    [SerializeField] Transform pole;

    // First bone parameters
    [SerializeField] Transform firstBone;
    [SerializeField] Vector3 firstBoneEulerAngleOffset;
    
    // Second bone parameters
    [SerializeField] Transform secondBone;
    [SerializeField] Vector3 secondBoneEulerAngleOffset;

    // Third bone parameters
    [SerializeField] Transform thirdBone;
    [SerializeField] Vector3 thirdBoneEulerAngleOffset;

    [SerializeField] bool alignThirdBoneWithTargetRotation = true;

    void OnEnable() {
        if (
            firstBone == null ||
            secondBone == null ||
            thirdBone == null ||
            target == null
        ) {
            Debug.LogError("InverseKinematics: Bones are not initialized.");
            enabled = false;
            return;
        }
    }

    void LateUpdate() {
        Vector3 towardPole = pole.position - firstBone.position;
        Vector3 towardTarget = target.position - firstBone.position;

        float rootBoneLength = Vector3.Distance(firstBone.position, secondBone.position);
        float secondBoneLength = Vector3.Distance(secondBone.position, thirdBone.position);
        float totalChainLength = rootBoneLength + secondBoneLength;

        // Align root with target
        firstBone.rotation = Quaternion.LookRotation(towardTarget, towardPole);
        firstBone.localRotation *= Quaternion.Euler(firstBoneEulerAngleOffset);

        Vector3 towardSecondBone = secondBone.position - firstBone.position;

        var targetDistance = Vector3.Distance(firstBone.position, target.position);

        // Limit hypothenuse to avoid overstretching
        targetDistance = Mathf.Min(targetDistance, totalChainLength * 0.9999f);

        // Solve for the angle for the root bone
        var adjacent =
        (
            (rootBoneLength * rootBoneLength) +
            (targetDistance * targetDistance) -
            (secondBoneLength * secondBoneLength)
        ) / (2 * targetDistance * rootBoneLength);
        var angle = Mathf.Acos(adjacent) * Mathf.Rad2Deg;

        // Rotate around the orthogonal vector to both pole and second bone
        Vector3 cross = Vector3.Cross(towardPole, towardSecondBone);

        if (!float.IsNaN(angle)) {
            firstBone.RotateAround(firstBone.position, cross, -angle);
        }

        // Align second bone with target
        var secondBoneTargetRotation = Quaternion.LookRotation(target.position - secondBone.position, cross);
        secondBoneTargetRotation *= Quaternion.Euler(secondBoneEulerAngleOffset);
        secondBone.rotation = secondBoneTargetRotation;

        if (alignThirdBoneWithTargetRotation) {
            thirdBone.rotation = target.rotation;
            thirdBone.localRotation *= Quaternion.Euler(thirdBoneEulerAngleOffset);
        }
    }
}
