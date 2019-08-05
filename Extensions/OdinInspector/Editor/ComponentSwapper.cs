#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using RyanQuagliataUnity.Extensions.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Extensions.OdinInspector.Editor {
//	public class ImageSwapper : ComponentSwapper<Image, ImagePPU> {
//		[MenuItem("RyanQuagliata/ComponentSwapper")]
//		public static void Create() {
//			GetWindow<ImageSwapper>().Show();
//		}
//	}

	public class ComponentSwapper<T1, T2> : OdinEditorWindow where T1 : Component where T2 : Component {
		public List<T1> Items = new List<T1>();

		[Button]
		public void FetchFromScene() {
			Items.Clear();
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var newItems = SceneManager.GetSceneAt(i).GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<T1>());
				Items.AddRange(newItems);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requiresTemp">Uses a temporary gameobject incase that both the Original and New component types cannot be on the gameobject simultaniously</param>
		[Button]
		public void Swap(bool requiresTemp = false) {
			if (Items.Count > 0)
				Undo.RegisterCompleteObjectUndo(Items.Select(x => x.gameObject).ToArray(),
					$"{Items.Count} component swap from {typeof(T1).GetNiceFullName()} => {typeof(T2).GetNiceFullName()}");
			foreach (var o in Items) {
				Swap(o, typeof(T2));
			}
		}

		public static void Swap(Component oldInstance, Type newType, bool requiresTemp = false) {
			//Undo.RegisterCompleteObjectUndo(oldInstance.gameObject, $"Component swap from {typeof(T1).GetNiceFullName()} => {typeof(T2).GetNiceFullName()}");
			var go = oldInstance.gameObject;
			var source = new SerializedObject(oldInstance);

			// Create a temporary gameobject to hold the image since you can't have 2 graphic components on one gameobject
			var tempGo = new GameObject();
			try {
				var dependencies = EditorUtility.CollectDependencies(new Object[] {oldInstance});

				if (dependencies.Length <= 0 || !EditorUtility.DisplayDialog("Confirm",
					    $"{dependencies.Length} objects depend on this instance, these reference will be lost if you swap\n{(string.Join("\n", dependencies as object[]))}", "yes",
					    "no")) return;

				var tempComponent = tempGo.AddComponent(newType);
				var dest = new SerializedObject(tempComponent);

				// Copy the properties over
				EditorUtilityExtensions.CopySerializedPolymorphic(source, dest);

				// Destroy the original image
				oldInstance.DestroySmart();

				// Add the ImagePPU to replace the UnityEngine.UI.Image
				var newComponent = go.AddComponent(newType);

				// Copy the data from the temp image
				EditorUtility.CopySerialized(tempComponent, newComponent);
			} finally { // Destroy the temp image
				tempGo.DestroySmart();
			}
		}
	}
}
#endif