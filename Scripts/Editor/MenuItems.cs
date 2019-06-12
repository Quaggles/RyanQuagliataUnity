﻿#if UNITY_EDITOR
using System;
using System.IO;
using RyanQuagliata.Extensions;
using RyanQuagliata.Extensions.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RyanQuagliata.Scripts.Editor {
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
	}
}
#endif