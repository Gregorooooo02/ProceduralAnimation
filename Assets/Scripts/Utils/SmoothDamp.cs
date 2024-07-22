using System;
using UnityEngine;

public static class SmoothDamp
{
    public const float MaxSpeed = 1000000.0f;

    [Serializable]
    public struct Float {
        public float currentValue;
        private float pastTarget;

        public void Reset(float newValue) {
            currentValue = newValue;
            pastTarget = newValue;
        }

        public float Step(float target, float speed) {
            var deltaTime = Time.deltaTime;

            var t = deltaTime * speed;

            if (0 == t) return currentValue;
            else if (t < MaxSpeed) {
                var v = (target - pastTarget) / t;
                var f = currentValue - pastTarget + v;

                pastTarget = target;

                return currentValue = target - v + f * Mathf.Exp(-t);
            }
            else {
                return currentValue = target;
            }
        }

        public static implicit operator float(Float rhs) {
            return rhs.currentValue;
        }
    }

    
}
