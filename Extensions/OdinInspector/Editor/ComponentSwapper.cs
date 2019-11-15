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
	public abstract class ComponentSwapper<T1, T2> : OdinEditorWindow where T1 : Component where T2 : Component {
//		[MenuItem("RyanQuagliata/Component Swapper/ComponentSwapper")]
//		public static void Create() => GetWindow<ComponentSwapper>().Show();

		public List<T1> Items = new List<T1>();

		protected virtual bool IsValid => typeof(T1) != typeof(T2);

		[Button]
		public void FetchFromScene() {
			Items.Clear();
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				var newItems = SceneManager.GetSceneAt(i).GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<T1>());
				Items.AddRange(newItems);
			}
		}

		private bool RequiresTemp() => !MultipleComponentsAllowed<T1, T2>();

		protected static bool MultipleComponentsAllowed<T1, T2>() => MultipleComponentsAllowed(typeof(T1), typeof(T2));

		protected static bool MultipleComponentsAllowed(Type a, Type b) {
			// Get inheritance chains for each class
			var a0 = typeof(T1).GetBaseClasses(true);
			var b0 = typeof(T2).GetBaseClasses(true);

			// Find common parents
			Type OuterKeySelector(Type type) => type;
			var joined = a0.Join(b0, OuterKeySelector, OuterKeySelector, (x, y) => (x, y));

			// If any common parent has `DisallowMultipleComponent` then they can't coexist
			foreach (var valueTuple in joined)
				if (valueTuple.x.GetAttribute<DisallowMultipleComponent>(false) != null)
					return false;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="forceTemp">Uses a temporary gameobject incase that both the Original and New component types cannot be on the gameobject simultaniously</param>
		[Button, EnableIf(nameof(IsValid))]
		public void Process() {
			cancelAllRequested = false;
			if (Items.Count(x => x) > 0)
				Undo.RegisterCompleteObjectUndo(Items.Where(x => x).Select(x => x.gameObject).ToArray(),
					$"{Items.Count} component swap from {typeof(T1).GetNiceFullName()} => {typeof(T2).GetNiceFullName()}");

			var requiresTemp = RequiresTemp();
			var keep = new List<T1>();
			foreach (var o in Items) {
				if (cancelAllRequested) return;
				if (!Swap(o, requiresTemp)) {
					keep.Add(o);
				}
			}
			Items.Clear();
			Items.AddRange(keep);
		}

		private static bool cancelAllRequested = false;

		protected bool Swap(T1 oldInstance, bool requiresTemp = false) {
			var all = GetAllActiveInScene().Where(x => x != oldInstance);
			var dependencies = all.Where(x => componentReferences(x, oldInstance));
			if (dependencies.Any()) {
				var dialogResult = EditorUtility.DisplayDialogComplex("Confirm",
					$"{dependencies.Count()} objects references the object\n\"{GetComponentPath(oldInstance)}\"\nThe following references will be lost if you continue\n\n{string.Join(",\n", dependencies.Select(GetComponentPath))}",
					"Ok",
					"Cancel", "Cancel All");
				switch (dialogResult) {
					case 0: break;
					case 1: return false;
					case 2:
						cancelAllRequested = true;
						goto case 1;
				}
			}
			PreCopy(oldInstance);
			T2 newInstance = default;
			newInstance = requiresTemp ? SwapWithTemp(oldInstance) : SwapInPlace(oldInstance);
			PostCopy(newInstance);
			return true;
		}
		
		string GetComponentPath(Component component) => $"{GetTransformPath(component.transform)} [{component.GetType().GetNiceFullName()}]";

		string GetTransformPath(Transform transform) {
			void AddParent(Transform trans, ref string pathRef) {
				pathRef = $"{trans.name}/" + pathRef;
				if (trans.parent != null) AddParent(trans.parent, ref pathRef);
			}
			string path = transform.name;
			AddParent(transform.parent, ref path);
			return path;
		}
		
		/// <summary>
		/// Determines if the component makes any references to the second "references" component in any of its inspector fields
		/// </summary>
		private static bool componentReferences(Component component, Component references)
		{
			// find all fields exposed in the editor as per https://answers.unity.com/questions/1333022/how-to-get-every-public-variables-from-a-script-in.html
			SerializedObject serObj = new SerializedObject(component);
			SerializedProperty prop = serObj.GetIterator();
			while (prop.NextVisible(true))
			{
				bool isObjectField = prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue != null;
				if (isObjectField && prop.objectReferenceValue == references) {
					return true;
				}
			}

			return false;
		}

		private static Component[] GetAllActiveInScene() {
			var rootObjects = SceneManager
				.GetActiveScene()
				.GetRootGameObjects();
			List<Component> result = new List<Component>();
			foreach (var rootObject in rootObjects) result.AddRange(rootObject.GetComponentsInChildren<Component>());

			return result.ToArray();
		}

		protected T2 SwapInPlace(T1 oldInstance) {
			var newComponent = oldInstance.gameObject.AddComponent<T2>();
			CopyComponents(oldInstance, newComponent);
			return newComponent;
		}

		protected T2 SwapWithTemp(T1 oldInstance) {
			var tempGameObject = new GameObject {name = $"[{this.GetType().Name}] Temporary Swapper Object"};
			var gameobject = oldInstance.gameObject;
			try {
				var tempComponent = tempGameObject.AddComponent<T2>();

				// Copy the properties over
				CopyComponents(oldInstance, tempComponent);

				// Destroy the original image
				oldInstance.DestroySmart();

				var newComponent = gameobject.AddComponent<T2>();

				// Copy the data from the temp image
				EditorUtility.CopySerialized(tempComponent, newComponent);
				return newComponent;
			} finally { // Destroy the temp image
				tempGameObject.DestroySmart();
			}
		}

		public virtual void CopyComponents(T1 source, T2 destination) {
			var sourceSo = new SerializedObject(source);
			var destSo = new SerializedObject(destination);
			EditorUtilityExtensions.CopySerializedPolymorphic(sourceSo, destSo);
		}

		public virtual void PreCopy(T1 that) { }

		public virtual void PostCopy(T2 that) { }
	}
}
#endif