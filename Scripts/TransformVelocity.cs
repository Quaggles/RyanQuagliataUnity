using RyanQuagliataUnity.UnityEvents;
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

        public bool Events = false;
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public bool EventsSendLocalVelocity = true;
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public SingleEvent XVelocityEvent;         
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public SingleEvent YVelocityEvent;         
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public SingleEvent ZVelocityEvent;         
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public Vector3Event VelocityEvent;         
        [FoldoutGroup("Events"), ShowIf(nameof(Events))]
        public SingleEvent VelocityMagnitudeEvent; 
        
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
            var positionDelta = (curPosition - previousPosition) / Time.unscaledDeltaTime;
            
            var curRotation = cachedTransform.rotation;
            var rotationDelta = (curRotation.eulerAngles - previousRotation) / Time.unscaledDeltaTime;
            
            Velocity.AddDataPoint(positionDelta);
            AngularVelocity.AddDataPoint(rotationDelta);
            
            previousPosition = curPosition;
            previousRotation = curRotation.eulerAngles;

            if (Events) {
                var eventVelocity = EventsSendLocalVelocity ? transform.TransformVector(Velocity) : Velocity;
                XVelocityEvent?.Invoke(eventVelocity.x);
                YVelocityEvent?.Invoke(eventVelocity.y);
                ZVelocityEvent?.Invoke(eventVelocity.z);
                VelocityEvent?.Invoke(eventVelocity);
                VelocityMagnitudeEvent?.Invoke(eventVelocity.magnitude);
            }
        }
    }
}