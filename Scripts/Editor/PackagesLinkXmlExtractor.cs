using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;

namespace RyanQuagliataUnity.Editor {
	/// <summary>
	/// https://forum.unity.com/threads/while-custom-package-need-a-link-xml.727460/#post-5832736
	/// </summary>
	public class PackagesLinkXmlExtractor : IPreprocessBuildWithReport, IPostprocessBuildWithReport {
		public string TemporaryFolder => $"{Application.dataPath}/TemporaryLinkXml";

		public string LinkFilePath => $"{TemporaryFolder}/link.xml";
		public string LinkFileMetaPath => $"{TemporaryFolder}/link.xml.meta";
		public string TemporaryFolderMetaPath => $"{TemporaryFolder}.meta";

		public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report) => CreateMergedLinkFromPackages();

		public void OnPostprocessBuild(BuildReport report) {
			if (File.Exists(LinkFilePath)) File.Delete(LinkFilePath);
			if (File.Exists(LinkFileMetaPath)) File.Delete(LinkFileMetaPath);
			if (!Directory.EnumerateFiles(TemporaryFolder, "*").Any()) {
				Directory.Delete(TemporaryFolder);
				if (File.Exists(TemporaryFolderMetaPath)) File.Delete(TemporaryFolderMetaPath);
			}
		}

		private void CreateMergedLinkFromPackages() {
			var request = Client.List();
			do { } while (!request.IsCompleted);

			if (request.Status == StatusCode.Success) {
				List<string> xmlPathList = new List<string>();
				foreach (var package in request.Result) {
					var path = package.resolvedPath;
					xmlPathList.AddRange(Directory.EnumerateFiles(path, "link.xml", SearchOption.AllDirectories).ToList());
				}

				if (xmlPathList.Count <= 0) return;

				var xmlList = xmlPathList.Select(XDocument.Load).ToArray();

				var combinedXml = xmlList.First();
				foreach (var xDocument in xmlList.Where(xml => xml != combinedXml)) {
					combinedXml.Root.Add(xDocument.Root.Elements());
				}

				if (!Directory.Exists(TemporaryFolder)) Directory.CreateDirectory(TemporaryFolder);
				combinedXml.Save(LinkFilePath);
			} else if (request.Status >= StatusCode.Failure) {
				Debug.LogError(request.Error.message);
			}
		}
	}
}