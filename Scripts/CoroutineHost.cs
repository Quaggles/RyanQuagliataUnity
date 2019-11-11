using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyanQuagliataUnity {
    public class CoroutineHost : MonoBehaviour {
        private static CoroutineHost host = null;
        public static readonly List<Coroutine> running = new List<Coroutine>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Clear() => running.Clear();

        public static int RunningRoutineCount => running.Count;

        public static Coroutine Run(IEnumerator coroutine) {
            CreateHost();
            var runningCoroutine = host.StartCoroutine(coroutine);
            host.StartCoroutine(RemoveWhenDone(runningCoroutine));
            running.Add(runningCoroutine);
            return runningCoroutine;
        }

        static void CreateHost() {
            if (host != null) return;
            host = new GameObject(nameof(CoroutineHost)).AddComponent<CoroutineHost>();
            host.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            if (Application.isPlaying) DontDestroyOnLoad(host);
        }

        static IEnumerator RemoveWhenDone(Coroutine coroutine) {
            yield return coroutine;
            running.Remove(coroutine);
        }
    }
}