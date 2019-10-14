#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RyanQuagliataUnity.Extensions.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Editor {
	public static class MenuItems {
		/// <summary>
		/// Forces a recompile by adding a temporary new symbol to the PlayerSettings and then removing it
		/// </summary>
		[MenuItem("RyanQuagliata/Force Recompile _F6", priority = 0)]
		public static void ForceRecompile() {
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			var newSymbols = "RECOMPILE;" + symbols;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newSymbols);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, symbols);
		}

		/// <summary>
		/// Forces a recompile by adding a temporary new symbol to the PlayerSettings and then removing it
		/// </summary>
		[MenuItem("RyanQuagliata/Force Update Canvases _F7", priority = 0)]
		public static void ForceUpdateCanvases() => Canvas.ForceUpdateCanvases();

		/// <summary>
		/// Forces a recompile by adding a temporary new symbol to the PlayerSettings and then removing it
		/// </summary>
		[MenuItem("RyanQuagliata/Set All UI Dirty _F8", priority = 0)]
		public static void ForceSetAllUIDirty() {
			foreach (var graphic in Object.FindObjectsOfType<Graphic>()) graphic.SetAllDirty();
		}

#if !UNITY_2018_3_OR_NEWER
        [MenuItem("RyanQuagliata/Open Player Settings &p", priority = 0)]
        public static void OpenPlayerSettings() {
		    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
        }

        [MenuItem("RyanQuagliata/Open Build Settings &b", priority = 0)]
        public static void OpenBuildSettings() =>
            EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));

        [MenuItem("RyanQuagliata/Open Time Settings &t", priority = 0)]
        public static void OpenTimeSettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings/Time");

        [MenuItem("RyanQuagliata/Open Quality Settings &q", priority = 0)]
        public static void OpenQualitySettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
#else
		[MenuItem("RyanQuagliata/Open Project Settings &p", priority = 0)]
		public static void OpenProjectSettings() => EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
#endif

		/// <summary>
		/// Saves the scene and project settings
		/// </summary>
		[MenuItem("RyanQuagliata/Save All %&s", priority = 0)]
		public static void SaveAll() {
#if UNITY_2018_3_OR_NEWER
			EditorApplication.ExecuteMenuItem("File/Save");
#else
            EditorApplication.ExecuteMenuItem("File/Save Scene");
#endif
			EditorApplication.ExecuteMenuItem("File/Save Project");
		}

		static SerializedObject source;

		/// <summary>
		/// http://answers.unity.com/answers/1538287/view.html
		///
		/// Copies properties from one object to another even if they are of different types
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Copy Serialized")]
		public static void CopySerializedFromBase(MenuCommand command) {
			source = new SerializedObject(command.context);
		}

		/// <summary>
		/// http://answers.unity.com/answers/1538287/view.html
		///
		/// Copies properties from one object to another even if they are of different types
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Paste Serialized")]
		public static void PasteSerializedFromBase(MenuCommand command) {
			if (source == null) {
				Debug.LogError("No serialized data stored on the clipboard, copy first");
				return;
			}
			
			EditorUtilityExtensions.CopySerializedPolymorphic(source, new SerializedObject(command.context));
		}

		/// <summary>
		/// Returns false if there are no properties that would match this command.context for copying
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/Component/Paste Serialized", true)]
		public static bool PasteSerializedFromBaseValid(MenuCommand command) {
			var dest = new SerializedObject(command.context);
			return EditorUtilityExtensions.MatchingPropertyCount(source, dest) > 0;
		}

		[MenuItem("RyanQuagliata/Open Package Manifest", priority = 0)]
		public static void OpenPackageManifest() => Application.OpenURL(Directory.GetParent(Application.dataPath).FullName + "\\Packages\\manifest.json");
		
		[MenuItem("RyanQuagliata/Remove Package Locks", priority = 0)]
		public static void RemovePackageLocks() {
			if (!EditorUtility.DisplayDialog("Confirm", "Are you sure you want to remove all package locks? Any packages using git will be upgraded to the latest commit", "Yes", "No"))
				return;
			try {
				var path = Directory.GetParent(Application.dataPath).FullName + "\\Packages\\manifest.json";
				var regex = new Regex(" *\"dependencies\": {.*?}", RegexOptions.Singleline);
				var match = regex.Match(File.ReadAllText(path));
				if (match.Success) {
					var output = string.Concat("{\n", match.Value, "\n}");
					File.WriteAllText(path, output);
					AssetDatabase.Refresh();
				} else {
					throw new Exception($"Regex could not find the dependency array in {path}");
				}
			} catch (Exception ex) {
				EditorUtility.DisplayDialog("Error", ex.Message, "ok");
			}
		}

		[MenuItem("RyanQuagliata/Clear Missing Build Scenes")]
		public static void ClearMissingBuildScenes() {
			var oldCount = EditorBuildSettings.scenes.Length;
			var valid = EditorBuildSettings.scenes.Where(x => !x.guid.Empty());
			if (!EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to remove {oldCount - valid.Count()} missing scenes from the build list?", "Yes", "No"))
				return;
			EditorBuildSettings.scenes = valid.ToArray();
		}
		
        [MenuItem("RyanQuagliata/Restart")]
        public static void Restart() {
	        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorApplication.ExecuteMenuItem("File/Save Project");
	        EditorApplication.OpenProject(Environment.CurrentDirectory);
        }
        
        [MenuItem("RyanQuagliata/Restart With Empty Scene")]
        public static void RestartWithEmptyScene() {
	        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorApplication.ExecuteMenuItem("File/Save Project");
	        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
	        EditorApplication.OpenProject(Environment.CurrentDirectory);
        }
	}
	
}
#endif