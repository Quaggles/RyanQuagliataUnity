using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RyanQuagliataUnity {
    [HideReferenceObjectPicker]
    public abstract class ExponentialMovingAverageBase<T> where T : struct {
        private bool isInitialized;
        private float weightingMultiplier;
        private T previousAverage;

        [ShowInInspector, ReadOnly]
        public T Average { get; private set; }

        private int loopback;
        [ShowInInspector]
        [MinValue(0)]
        public int Loopback {
            get => loopback;
            set {
                weightingMultiplier = WeightingMultiplierCalculation(value);
                loopback = value;
            }
        }

        protected ExponentialMovingAverageBase(int loopback) {
            Loopback = loopback;
        }

        public override string ToString() {
            return $"[{typeof(T)}] Loopback: {loopback} Average: {Average.ToString()}";
        }

        public void AddDataPoint(T dataPoint) {
            if (!isInitialized) {
                Average = dataPoint;
                previousAverage = Average;
                isInitialized = true;
                return;
            }

            Average = AverageCalculation(dataPoint, previousAverage, weightingMultiplier);
            
            //update previous average
            previousAverage = Average;
        }

        /// <summary>
        /// Concrete implementation of average calculation since I can't type constrain generics based on mathematical operator support (+-*/)
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <param name="previousAverage"></param>
        /// <param name="weightingMultiplier"></param>
        /// <returns></returns>
        public abstract T AverageCalculation(T dataPoint, T previousAverage, float weightingMultiplier);

        /// <summary>
        /// Override this for your own custom formula
        /// </summary>
        /// <param name="loopback"></param>
        /// <returns></returns>
        public virtual float WeightingMultiplierCalculation(int loopback) => 2.0f / (loopback + 1);
        
        public static implicit operator T(ExponentialMovingAverageBase<T> that) => that.Average;
    }

[Serializable]
    public class ExponentialMovingAverageVector2 : ExponentialMovingAverageBase<Vector2> {
        public override Vector2 AverageCalculation(Vector2 dataPoint, Vector2 previousAverage, float weightingMultiplier) => (dataPoint - previousAverage) * weightingMultiplier + previousAverage;

        public ExponentialMovingAverageVector2(int loopback) : base(loopback) { }
    }
    
    [Serializable]
    public class ExponentialMovingAverageVector3 : ExponentialMovingAverageBase<Vector3> {
        public override Vector3 AverageCalculation(Vector3 dataPoint, Vector3 previousAverage, float weightingMultiplier) => (dataPoint - previousAverage) * weightingMultiplier + previousAverage;

        public ExponentialMovingAverageVector3(int loopback) : base(loopback) { }
    }
    
    [Serializable]
    public class ExponentialMovingAverageFloat : ExponentialMovingAverageBase<float> {
        public override float AverageCalculation(float dataPoint, float previousAverage, float weightingMultiplier) => (dataPoint - previousAverage) * weightingMultiplier + previousAverage;

        public ExponentialMovingAverageFloat(int loopback) : base(loopback) { }
    }
    
    [Serializable]
    public class ExponentialMovingAverageDouble : ExponentialMovingAverageBase<double> {
        public override double AverageCalculation(double dataPoint, double previousAverage, float weightingMultiplier) => (dataPoint - previousAverage) * weightingMultiplier + previousAverage;

        public ExponentialMovingAverageDouble(int loopback) : base(loopback) { }
    }
}