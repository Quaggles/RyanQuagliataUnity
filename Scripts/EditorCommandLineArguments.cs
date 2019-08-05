using UnityEngine;

namespace RyanQuagliataUnity {
    public class EditorCommandLineArguments : MonoBehaviour {
        public bool Enabled;
        public string[] EditorArguments;
        void Awake() {
#if UNITY_EDITOR
	        if (!Enabled) return;
            CommandLineArguments.AddArguments(EditorArguments);
            Debug.Log($"Adding {string.Join("|", EditorArguments)}");
#endif
        }
    }
}