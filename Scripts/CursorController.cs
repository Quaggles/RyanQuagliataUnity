using UnityEngine;

namespace RyanQuagliata.Scripts {
	public class CursorController : MonoBehaviour {
		public bool VisibleOnStart = true;

		public KeyCode ToggleKeycode = KeyCode.C;
		
		// Start is called before the first frame update
		void Start() {
			if (!Application.isEditor) {
				Cursor.visible = VisibleOnStart;
			}
		}

		// Update is called once per frame
		void Update() {
			if (Input.GetKeyDown(ToggleKeycode)) Cursor.visible = !Cursor.visible;
		}
	}
}