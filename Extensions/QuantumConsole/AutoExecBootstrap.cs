using System;
using System.IO;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    /// <summary>
    /// Runs AutoExec before any other script has initialized and then destroys itself
    /// </summary>
    [DefaultExecutionOrder(-32000)]
    public class AutoExecBootstrap : MonoBehaviour
#if UNITY_EDITOR
        , IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
    {
        void Awake() {
            ConfigCommands.InvokeAutoExec();
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        public int callbackOrder => -3000; // Odin' project validator scan happens at -2000, AOT Generation happens at -1000 and we want to be before that since we are adding a script to the scene

        public void OnPreprocessBuild(BuildReport report) {
            AddBootstrap();
        }

        public void OnPostprocessBuild(BuildReport report) {
            //RemoveBootstrap();
        } 

        [Button]
        public static void AddBootstrap() => ConfigureBootstrap(AutoExecOperation.Add);

        [Button]
        public static void RemoveBootstrap() => ConfigureBootstrap(AutoExecOperation.Remove);

        [Button]
        public static void CheckBootstrap() => ConfigureBootstrap(AutoExecOperation.Check);

        public enum AutoExecOperation {
            Add,
            Remove,
            Check
        }
        
        static Regex yamlObjectRegex = new Regex("^[a-zA-Z]+?:\n.*?.*?(--- !u![0-9]+ &[0-9]+( stripped)?|\\Z)$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
        static string scriptExistsRegexTemplate = "\\s*m_Script: {{fileID: {0}, guid: {1}, type: {2}}}";
        public static void ConfigureBootstrap(AutoExecOperation operation) {
            var originalOperation = operation;
            var validSetup = true;
            var activeScene = false;
            if (EditorSceneManager.loadedSceneCount > 0) {
                for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++) {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    if (!scene.IsValid()) validSetup = false;
                    if (string.IsNullOrWhiteSpace(scene.name)) validSetup = false;
                    if (EditorSceneManager.GetActiveScene() == scene) activeScene = true;
                }
            } else {
                validSetup = false;
            }

            var sceneRestoreRequired = false;
            SceneSetup[] backup = null;

            var identifier = GetAutoExecBootstrapIdentifier();
            var scriptExistsRegex = new Regex(string.Format(scriptExistsRegexTemplate, identifier.Item2, identifier.Item1, "[0-4]"), RegexOptions.Compiled);
            try {
                int startAt = 0;
                for (int i = startAt; i < EditorSceneManager.sceneCountInBuildSettings; i++) {
                    if (!EditorBuildSettings.scenes[i].enabled) continue;
                    if (i == 0 && originalOperation != AutoExecOperation.Check) operation = AutoExecOperation.Remove; // RuntimeInitializeOnLoad handles the first scene so make sure AutoExecBootstrapper doesn't try as well
                    var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    
                    // Report progress
                    EditorUtility.DisplayProgressBar($"{operation.ToString()} autoexec bootstrap", $"{operation.ToString()} autoexec bootstrap in scene {scenePath}",
                        i / (float) EditorSceneManager.sceneCountInBuildSettings);
                    
                    // Scan the scene yaml text to see if the bootstrapper script is loaded (This skips having to load the scene in Unity which can be very s;pw)
                    var absoluteScenePath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), scenePath);
                    var sceneYaml = File.ReadAllText(absoluteScenePath);
                    var bootstrapExists = SceneContainsComponent(sceneYaml, scriptExistsRegex);

                    // Only load the scene if absolutely necessary
                    Scene scene;
                    void LoadScene() {
                        // If this is the first time we will load a new scene then we need to save the scene setup and ask the user to save the existing one
                        if (!sceneRestoreRequired) {
                            if (validSetup && activeScene) {
                                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                                backup = EditorSceneManager.GetSceneManagerSetup();
                            }
                            sceneRestoreRequired = true;
                        }
                        scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    }

                    switch (operation) {
                        case AutoExecOperation.Add when bootstrapExists:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} already contains bootstrap, skipping");
                            break;
                        case AutoExecOperation.Add:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} did not contain bootstrap, adding...");
                            LoadScene();
                            var go = new GameObject();
                            go.name = $"[{nameof(AutoExecBootstrap)}]";
                            go.AddComponent<AutoExecBootstrap>();
                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                            break;
                        case AutoExecOperation.Remove when bootstrapExists:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} contains bootstrap, removing");
                            LoadScene();
                            // Get bootstrap reference
                            var bootstrap = FindObjectOfType<AutoExecBootstrap>();
                            if (!bootstrap) {
                                Debug.LogError($"YAML search showed that {nameof(AutoExecBootstrap)} component existed in {scenePath} but {nameof(FindObjectOfType)} could not find it, skipping");
                                break;
                            }
                            bootstrap.gameObject.DestroySmart();
                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                            break;
                        case AutoExecOperation.Remove:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} did not contain bootstrap, skipping...");
                            break;
                        case AutoExecOperation.Check when bootstrapExists:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} contains bootstrap");
                            break;
                        case AutoExecOperation.Check:
                            Debug.Log($"[{nameof(AutoExecBootstrap)}] [{operation.ToString()}] {scenePath} does not contain bootstrap");
                            break;
                    }

                    operation = originalOperation;
                }
            } finally {
                EditorUtility.ClearProgressBar();
                if (sceneRestoreRequired) {
                    if (backup != null)
                        EditorSceneManager.RestoreSceneManagerSetup(backup);
                    else
                        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                }

                CollectGarbage();
            }
        }

        public static bool SceneContainsComponent(string yaml, Regex validScriptLine) {
            var objectMatch = yamlObjectRegex.Matches(yaml);
            bool exists = false;
            foreach (Match o in objectMatch)
                if (validScriptLine.Match(o.Value).Success)
                    exists = true;
            return exists;
        }

        [Button]
        public static void CollectGarbage() {
            EditorUtility.UnloadUnusedAssetsImmediate(true);
            GC.Collect();
        }

        public static (string, long) GetAutoExecBootstrapIdentifier() => GetMonoBehaviourIdentifier<AutoExecBootstrap>();

        public static (string, long) GetMonoBehaviourIdentifier<T>() where T : MonoBehaviour {
            var gameObject = new GameObject();
            try {
                gameObject.hideFlags = HideFlags.HideAndDontSave;
                var monoScript = MonoScript.FromMonoBehaviour(gameObject.AddComponent<T>());
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(monoScript, out var guid, out long localId);
                return (guid, localId);
            } finally {
                gameObject.DestroySmart();
            }
        }
#endif
    }
}