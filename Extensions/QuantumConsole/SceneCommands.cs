using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	[CommandPrefix("Scene")]
	public static class SceneCommands {
		[Command]
		public static bool IncludeInactive = true;

		[Command]
		public static List<T> GetAll<T>(string searchTerm = "", SearchType searchType = SearchType.Contains, bool ignoreCase = true) where T : Component =>
			ObjectExtensions.FindSceneObjectsOfType<T>(IncludeInactive).Where(x => x.transform.GetPath().Filter(searchTerm, searchType, ignoreCase)).ToList();

		[Command]
		public static void List<T>(string searchTerm = "", SearchType searchType = SearchType.Contains, bool ignoreCase = true) where T : Component {
			var items = GetAll<T>(searchTerm, searchType, ignoreCase);
			Debug.Log(StringExtensions.FilterToString(items?.Count ?? 0, searchTerm, searchType, ignoreCase));
			if (items == null) return;
			for (var i = 0; i < items.Count; i++) {
				var item = items[i];
				Debug.Log($"[{i}] {item.transform.GetPath()}");
			}
		}

		[Command]
		public static T Get<T>(string searchTerm = "", SearchType searchType = SearchType.Contains, bool ignoreCase = true) where T : Component {
			foreach (var item in GetAll<T>())
				if (item.name.Filter(searchTerm, searchType, ignoreCase))
					return item;
			Debug.LogError($"No {typeof(T).Name} with name \"{searchTerm}\" in search mode \"{searchType.ToString()}\" could be found");
			return null;
		}

		[Command]
		public static T GetByPath<T>(string searchTerm = "", SearchType searchType = SearchType.Contains, bool ignoreCase = true) where T : Component {
			foreach (var item in GetAll<T>())
				if (StringExtensions.Filter(item.transform.GetPath(), searchTerm, searchType, ignoreCase))
					return item;
			Debug.LogError($"No {typeof(T).Name} with name \"{searchTerm}\" in search mode \"{searchType.ToString()}\" could be found");
			return null;
		}

		[Command]
		public static T GetByIndex<T>(int index) where T : Component => GetAll<T>()[index];
	}
}