using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyanQuagliataUnity {
    [DisallowMultipleComponent]
    public class TransformHasChangedResetter : MonoBehaviour {
        public static YieldInstruction WaitYield = null;
        private static bool routineStarted = false;
        public static List<TransformHasChangedResetter> All = new List<TransformHasChangedResetter>();

        private void OnEnable() => All.Add(this);
        private void OnDisable() => All.Remove(this);

        void Start() {
            if (!routineStarted) {
                CoroutineHost.Run(EndOfFrameReset());
                routineStarted = true;
            }
        }

        /// <summary>
        /// This coroutine will always be looping over every object and clearing the has
        /// </summary>
        /// <returns></returns>
        static IEnumerator EndOfFrameReset() {
            while (Application.isPlaying) {
                yield return WaitYield;
                foreach (var transformHasChangedResetter in All) transformHasChangedResetter.transform.hasChanged = false;
            }
        }
    }
}