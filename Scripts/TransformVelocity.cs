using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity {
    public class TransformVelocity : MonoBehaviour {
        [Min(0)]
        public int InitialVelocityLoopback = 1;
        [Min(0)]
        public int InitialAngularVelocityLoopback = 1;

        public ExponentialMovingAverageVector3 Velocity;
        public ExponentialMovingAverageVector3 AngularVelocity;
        
        [ShowInInspector]
        public float VelocityMagnitude => Velocity.Average.magnitude;
        [ShowInInspector]
        public float AngularVelocityMagnitude => AngularVelocity.Average.magnitude;
        
        private Vector3 previousPosition;
        private Vector3 previousRotation;

        private Transform cachedTransform;

        void Awake() {
            cachedTransform = transform;
            previousPosition = transform.position;
            Velocity = new ExponentialMovingAverageVector3(InitialVelocityLoopback);
            AngularVelocity = new ExponentialMovingAverageVector3(InitialAngularVelocityLoopback);
        }

        // Update is called once per frame
        void LateUpdate() {
            var curPosition = cachedTransform.position;
            var positionDelta = (curPosition - previousPosition) / Time.deltaTime;
            
            var curRotation = cachedTransform.rotation;
            var rotationDelta = (curRotation.eulerAngles - previousRotation) / Time.deltaTime;
            
            Velocity.AddDataPoint(positionDelta);
            AngularVelocity.AddDataPoint(rotationDelta);
            
            previousPosition = curPosition;
            previousRotation = curRotation.eulerAngles;
        }
    }
}