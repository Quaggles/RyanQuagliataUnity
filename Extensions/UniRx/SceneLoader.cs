using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RyanQuagliataUnity.Attributes;
// ReSharper disable once RedundantUsingDirective
using RyanQuagliataUnity.Utilities;
using Sirenix.OdinInspector;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RyanQuagliataUnity.Extensions.UniRx {
	public enum LoadFormat {
		/// <summary>
		/// Scenes are loaded sequentially, the game thread is paused during this operation, progress is reported as 0 when starting and 1 when done
		/// </summary>
		Sequential,

		/// <summary>
		/// Scenes sequentially on a background thread
		/// </summary>
		SequentialAsync,

		/// <summary>
		/// All scenes are loaded at the same time on background threads with no guarantee to order
		/// </summary>
		SimultaneousAsync
	}

	public class SceneLoader : MonoBehaviour {
		#region Inspector Region
		private bool IsEditMode => !Application.isPlaying;
		[PropertyOrder(-1), Button("Load Scenes"), InfoBox("When in edit mode the load format will always be 'Sequential' as you can only async load levels at runtime", InfoMessageType.Warning, "IsEditMode")]
		private async Task LoadScenesAsyncButton() {
			var progress = new Progress<float>();
			progress.ProgressChanged += ReportProgress; 
			await LoadScenesAsync(progress);
		}
	
		[TabGroup("Scenes To Load")]
		public SceneField[] SceneLoadList;

		[TabGroup("Scenes To Load"), InfoBox("Active Scene is null or not in load list and will be ignored", InfoMessageType.Warning, "ActiveSceneWontBeLoaded")]
		public SceneField NewActiveScene = null;
	
		private bool ActiveSceneWontBeLoaded => ActiveSceneNull || !ActiveSceneInLoadList;
		private bool ActiveSceneInLoadList => SceneLoadList.Any(x => x.ScenePath == NewActiveScene.ScenePath);
		private bool ActiveSceneNull => NewActiveScene.IsNull;
	
		[TabGroup("Load Settings"), EnumToggleButtons]
		public LoadFormat LoadFormat;
		[TabGroup("Load Settings")]
		public bool LoadOnAwake = true;
		[TabGroup("Load Settings"), Tooltip("When loading a scene this will search through its root objects, if it located a 'Load Scene' script this will also trigger its Load Scenes function, recursively calling it until all child scenes are loaded")]
		public bool LoadChildScenes;
		[TabGroup("Load Settings"), Tooltip("When loading child scenes should they be loaded with the same load format as this script?")]
		public bool ChildScenesInheritLoadFormat = true;
	
		[TabGroup("Progress Reporting"), Range(0, 1000), SuffixLabel("ms"), Tooltip("How often should progress be reported about the level load status, setting this too low might slow down loading")]
		public int ProgressReportInterval = 25;
		[Serializable]
		public class ProgressReportEvent : UnityEvent<float> { }
		[TabGroup("Progress Reporting")]
		public ProgressReportEvent OnProgressReport;
	
		[Serializable] public class SceneEvent : UnityEvent<Scene> { }
		[TabGroup("Events")]
		public SceneEvent OnSceneLoadStart;
		[TabGroup("Events")]
		public SceneEvent OnChildSceneLoaded;
		[TabGroup("Events")]
		public SceneEvent OnSceneLoadComplete;

		#endregion
	
		private bool currentlyLoadingTask = false;

		private async void Awake() {
			if (LoadOnAwake) {
				var progress = new Progress<float>();
				progress.ProgressChanged += ReportProgress; 
				await LoadScenesAsync(progress);
			}
		}

		private void ReportProgress(object sender, float progress) {
			if (IsEditMode) return;
			OnProgressReport.Invoke(progress);
		}

		private bool AllScenesLoaded {
			get {
				// If the active scene isn't set correctly count that as not loaded
				if (!ActiveSceneNull && NewActiveScene.ScenePath != SceneManager.GetActiveScene().path) return false;
			

				foreach (var sceneToLoad in SceneLoadList) {
					// Loop through every scene we need to load
					if (!SceneManagerExtensions.IsSceneLoaded(sceneToLoad)) return false;
				}
				return true;
			}
		}

		private int UnloadedSceneCount {
			get {
				int count = 0;
				foreach (var sceneToLoad in SceneLoadList) {
					if (!SceneManagerExtensions.IsSceneLoaded(sceneToLoad)) count++;
				
				}
				return count;
			}
		}
	
		public async Task LoadScenesAsync(IProgress<float> progress = null) {
			if (currentlyLoadingTask) throw new ScenesAlreadyLoadingException();
			currentlyLoadingTask = true;
			var thisScene = SceneManagerExtensions.GetLoadedSceneContainingGameObject(this.gameObject);
			OnSceneLoadStart?.Invoke(thisScene);
			try {
				Debug.Log(
					$"Scene: '{SceneManagerExtensions.GetLoadedSceneContainingGameObject(this.gameObject).name}' is loading with format: '{LoadFormat}'");

				// If the scenes are already loaded nothing needs to be done
				if (AllScenesLoaded) {
					Debug.Log(
						$"Scene: '{SceneManagerExtensions.GetLoadedSceneContainingGameObject(this.gameObject).name}' has all its children loaded so is cancelled");
					return;
				}

				// Report 0 progress initially
				progress?.Report(0);
			
				if (!Application.isPlaying) {
					// In edit mode scenes need to be loaded using different method
					LoadScenesEditor();
				} else {
					switch (LoadFormat) {
						case LoadFormat.Sequential:
							LoadScenesSyncronously();
							progress?.Report(1); // Finished when syncrounously is done
							break;
						case LoadFormat.SequentialAsync:
							await LoadScenesSequentialAync(progress);
							break;
						case LoadFormat.SimultaneousAsync:
							await LoadScenesSimultaneousAsync(progress);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				// If the game is running and a scene was just loaded we need to wait until the next frame to set it as active
				if (Application.isPlaying)
					await UniTask.DelayFrame(0, PlayerLoopTiming.PostLateUpdate); 
			
				if (!ActiveSceneNull) {
					Debug.Log($"Setting scene {NewActiveScene.ScenePath} as active");
					if (!SceneManagerExtensions.IsSceneLoaded(NewActiveScene))
						throw new ActiveSceneNotReadyException();
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(NewActiveScene));
				} else {
					Debug.Log($"Scene {NewActiveScene.ScenePath} is null and cannot be set as active");
				}
			} finally {
				currentlyLoadingTask = false;
				OnSceneLoadComplete?.Invoke(thisScene);
			}
		}
	
		/// <summary>
		/// <b>Warning</b> editor only
		/// </summary>
		private void LoadScenesEditor() {
#if UNITY_EDITOR
			foreach (var scene in SceneLoadList) {
				if (!CheckSceneLoadedScene(scene)) {
					var loadedScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.ScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
					OnChildSceneLoaded?.Invoke(loadedScene);
				}
				LoadSceneChildren(scene.ScenePath).Wait();
			}
#else
		throw new EditorOnlyException("LoadScenesEditor");
#endif
		}

		private void LoadScenesSyncronously() {
			foreach (var scene in SceneLoadList) {
				if (!CheckSceneLoadedScene(scene)){
					SceneManager.LoadScene(scene, LoadSceneMode.Additive);
					OnChildSceneLoaded?.Invoke(SceneManager.GetSceneByPath(scene.ScenePath));
				}

				if (LoadChildScenes) LoadSceneChildren(scene.ScenePath).Wait();
			}
		}

		private async Task LoadScenesSequentialAync(IProgress<float> progress = null) {
			int total = SceneLoadList.Length;
		
			// Creates a list of gameobjects here to recycle and reduce garbage
			// This is only possible because scenes are load
			List<GameObject> rootGameObjects = new List<GameObject>();

			// Work out how many scenes we need to load and how many are currently loaded to ensure correct progress is reported
			int unloadedSceneCount = UnloadedSceneCount;
			int loadCount = 0;
		
			// Load each scene in order waiting for the current to complete before moving on
			foreach (var scene in SceneLoadList) {
				if (CheckSceneLoadedScene(scene)) continue;
			
				// Load scene
				AsyncOperation task = SceneManager.LoadSceneAsync(scene.ScenePath, LoadSceneMode.Additive);
			
				// When the load task completes find the newly loaded scene and notify that a scene has been loaded
				task.completed += x => { OnChildSceneLoaded?.Invoke(SceneManager.GetSceneByPath(scene.ScenePath)); };
			
				// While waiting for the task to complete only report progress on smaller interval to now waste performance
				while (!task.isDone) {
					float startValue = (float) loadCount / unloadedSceneCount;
					float normalizedCurrentProgress = task.progress / unloadedSceneCount;
					progress?.Report(startValue + normalizedCurrentProgress);
					await Task.Delay(ProgressReportInterval);
				}
			
			
				// Find the newly created scene, search through root objects and find if it needs to load levels of its own
				if (LoadChildScenes) await LoadSceneChildren(scene.ScenePath, rootGameObjects);
			
				loadCount++;
			}
			// Report finished at the end as sometimes the LoadSceneAsync function doesn't end with 100% progress
			progress?.Report(1);
		}

		private async Task LoadScenesSimultaneousAsync(IProgress<float> progress = null) {
			// Load all the scenes
			int total = SceneLoadList.Length;
			var sceneAwaits = new List<UnityAsyncExtensions.AsyncOperationAwaiter>(total);
			var sceneChildrenAwaits = new List<TaskAwaiter>();

			// Create tasks to load all the scenes
			foreach (var scene in SceneLoadList) {
				if (CheckSceneLoadedScene(scene)) continue;
			
				var task = SceneManager.LoadSceneAsync(scene.ScenePath, LoadSceneMode.Additive);
				sceneAwaits.Add(task.GetAwaiter()); // Wait for the scenes to load
				
				// When each scene finishes loading tell them to load their children and also wait for them to finish
				if (LoadChildScenes) {
					task.completed += operation => {
						var newScene = SceneManager.GetSceneByPath(scene.ScenePath);
						OnChildSceneLoaded?.Invoke(newScene); // Notify that a scene has been loaded
						sceneChildrenAwaits.Add(LoadSceneChildren(newScene).GetAwaiter());
					};
				}
			}

			// Wait until each scene and their children is finished loading
			await new WaitUntil(() => {
				foreach (var task in sceneAwaits)
					if (!task.IsCompleted) return false;
				foreach (var task in sceneChildrenAwaits)
					if (!task.IsCompleted) return false;
				return true;
			});
		}

		private bool CheckSceneLoadedScene(SceneField scene) {
			if (SceneManagerExtensions.IsSceneLoaded(scene)) {
				Debug.LogWarning($"Scene {scene.SceneName} is already loaded... Skipping");
				return true;
			}
			Debug.LogWarning($"Scene {scene.SceneName} is not loaded... Loading");
			return false;
		}

		private async Task LoadSceneChildren(string scenePath, List<GameObject> listForRecycling = null) {
			await LoadSceneChildren(SceneManager.GetSceneByPath(scenePath), listForRecycling);
		}

		private async Task LoadSceneChildren(Scene scene, List<GameObject> listForRecycling = null) {
			if (!scene.IsValid()) throw new ArgumentException("Scene is not valid");
			if (listForRecycling == null) listForRecycling = new List<GameObject>(scene.rootCount);
			scene.GetRootGameObjects(listForRecycling);
		
			// Loop through all the root gameobjects looking for 'Load Scene' scripts, if one is found also load their scenes
			foreach (var go in listForRecycling) {
				var loadScenes = go.GetComponent<SceneLoader>();
				if (loadScenes != null) {
					if (ChildScenesInheritLoadFormat) {
						var originalLoadFormat = loadScenes.LoadFormat;
						loadScenes.LoadFormat = LoadFormat;
						await loadScenes.LoadScenesAsync();
						loadScenes.LoadFormat = originalLoadFormat;
					} else {
						await loadScenes.LoadScenesAsync();
					}
				}
			}
		}

		public class ActiveSceneNotReadyException : Exception {
			public override string Message => "The scene could not be set as active because it is unassigned or not loaded";
		}
	
		public class ScenesAlreadyLoadingException : Exception {
			public override string Message => "Scenes are currently being loaded, cannot start loading scenes";
		}
	}
}