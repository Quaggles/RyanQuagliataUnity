using UnityEngine;

namespace RyanQuagliata.Extensions {
	public static class MeshFilterExtensions {
		/// <summary>
		/// If the application is playing it gets the instantiated mesh, otherwise gets the shared one
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public static Mesh GetProperMesh(this MeshFilter that) => Application.isPlaying ? that?.mesh : that?.sharedMesh;
	}
}