using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform cameraPivot;

    [SerializeField] Transform pill;
    [SerializeField] float pillSpeed = 1.0f;

    [SerializeField] Transform lookTarget;

    [SerializeField] float keyboardSensitivity = 80.0f;

    Plane xzPlane = new Plane(Vector3.up, Vector3.zero);
    Vector3 pillTargetPosition;
    bool raiseLookTarget;

    Vector3 camTargetAngle;
    SmoothDamp.EulerAngles camAngle;
    float camTargetDistance;
    SmoothDamp.Float camDistance;

    void Awake() {
        camTargetAngle = cameraPivot.localEulerAngles;
        camTargetDistance = playerCamera.localPosition.z;
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (xzPlane.Raycast(ray, out float hitDistance)) {
                pillTargetPosition = ray.origin + ray.direction * hitDistance;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            raiseLookTarget = !raiseLookTarget;
        }

        pill.position = Vector3.Lerp(pill.position, pillTargetPosition, 1 - Mathf.Exp(-pillSpeed * Time.deltaTime));

        lookTarget.localPosition = Vector3.Lerp(
            lookTarget.localPosition,
            new Vector3(0, raiseLookTarget ? 3.0f : 1, 0),
            1 - Mathf.Exp(-pillSpeed * Time.deltaTime)
        );

        camTargetAngle += new Vector3(
            -Input.GetAxisRaw("Vertical") * keyboardSensitivity * Time.deltaTime,
            -Input.GetAxisRaw("Horizontal") * keyboardSensitivity * Time.deltaTime,
            0
        );

        camTargetAngle.x = Mathf.Clamp((camTargetAngle.x + 180) % 360 - 180, -80, 80);

        camAngle.Step(camTargetAngle, 8.0f);
        cameraPivot.localEulerAngles = camAngle;

        camTargetDistance -= Input.mouseScrollDelta.y;
        camDistance.Step(camTargetDistance, 8.0f);

        playerCamera.localPosition = Vector3.forward * camDistance;
    }
}
