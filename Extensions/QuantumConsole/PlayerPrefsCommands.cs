using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	[CommandPrefix("PlayerPrefs")]
	public static class PlayerPrefsCommands {
		[Command]
		public static void DeleteAll() => PlayerPrefs.DeleteAll();
		
		[Command]
		public static void Save() => PlayerPrefs.Save();

		[Command]
		public static float GetFloat(string key) => PlayerPrefs.GetFloat(key);
		
		[Command]
		public static int GetInt(string key) => PlayerPrefs.GetInt(key);
		
		[Command]
		public static string GetString(string key) => PlayerPrefs.GetString(key);
		
		[Command]
		public static void DeleteKey(string key) {
			PlayerPrefs.DeleteKey(key);
			Save();
		}
		
		[Command]
		public static void SetFloat(string key, float value) {
			PlayerPrefs.SetFloat(key, value);
			Save();
		}

		[Command]
		public static void SetInt(string key, int value) {
			PlayerPrefs.SetInt(key, value);
			Save();
		}

		[Command]
		public static void SetString(string key, string value) {
			PlayerPrefs.SetString(key, value);
			Save();
		}

		[Command]
		public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
	}
}