#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using System;
using System.Collections.Generic;
using QFSW.QC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Extensions.OdinInspector {
    [GlobalConfig(ASSET_FOLDER)]
    [CommandPrefix]
    public class StandaloneAssetDatabase : SerializedGlobalConfig<StandaloneAssetDatabase>
#if UNITY_EDITOR
        , IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
    {
        [ShowInInspector]
        public Dictionary<string, List<GuidScriptableObject>> ScriptableObjectsFullyQualified = new Dictionary<string, List<GuidScriptableObject>>();

        [ShowInInspector]
        public Dictionary<Type, List<GuidScriptableObject>> ScriptableObjectsTyped = new Dictionary<Type, List<GuidScriptableObject>>();

        [Command, Button]
        public static void Print() {
            foreach (var keyValuePair in Instance.ScriptableObjectsTyped) {
                Debug.Log($"[{keyValuePair.Key}] {keyValuePair.Value.Count} object(s)");
                foreach (var guidScriptableObject in keyValuePair.Value) {
                    Debug.Log($"    [{keyValuePair.Key.FullName}] {guidScriptableObject.name}");
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize() {
            // If in the editor play mode then store the objects and write GUIDs for usage 
#if UNITY_EDITOR
            StoreObjects();
            // If exiting play mode clear
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            // Resolve type strings at runtime
            foreach (var keyValuePair in Instance.ScriptableObjectsFullyQualified) {
                var type = Type.GetType(keyValuePair.Key);
                if (type == null) {
                    Debug.LogError($"Type \"{keyValuePair.Key}\" could not be resolved, does it exist? Has it been code stripped?");
                    continue;
                }
                Instance.ScriptableObjectsTyped.Add(type, keyValuePair.Value);
            }
        }

#if UNITY_EDITOR
        public int callbackOrder { get; }
        
        /// <summary>
        /// Before build store the types as strings and store the scriptable objects
        /// </summary>
        /// <param name="report"></param>
        public void OnPreprocessBuild(BuildReport report) => StoreObjects();

        /// <summary>
        /// After build clear everything
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report) => ClearObjects();

        /// <summary>
        /// Clear on playmode exit
        /// </summary>
        /// <param name="obj"></param>
        private static void OnPlayModeStateChanged(PlayModeStateChange obj) {
            if (obj == PlayModeStateChange.ExitingPlayMode) {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                ClearObjects();
            }
        }

        private static void ClearObjects() => StoreObjects(true);

        public static string GetGuid(Object obj) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));

        [Button]
        private static void StoreObjects(bool clear = false) {
            if (Instance.ScriptableObjectsFullyQualified == null)
                Instance.ScriptableObjectsFullyQualified = new Dictionary<string, List<GuidScriptableObject>>();
            var objects = GetAllInstances<GuidScriptableObject>();
            // Clear the SO database
            if (clear) {
                Instance.ScriptableObjectsFullyQualified.Clear();
                Instance.ScriptableObjectsTyped.Clear();
            }
            for (var i = 0; i < objects.Length; i++) {
                var scriptableObject = objects[i];
                // Set GUID SO to null
                if (clear) {
                    scriptableObject.Guid = String.Empty;
                    continue;
                } else {
                    // Populate SO databse
                    var type = scriptableObject.GetType();
                    List<GuidScriptableObject> list;
                    if (!Instance.ScriptableObjectsFullyQualified.TryGetValue(type.AssemblyQualifiedName, out list))
                        Instance.ScriptableObjectsFullyQualified.Add(type.AssemblyQualifiedName, new List<GuidScriptableObject>());

                    if (list == null) Instance.ScriptableObjectsFullyQualified[type.AssemblyQualifiedName] = list = new List<GuidScriptableObject>();
                    list.Add(scriptableObject);
                }

                // Set GUID of SO
                var guid = GetGuid(scriptableObject);
                scriptableObject.Guid = guid;
            }
        }

        public static T[] GetAllInstances<T>() where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) {
                a[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            return a;
        }
#endif
    }
}