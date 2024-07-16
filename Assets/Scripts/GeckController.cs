using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckController : MonoBehaviour
{
    // The target, which the head will follow
    [SerializeField] Transform target;

    // A reference to the gecko's head bone
    [SerializeField] Transform headBone;

    // All animation code should be in LateUpdate
    // This allows other systems to update the environment first,
    // allowing the animation system to adapt to it before the frame is rendered.
    void LateUpdate() {
        Vector3 towardsTarget = target.position - headBone.position;
        headBone.rotation = Quaternion.LookRotation(towardsTarget, Vector3.up);
    }
}
