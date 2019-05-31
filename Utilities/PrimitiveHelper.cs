using System.Collections.Generic;
using UnityEngine;

namespace RyanQuagliata.Utilities {
	/// <summary>
	/// By Octopoid: http://answers.unity3d.com/questions/514293/changing-a-gameobjects-primitive-mesh.html
	/// </summary>
	public static class PrimitiveHelper {
		private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();
 		
		/// <summary>
		/// Creates a new instance of the Primative Mesh Type as a gameobject with a mesh filter
		/// and mesh renderer
		/// </summary>
		/// <param name="type"></param>
		/// <param name="withCollider"></param>
		/// <returns></returns>
		public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider) {
			if (withCollider) return GameObject.CreatePrimitive(type);

			GameObject gameObject = new GameObject(type.ToString());
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(type);
			gameObject.AddComponent<MeshRenderer>();
 
			return gameObject;
		}
 		
		/// <summary>
		/// Gets a primitive mesh from the cache if it exists, otherwise creates a new one
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Mesh GetPrimitiveMesh(PrimitiveType type) {
			if (!PrimitiveHelper.primitiveMeshes.ContainsKey(type))
				PrimitiveHelper.CreatePrimitiveMesh(type);

			return PrimitiveHelper.primitiveMeshes[type];
		}
 
		private static Mesh CreatePrimitiveMesh(PrimitiveType type) {
			GameObject gameObject = GameObject.CreatePrimitive(type);
			Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			GameObject.Destroy(gameObject);
 
			PrimitiveHelper.primitiveMeshes[type] = mesh;
			return mesh;
		}
	}
}