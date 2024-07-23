using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    float velocity = 0.0f;

    [SerializeField] float acceleration = 0.1f;
    [SerializeField] float deceleration = 0.5f;

    int velocityHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        if (forwardPressed && velocity < 1.0f) {
            velocity += Time.deltaTime * acceleration;
        }
        else if (!forwardPressed && velocity > 0.0f) {
            velocity -= Time.deltaTime * deceleration;
        }

        if (!forwardPressed && velocity < 0.0f) {
            velocity = 0.0f;
        }

        animator.SetFloat(velocityHash, velocity);
    }
}
