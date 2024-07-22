using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LegStepper : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;
    // Stay within this distance of home
    [SerializeField] float wantStepAtDistance;
    // How long a step takes to complete
    [SerializeField] float stepDuration;

    // Is the leg actually moving? :O
    public bool Moving { get; private set; }

    // Fraction of the max distance from home we want to overshoot by
    [SerializeField, Range(0, 1)] float stepOvershootFraction;

    void Awake() {
        // Make sure the leg is at home
        transform.SetParent(null);

        // Immediately move to home position
        TryMove();
    }

    public void TryMove() {
        if (Moving) return;

        // Calculate the distance from home
        float distanceFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far from home - just take a step
        if (distanceFromHome > wantStepAtDistance) {
            // Start the step coroutine
            StartCoroutine(MoveToHome());
        }
    }

    IEnumerator MoveToHome() {
        Moving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRotation = transform.rotation;

        Quaternion endRotation = homeTransform.rotation;
        
        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distance overshoot by
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        // Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2.0f;

        // Time since step started
        float timeElapsed = 0.0f;

        do {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / stepDuration;
            
            normalizedTime = Easing.InOutCubic(normalizedTime);

            // Let's use a quadratic Bezier curve to interpolate between the start and end points
            transform.position =
                Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime
                );

            // Let's also interpolate the rotation
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, normalizedTime);

            yield return null;
        }
        while (timeElapsed < stepDuration);

        // Moving is done
        Moving = false;
    }

    void OnDrawGizmosSelected() {
        if (Moving)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.DrawLine(transform.position, homeTransform.position);
        Gizmos.DrawWireCube(homeTransform.position, Vector3.one * 0.1f);
    }
}
