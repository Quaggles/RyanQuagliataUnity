using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity {
    public class TransformVelocity : MonoBehaviour {
        public ExponentialMovingAverageVector3 Velocity = new ExponentialMovingAverageVector3(10);
        public ExponentialMovingAverageVector3 AngularVelocity = new ExponentialMovingAverageVector3(10);
        
        [ShowInInspector]
        public float VelocityMagnitude => Velocity.Average.magnitude;
        [ShowInInspector]
        public float AngularVelocityMagnitude => AngularVelocity.Average.magnitude;
        
        private Vector3 previousPosition;
        private Vector3 previousRotation;

        private Transform cachedTransform;

        void Awake() => cachedTransform = transform;

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