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

    [Serializable]
    public struct Vector3 {
        public float x { get { return currentValue.x; } }
        public float y { get { return currentValue.y; } }
        public float z { get { return currentValue.z; } }

        public UnityEngine.Vector3 currentValue;
        public UnityEngine.Vector3 pastTarget;

        public void Reset(UnityEngine.Vector3 newValue) {
            currentValue = newValue;
            pastTarget = newValue;
        }

        public UnityEngine.Vector3 Step(UnityEngine.Vector3 target, float speed) {
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

        public static implicit operator UnityEngine.Vector3(Vector3 rhs) {
            return rhs.currentValue;
        }
    }

    [Serializable]
    public struct EulerAngles {
        public UnityEngine.Vector3 currentValue;
        private UnityEngine.Vector3 pastTarget;

        public EulerAngles(EulerAngles copy) {
            this.currentValue = copy.currentValue;
            this.pastTarget = copy.pastTarget;
        }

        public void Reset(UnityEngine.Vector3 newValue) {
            currentValue = newValue;
            pastTarget = newValue;
        }

        public UnityEngine.Vector3 Step(UnityEngine.Vector3 target, float speed) {
            target.x = currentValue.x + Mathf.DeltaAngle(currentValue.x, target.x);
            target.y = currentValue.y + Mathf.DeltaAngle(currentValue.y, target.y);
            target.z = currentValue.z + Mathf.DeltaAngle(currentValue.z, target.z);

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

        public static implicit operator UnityEngine.Vector3(EulerAngles rhs) {
            return rhs.currentValue;
        }
    }
}
