using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RyanQuagliataUnity.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Editor {
	public static class MeshSerializer {
		[InitializeOnLoadMethod]
		static void Listen() => PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdate;

		public static readonly List<string> IgnoreIfWithComponent = new List<string>{
			"TMPro.TextMeshPro",
		};

		static void PrefabInstanceUpdate(GameObject instance) {
#if ODIN_INSPECTOR
			if (!ModelPostProcessorConfig.Instance.MeshSerializerEnabled) return;
#endif
			var prefab = PrefabUtility.GetCorrespondingObjectFromSource(instance);
			string prefabPath = AssetDatabase.GetAssetPath(prefab);

			// Create a tuple of the mesh filters on the instance we're saving and the new prefab
			var filters = instance.GetComponentsInChildren<MeshFilter>().Zip(
				prefab.GetComponentsInChildren<MeshFilter>(),
				(instanceFilter, prefabFilter) => (instanceFilter, prefabFilter));

			// Find all meshes that aren't already assets
			var nonAssets = filters.Where(x => !IsAsset(x.instanceFilter.sharedMesh));

			// Filter out meshfilters if they are next to components on the ignore list
			nonAssets = nonAssets.Where(x => {
				var comps = x.instanceFilter.GetComponents<Component>();
				return comps.All(y => IgnoreIfWithComponent.Any(z => z != y.GetType().FullName));
			});
			if (nonAssets.IsEmpty()) return; // If there are no non-asset mesh's no work to be done

			// Confirm to the user they want save
			if (!EditorUtility.DisplayDialog("Warning",
				$"The GameObject you are creating a prefab from contains {nonAssets.Count()} dynamic meshes, their data will be lost unless they are saved as assets, do you want to save them?\n{string.Join(",\n", nonAssets.Select(x => x.instanceFilter.name))}",
				"Yes", "No")) {
				return;
			}


			bool valid = false;
			string path = "";

			if (nonAssets.Count() == 1) { // Individual file export
				var meshFilter = nonAssets.First();

				// Ensure the user chooses a valid path
				while (!valid) {
					path = EditorUtility.SaveFilePanel("Export Mesh", Path.GetDirectoryName(prefabPath),
						meshFilter.instanceFilter.name, "asset");
					if (string.IsNullOrWhiteSpace(path)) return;

					// If valid then make path relative, otherwise try again
					if (path.StartsWith(Application.dataPath)) {
						valid = true;
						path = "Assets" + path.Substring(Application.dataPath.Length);
					} else {
						EditorUtility.DisplayDialog("Error",
							$"File must reside within {Application.dataPath}", "OK");
					}
				}

				// Create the new mesh asset and link it up to the MeshFilter in the new prefab
				try {
					AssetDatabase.CreateAsset(meshFilter.instanceFilter.sharedMesh, path);
					meshFilter.prefabFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
				} catch (Exception ex) {
					EditorUtility.DisplayDialog("Error", ex.ToString(), "OK");
				}
			} else { // Bulk folder export
				// Ensure the user chooses a valid path
				while (!valid) {
					path = EditorUtility.SaveFolderPanel("Export Meshes", Path.GetDirectoryName(prefabPath),
						Path.GetDirectoryName(prefabPath));
					if (string.IsNullOrWhiteSpace(path)) return;

					// If valid then make path relative, otherwise try again
					if (path.StartsWith(Application.dataPath)) {
						valid = true;
						path = "Assets" + path.Substring(Application.dataPath.Length);
					} else {
						EditorUtility.DisplayDialog("Error",
							$"Selected folder must reside within {Application.dataPath}", "OK");
					}
				}

				// Create the new mesh assets and link it up to the MeshFilter in the new prefab
				int progress = 0;
				int total = nonAssets.Count();
				foreach (var meshFilter in nonAssets) {
					try {
						var validPath =
							AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path,
								$"{meshFilter.instanceFilter.name}.asset"));
						EditorUtility.DisplayProgressBar("Serializing Meshes", $"Saving {validPath}",
							progress / (float) total);
						AssetDatabase.CreateAsset(meshFilter.instanceFilter.sharedMesh, validPath);
						meshFilter.prefabFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(validPath);
						progress++;
					} catch (Exception ex) {
						EditorUtility.DisplayDialog("Error", ex.ToString(), "OK");
					}
				}

				EditorUtility.ClearProgressBar();
			}

			AssetDatabase.SaveAssets();
		}

		static bool IsAsset(Object obj) => !string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(obj));
	}
}