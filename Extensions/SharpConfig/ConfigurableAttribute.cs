using System;

namespace RyanQuagliataUnity.Extensions.SharpConfig {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ConfigurableAttribute : Attribute {
		public readonly string ConfigFileName;
		public readonly string SectionName;
		public readonly string SettingName;
		public readonly string Comment;

		public ConfigurableAttribute(string configFileName = null, string sectionName = null, string settingName = null, string comment = "") {
			ConfigFileName = configFileName;
			SectionName = sectionName;
			Comment = comment;
		}
	}
}