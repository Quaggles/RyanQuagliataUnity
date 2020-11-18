using System;
using System.Collections.Generic;
using System.IO;
using RyanQuagliataUnity.Extensions.OdinInspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Editor {
    [GlobalConfig(SerializedGlobalConfig<CopyProjectFilesOnBuild>.ASSET_FOLDER)]
    public class CopyProjectFilesOnBuild : GlobalConfig<CopyProjectFilesOnBuild>, IPostprocessBuildWithReport {
        [Serializable]
        public struct CopyItem {
            private string SourceInfo => $"<b>{{StreamingAssets}}</b>: {Application.streamingAssetsPath}\n" +
                                         $"<b>{{DataPath}}</b>: {Application.dataPath}\n" +
                                         $"<b>{{PersistentDataPath}}</b>: {Application.persistentDataPath}";
        
            private string DestinationInfo => $"<b>{{OutputPath}}</b>: {Application.productName}/\n" +
                                              $"<b>{{OutputPathData}}</b>: {Application.productName}/{Application.productName}_Data/\n" +
                                              $"<b>{{StreamingAssets}}</b>: {Application.productName}/{Application.productName}_Data/StreamingAssets/\n" +
                                              $"<b>{{Name}}</b>: {Name}\n" +
                                              $"<b>{{Ext}}</b>: {Path.GetExtension(FileName)}\n" +
                                              $"<b>{{FileName}}</b>: {FileName}";
            public enum CopyType {
                Object,
                FilePath,
                DirectoryPath,
            }

            public bool Disabled;

            [EnumToggleButtons]
            public CopyType CopyProjectType;
            
            [ShowIf("@CopyProjectType == CopyType.FilePath")]
            [Sirenix.OdinInspector.FilePath(RequireExistingPath = true)]
            [ValidateInput(nameof(ValidateFilePath))]
            [DetailedInfoBox("Source Format Keys", "$SourceInfo")]
            public string FilePath;
            
            [ShowIf("@CopyProjectType == CopyType.DirectoryPath")]
            [FolderPath(RequireExistingPath = true)]
            [ValidateInput(nameof(ValidateDirectoryPath))]
            [DetailedInfoBox("Source Format Keys", "$SourceInfo")]
            public string DirectoryPath;

            [ShowIf("@CopyProjectType == CopyType.DirectoryPath")]
            public string DirectorySearchPattern;
            
            [ShowIf("@CopyProjectType == CopyType.DirectoryPath")]
            public string[] DirectoryExcludeExtensions;
            
            [ShowIf("@CopyProjectType == CopyType.Object")]
            [ValidateInput(nameof(ValidateObject))]
            public Object SourceObject;
            
            public bool OverwriteIfExists;

            [Button]
            public void DefaultDestinationPath() => DestinationPath = "{OutputPath}/{FileName}";
            
            [DetailedInfoBox("Destination Format Keys", "$DestinationInfo")]
            public string DestinationPath;

            bool ValidateFilePath(string path) => CopyProjectType != CopyType.FilePath || File.Exists(FormatSourcePath(path));

            bool ValidateDirectoryPath(string path) => CopyProjectType != CopyType.DirectoryPath || Directory.Exists(FormatSourcePath(path));

            bool ValidateObject(Object obj) => CopyProjectType != CopyType.Object || obj != null;

            public string FormatSourcePath(string path) {
                path = path.Replace(@"{StreamingAssets}", Application.streamingAssetsPath);
                path = path.Replace(@"{DataPath}", Application.dataPath);
                path = path.Replace(@"{PersistentDataPath}", Application.persistentDataPath);
                return path.Replace('\\', '/');
            }

            public string FormatDestinationPath(string path, string outputPath) {
                var outputDir = Directory.GetParent(outputPath);
                path = path.Replace(@"{OutputPath}", outputDir.ToString());
                var dataPath = Path.Combine(outputDir.ToString(), $"{Path.GetFileNameWithoutExtension(outputPath)}_Data");
                path = path.Replace(@"{OutputPathData}", dataPath);
                path = path.Replace(@"{StreamingAssets}", Path.Combine(dataPath, "StreamingAssets/"));
                path = path.Replace(@"{Ext}", Path.GetExtension(FileName));
                path = path.Replace(@"{File}", Name);
                path = path.Replace(@"{FileName}", FileName);
                return path.Replace('\\', '/');
            }

            public string Name {
                get {
                    switch (CopyProjectType) {
                        case CopyType.FilePath:
                            return FilePath;
                        case CopyType.DirectoryPath:
                            return DirectoryPath;
                        case CopyType.Object:
                            if (SourceObject == null) return "Null";
                            return SourceObject.name;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            public string FileName {
                get {
                    switch (CopyProjectType) {
                        case CopyType.FilePath:
                            try {
                                return Path.GetFileName(FilePath);
                            } catch {
                                return "InvalidFilePath";
                            }
                        case CopyType.DirectoryPath:
                            try {
                                return Path.GetDirectoryName(DirectoryPath);
                            } catch {
                                return "InvalidDirectoryPath";
                            }
                        case CopyType.Object:
                            if (SourceObject == null) return "Null";
                            return Path.GetFileName(AssetDatabase.GetAssetPath(SourceObject));
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        
        public List<CopyItem> CopyList;

        public int callbackOrder { get; }
        public void OnPostprocessBuild(BuildReport report) {
            Output(report.summary.outputPath, Instance);
        }

        [BoxGroup("Debug")]
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true, Extensions = ".exe")]
        [ShowInInspector]
        private string copyFilesOutputPath;

        [Button]
        [BoxGroup("Debug")]
        public static void Output() => Output(Instance.copyFilesOutputPath, Instance);
        
        public static void Output(string outputPath, CopyProjectFilesOnBuild cp) {
            Debug.Log($"[{nameof(CopyProjectFilesOnBuild)}]");
            for (var i = 0; i < cp.CopyList.Count; i++) {
                var copyItem = cp.CopyList[i];
                if (copyItem.Disabled) continue;
                try {
                    switch (copyItem.CopyProjectType) {
                        case CopyItem.CopyType.Object:
                            var sourceProjectPath = AssetDatabase.GetAssetPath(copyItem.SourceObject);
                            cp.CopyFile(copyItem, sourceProjectPath, copyItem.DestinationPath, outputPath);
                            break;
                        case CopyItem.CopyType.FilePath:
                            cp.CopyFile(copyItem, copyItem.FilePath, copyItem.DestinationPath, outputPath);
                            break;
                        case CopyItem.CopyType.DirectoryPath:
                            cp.CopyFiles(copyItem, copyItem.DirectoryPath, copyItem.DestinationPath, outputPath, string.IsNullOrWhiteSpace(copyItem.DirectorySearchPattern) ? "*" : copyItem.DirectorySearchPattern);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } catch (Exception ex) {
                    Debug.Log($"[{nameof(CopyProjectFilesOnBuild)}] Failed to copy [{i}] {copyItem.Name}, reason: {ex.ToString()}");
                }
            }
        }

        public void CopyFile(CopyItem copyItem, string source, string dest, string outputPath) {
            source = copyItem.FormatSourcePath(source);
            source = Path.GetFullPath(source);
            dest = copyItem.FormatDestinationPath(dest, outputPath);
            
            // If the output path is a directory assume they want to just drop the file in here
            if (Directory.Exists(dest)) {
                Debug.Log($"[{nameof(CopyProjectFilesOnBuild)}] {dest} is directory so automatically appending the filename");
                dest += $"{Path.GetFileName(source)}";
            }

            // Create the directory structure to the output path, if the output path is a file make sure to get the parent directory (To avoid creating a folder with the file name)
            Directory.CreateDirectory(!Directory.Exists(dest) ? Directory.GetParent(dest).ToString() : dest);

            // Follow overwrite rule
            if (!copyItem.OverwriteIfExists && File.Exists(dest)) {
                Debug.Log($"[{nameof(CopyProjectFilesOnBuild)}] File {copyItem.Name} already exists and set to no overwrite, skipping...");
                return;
            }

            Debug.Log($"[{nameof(CopyProjectFilesOnBuild)}] Copying {source} => {dest}");
            /*if (EditorUtility.DisplayDialog("ExecuteCopy?", $"Copy {source} => {dest}", "Yes", "No"))*/
            File.Copy(source, dest, copyItem.OverwriteIfExists);
        }

        public void CopyFiles(CopyItem copyItem, string source, string dest, string outputPath, string searchPattern = "*") {
            dest = copyItem.FormatDestinationPath(dest, outputPath);
            Directory.CreateDirectory(dest);
            // If the output is a file then get the parent
            //if (File.Exists(dest)) dest = Directory.GetParent(dest).ToString();
            foreach (var file in Directory.GetFiles(source, searchPattern)) {
                var skip = false;
                // If the file ends with any of the extensions then skip
                foreach (var excludeExtension in copyItem.DirectoryExcludeExtensions)
                    if (file.EndsWith(excludeExtension, StringComparison.InvariantCultureIgnoreCase))
                        skip = true;
                if (skip) continue;

                CopyFile(copyItem, file, dest, outputPath);
            }
        }
    }
    
    public class CopyProjectFilesOnBuildWindow : OdinScriptableObjectEditorWindow<CopyProjectFilesOnBuild> {
        [MenuItem("RyanQuagliata/Copy Project Files On Build")]
        public static void OpenWindow() => GetWindow<CopyProjectFilesOnBuildWindow>();
    }
    
    public abstract class OdinScriptableObjectEditorWindow<T> : OdinEditorWindow where T : GlobalConfig<T>, new() {
        private Vector2 instanceScrollPos;
        private UnityEditor.Editor editor;

        protected override void OnEnable() {
            base.OnEnable();
            editor = UnityEditor.Editor.CreateEditor(GlobalConfig<T>.Instance);
        }

        protected override void OnGUI() {
            base.OnGUI();
            instanceScrollPos = EditorGUILayout.BeginScrollView(instanceScrollPos);
            GUIHelper.PushHierarchyMode(false);
            editor.OnInspectorGUI();
            GUIHelper.PopHierarchyMode();
            EditorGUILayout.EndScrollView();

            this.RepaintIfRequested();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            DestroyImmediate(editor);
        }
    }
}