using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity {
    // ReSharper disable once InconsistentNaming
    public class OnGuiMonoBehaviour : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        public Queue<Action> OnGuiActions = new Queue<Action>();
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            while (OnGuiActions.Count > 0) {
                var i = OnGuiActions.Dequeue();
                i.Invoke();
            }
        }
#endif
    }

    // ReSharper disable once InconsistentNaming
    public static class OnGuiAnywhere {
        private static OnGuiMonoBehaviour onGuiHost;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize() {
            GetHandlesHost();
        }
#endif

        static OnGuiMonoBehaviour GetHandlesHost() {
            if (!onGuiHost) {
                onGuiHost = new GameObject().AddComponent<OnGuiMonoBehaviour>();
                onGuiHost.gameObject.name = $"[{nameof(onGuiHost)}]";
                Object.DontDestroyOnLoad(onGuiHost);
            }

            return onGuiHost;
        }

        public static void AddGeneric(Action action) => GetHandlesHost().OnGuiActions.Enqueue(action);

        public static class Handles {
            public static void Label(Vector3 position, string label) {
#if UNITY_EDITOR
                void Action() => UnityEditor.Handles.Label(position, label);
                AddGeneric(Action);
#endif
            }
        }

        public static class Gizmos {
            public static void DrawLine(Vector3 from, Vector3 to) {
#if UNITY_EDITOR
                void Action() => UnityEngine.Gizmos.DrawLine(from, to);
                AddGeneric(Action);
#endif
            }
        }
    }
}