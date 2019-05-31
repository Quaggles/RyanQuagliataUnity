#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RyanQuagliata.PluginExtensions.SharpConfig {
	public class ExportDuringBuild : IPreprocessBuildWithReport {
		public int callbackOrder => 0;
		public void OnPreprocessBuild(BuildReport report) {
			try {
				var buildDirectory = Directory.GetParent(report.summary.outputPath).ToString();
				if (!buildDirectory.EndsWith("\\")) buildDirectory += '\\';
				Debug.Log($"{report.summary.platform.ToString()} build triggered at {report.summary.outputPath}, exporting config files to {buildDirectory}...");
				Debug.Log($"Exporting of config files successful");
			} catch (Exception ex){
				Debug.LogWarning($"Error during config export: {ex.Message}");
				throw;
			}
		}
	}
}
#endif