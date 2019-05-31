using System;
using System.IO;
using System.Text.RegularExpressions;
using RyanQuagliata.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace RyanQuagliata.Attributes {
	[System.Serializable]
	public class SceneField {
		[SerializeField] private Object m_SceneAsset;

		[SerializeField] private string m_SceneName = "";
		[SerializeField] private string m_ScenePath = "";
		public string ScenePath => m_ScenePath;
		public string SceneName => m_SceneName;

		// makes it work with the existing Unity methods (LoadLevel/LoadScene)
		public static implicit operator string(SceneField sceneField) {
			return sceneField.SceneName;
		}

		public bool IsNull => string.IsNullOrWhiteSpace(m_ScenePath);
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SceneField))]
	public class SceneFieldPropertyDrawer : PropertyDrawer {
		private enum ButtonState {
			None,
			Warning,
			Correct
		}

		public override void OnGUI(Rect rect, SerializedProperty _property, GUIContent _label) {
			EditorGUI.BeginProperty(rect, GUIContent.none, _property);
			SerializedProperty sceneAsset = _property.FindPropertyRelative("m_SceneAsset");
			SerializedProperty scenePath = _property.FindPropertyRelative("m_ScenePath");
			SerializedProperty sceneName = _property.FindPropertyRelative("m_SceneName");
			SceneAsset sceneAssetRef = sceneAsset.objectReferenceValue as SceneAsset;
			
			// Add to build settings button
			DrawBuildSettingsButton(ref rect, sceneAssetRef);
			
			// Add Lightmap settings warning button
			DrawLightmapSettingsButton(ref rect, sceneAssetRef);

			// Prefix label
			rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), _label);

			// Object field
			if (sceneAsset != null) {
				EditorGUI.BeginChangeCheck();
				var value = EditorGUI.ObjectField(rect, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if (EditorGUI.EndChangeCheck()) {
					sceneAsset.objectReferenceValue = value;
					scenePath.stringValue = sceneAsset.objectReferenceValue != null ? AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue) : "";
					sceneName.stringValue = sceneAsset.objectReferenceValue != null ? sceneAsset.objectReferenceValue.name : "";
				}
			}
			EditorGUI.EndProperty();
		}

		private void DrawBuildSettingsButton(ref Rect rect, SceneAsset sceneAsset) {
			bool sceneInBuildList = SceneManagerEditorExtensions.IsSceneInBuildList(sceneAsset);
			ButtonState buttonState = ButtonState.None;
			if (sceneAsset != null)
				buttonState = sceneInBuildList ? ButtonState.Correct : ButtonState.Warning;
			
			var buttonStyle = new GUIStyle("Button");
			buttonStyle.normal.textColor = GetButtonColor(buttonState);
			var buttonContent = new GUIContent(GetButtonLabel(buttonState), GetBuildSettingsButtonTooltip(buttonState));

			// Disable button if there is no scene asset loaded
			bool preGUIState = GUI.enabled;
			if (sceneAsset == null) GUI.enabled = false;

			float buttonWidth = 22;
			float buttonPadding = 5;
			if (GUI.Button(new Rect(rect.x, rect.y, buttonWidth, rect.height), buttonContent, buttonStyle)) {
				if (sceneInBuildList)
					SceneManagerEditorExtensions.RemoveSceneFromBuildSettings(sceneAsset);
				else
					SceneManagerEditorExtensions.AddSceneToBuildSettings(sceneAsset);
			}
			rect.position = new Vector2(rect.x += buttonWidth + buttonPadding, rect.y);
			rect.width -= buttonWidth + buttonPadding;

			// Restore the gui enabled state to what it was before this control
			GUI.enabled = preGUIState;
		}

		private void DrawLightmapSettingsButton(ref Rect rect, SceneAsset sceneAsset) {
			// Retrieve the Gi lighting workglow from the scene file (To avoid switching scenes to query this value)
			var giLightingWorkflow = GetSceneLightingGIWorkflowFromFile(AssetDatabase.GetAssetPath(sceneAsset));
			
			// Sets the correct button state
			bool lightmappingModeIncorrect = giLightingWorkflow != Lightmapping.GIWorkflowMode.OnDemand;
			ButtonState buttonState = ButtonState.None;
			if (sceneAsset != null)
				buttonState = lightmappingModeIncorrect ? ButtonState.Warning : ButtonState.Correct;
			
			// Sets the button style according to the state
			var buttonStyle = new GUIStyle("Button");
			buttonStyle.normal.textColor = GetButtonColor(buttonState);
			var buttonContent = new GUIContent(GetButtonLabel(buttonState), GetLightmapSettingsButtonTooltip(buttonState));
			
			// Disable the button if no scene asset is loaded
			bool preGUIState = GUI.enabled;
			if (giLightingWorkflow == Lightmapping.GIWorkflowMode.Legacy) GUI.enabled = false;
			
			float buttonWidth = 22;
			float buttonPadding = 5;
			
			if (GUI.Button(new Rect(rect.x, rect.y, buttonWidth, rect.height), buttonContent, buttonStyle)) {
				if (lightmappingModeIncorrect) {
					ChangeSceneLightingGIWorkflow(sceneAsset, Lightmapping.GIWorkflowMode.OnDemand);
				} else {
					ChangeSceneLightingGIWorkflow(sceneAsset, Lightmapping.GIWorkflowMode.Iterative);
				}
			}
			rect.position = new Vector2(rect.x += buttonWidth + buttonPadding, rect.y);
			rect.width -= buttonWidth + buttonPadding;
			
			// Restore the gui enabled state to what it was before this control
			GUI.enabled = preGUIState;
		}

		private Lightmapping.GIWorkflowMode GetSceneLightingGIWorkflowFromFile(string path) {
			if (string.IsNullOrWhiteSpace(path)) return Lightmapping.GIWorkflowMode.Legacy;
			using (FileStream fs = new FileStream(path, FileMode.Open)) {
				using (StreamReader sr = new StreamReader(fs)) {
					var regex = new Regex("m_GIWorkflowMode: ([0-9])");
					string line;
					while ((line = sr.ReadLine()) != null) {
						if (!line.Contains("m_GIWorkflowMode:")) continue;
						var match = regex.Match(line);
						if (!match.Success) continue;
						int workflowIndex = Int32.Parse(match.Groups[1].Value);
						return (Lightmapping.GIWorkflowMode)workflowIndex;
					}
				}
			}
			return Lightmapping.GIWorkflowMode.Legacy;
		}

		/// <summary>
		/// Changes the lighting GI workflow of a scene asset
		/// </summary>
		/// <param name="workflow"></param>
		/// <param name="targetSceneAsset"></param>
		private void ChangeSceneLightingGIWorkflow(SceneAsset targetSceneAsset, Lightmapping.GIWorkflowMode workflow) {
			var originalSelection = Selection.activeObject;
			var originalContext = Selection.activeContext;
			
			var originalScene = EditorSceneManager.GetActiveScene(); // Find the scene we are on so we can change back to it later
			string newScenePath = AssetDatabase.GetAssetPath(targetSceneAsset);
			var newScene = EditorSceneManager.GetSceneByPath(newScenePath); // Get the scene reference of the target scene
			
			bool requireSceneLoad = !SceneManagerExtensions.IsSceneLoaded(newScene);
			// If the scene we are going to change lightmaps on is loaded and dirty, warn the user any changes will be saved
			if (!requireSceneLoad && newScene.isDirty) {
				if (!EditorUtility.DisplayDialog("Confirm changing GI Lighting Workflow",
					$"By changing the GI lighting workflow on \"{newScene.name}\" it will have to be saved, do you want to continue?",
					"Continue", "Cancel")) {
					return;
				}
			}
			
			// If the scene we are trying to change isn't loaded load it
			if (requireSceneLoad) EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Additive);
			
			var loadedScene = EditorSceneManager.GetSceneByPath(newScenePath);
			
			// Change active scenes to the target so we can change the settings
			bool requireSceneChange = originalScene != newScene;
			if (requireSceneChange) EditorSceneManager.SetActiveScene(loadedScene);
			
			// Change the lightmap workflow
			Lightmapping.giWorkflowMode = workflow;
			EditorSceneManager.SaveScene(loadedScene);
			
			// Change back to the original scene
			if (requireSceneChange) EditorSceneManager.SetActiveScene(originalScene);
			
			// If we loaded the scene unload it when we're done
			if (requireSceneLoad) EditorSceneManager.CloseScene(loadedScene, true);
			
			Selection.SetActiveObjectWithContext(originalSelection, originalContext);
		}

		private string GetButtonLabel(ButtonState buttonState) {
			switch (buttonState) {
				case ButtonState.None:
					return "X";
				case ButtonState.Warning:
					return "!";
				case ButtonState.Correct:
					return "✓";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private string GetBuildSettingsButtonTooltip(ButtonState buttonState) {
			switch (buttonState) {
				case ButtonState.None:
					return "No Scene loaded, cannot check if it is in the build list";
				case ButtonState.Warning:
					return "Scene is not in build list and cannot be loaded at runtime, click to fix";
				case ButtonState.Correct:
					return "Scene is in build list and can be loaded at runtime, click to remove from build list";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private string GetLightmapSettingsButtonTooltip(ButtonState buttonState) {
			switch (buttonState) {
				case ButtonState.None:
					return "No Scene loaded, GI Lighting Workflow cannot be read";
				case ButtonState.Warning:
					return "Lightmap workflow set to automatic, this will cause the lighting to be rendered incorrectly if this is set as the active scene, click to set to 'Manual'";
				case ButtonState.Correct:
					return "Lightmap workflow set to manual, this scene can now be 'Set as Active' during runtime, remember to manually bake when you change elements in the scene, click to set to 'Automatic'";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private Color GetButtonColor(ButtonState buttonState) {
			switch (buttonState) {
				case ButtonState.None:
					return Color.red;
				case ButtonState.Warning:
					return Color.yellow;
				case ButtonState.Correct:
					return Color.green;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
#endif
}