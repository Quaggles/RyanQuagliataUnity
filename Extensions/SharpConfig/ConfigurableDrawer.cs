#if false
#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RyanQuagliata.PluginExtensions.SharpConfig {
	public class ConfigurableDrawer : OdinAttributeDrawer<ConfigurableAttribute> {
		protected override void DrawPropertyLayout(GUIContent label) {
			var newValue = Property.ValueEntry.WeakSmartValue;
			
			// Get a member to config map for this value
			if (!SharpConfigUnity.TryGetMember2ConfigMap(Property.Info.GetMemberInfo(), out var member2ConfigMap)) {
				Debug.LogError("This should never happen");
				this.CallNextDrawer(label);
				return;
			}

			// Member info becomes invalid as soon as recompilation occurs so don't compare using it
			bool PredicateWithoutMemberInfoMatch(KeyValuePair<SharpConfigUnity.Member2ConfigMap, object> x) {
				var (key, _) = (x.Key, x.Value);
				if (key.Config != member2ConfigMap.Config) return false;
				if (key.Section != member2ConfigMap.Section) return false;
				if (key.Setting != member2ConfigMap.Setting) return false;
				return true;
			}

			// Find the current stored default value
			object currentValue;
			try {
				var result = SharpConfigUnity.ConfigDefaultValues.First(PredicateWithoutMemberInfoMatch);
				currentValue = result.Value;
			} catch {
				currentValue = null;
			}

			//SharpConfigUnity.ConfigDefaultValues.TryGetValue(memberInfo, out var currentValue);
			if (currentValue == null) {
				if (GUILayout.Button($"Set", EditorStyles.miniButton)) {
					SharpConfigUnity.ConfigDefaultValues.Add(member2ConfigMap, Property.ValueEntry.WeakSmartValue);
				}
			} else {
				if (Equals(currentValue.GetHashCode(), newValue.GetHashCode())) {
					if (GUILayout.Button($"Remove default config value `{currentValue}`", EditorStyles.miniButton)) {
						SharpConfigUnity.ConfigDefaultValues.Remove(member2ConfigMap);
					}
				} else {
					if (GUILayout.Button($"Default config value is `{currentValue}` replace with `{newValue}`?",
						EditorStyles.miniButton)) {
						SharpConfigUnity.ConfigDefaultValues.Remove(member2ConfigMap);
						SharpConfigUnity.ConfigDefaultValues.Add(member2ConfigMap, Property.ValueEntry.WeakSmartValue);
					}
				}
			}

			this.CallNextDrawer(label);
		}
	}
}

#endif
#endif