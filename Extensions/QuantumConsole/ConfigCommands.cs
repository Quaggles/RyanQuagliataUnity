using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QFSW.QC;
using UnityEngine;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
    [CommandPrefix("Config")]
    public static class ConfigCommands {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("RyanQuagliata/Config/OpenAutoExec")]
        private static void OpenAutoExec() {
            var path = Path.Combine(Application.streamingAssetsPath, AUTOEXEC_NAME);
            if (!File.Exists(path)) {
                if (UnityEditor.EditorUtility.DisplayDialog("", $"{path} does not exist, do you want to create it?", "Yes", "No"))
                    File.WriteAllText(path, "// Any commands here will be executed every time a new scene loads, full list of commands in \"ConsoleCommandList.txt\"\n");
                else
                    return;
            }
            Application.OpenURL(path);
        }
        
        [UnityEditor.MenuItem("RyanQuagliata/Config/DeleteAutoExec")]
        private static void DeleteAutoExec() {
            var path = Path.Combine(Application.streamingAssetsPath, AUTOEXEC_NAME);
            if (File.Exists(path)) {
                if (UnityEditor.EditorUtility.DisplayDialog("", $"Are you sure you want to delete {path}?", "Yes", "No"))
                    File.Delete(path);
            } else {
                UnityEditor.EditorUtility.DisplayDialog("", $"{path} does not exist", "Ok");
            }
        }

        [UnityEditor.MenuItem("RyanQuagliata/Config/OpenStreamingAssets")]
        private static void OpenStreamingAssets() => Application.OpenURL(Path.Combine(Application.streamingAssetsPath));
#endif
        private static readonly Regex commentRegex = new Regex(@" *//.*", RegexOptions.Compiled);
        public const string AUTOEXEC_NAME = "autoexec.cfg";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void BeforeScene() {
            // If the first scene contains an autoexec bootstrap it can handle the autoexec
            if (Object.FindObjectOfType<AutoExecBootstrap>()) return;
            InvokeAutoExec();
        }

        [Command]
        public static void InvokeAutoExec() {
            QuantumConsoleProcessor.GenerateCommandTable(); // Ensures the command table is generated
            try {
                InvokeConfigStreamingAssets(AUTOEXEC_NAME);
            } catch (FileNotFoundException) {
                // Ignore
            } catch (DirectoryNotFoundException) {
                // Ignore
            }
            try {
                InvokeConfig(Path.Combine(Directory.GetParent(Application.dataPath).ToString(), AUTOEXEC_NAME));
            } catch (FileNotFoundException) {
                // Ignore
            } catch (DirectoryNotFoundException) {
                // Ignore
            }

            try {
                var customExecPath = CommandLineArguments.ReadArgValue("-ExecConfig");
                customExecPath = customExecPath.Replace("{StreamingAssets}", Application.streamingAssetsPath);
                customExecPath = customExecPath.Replace("{DataPath}", Application.dataPath);
                customExecPath = customExecPath.Replace("{Root}", Directory.GetParent(Application.dataPath).ToString());
                InvokeConfig(customExecPath);
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                // Ignore
            } catch (Exception ex) {
                Debug.LogError(ex.ToString());
            }
            
            try {
                foreach (var command in CommandLineArguments.ReadArgValues("-Exec")) {
                    try {
                        QuantumConsoleProcessor.InvokeCommand(command.Trim('"'));
                    } catch (Exception ex) {
                        Debug.LogError(ex.ToString());
                    }
                }
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                // Ignore
            } catch (Exception ex) {
                Debug.LogError(ex.ToString());
            }
        }

        [Command]
        public static void InvokeConfigStreamingAssets(string relativePath) => InvokeConfig(Path.Combine(Application.streamingAssetsPath, relativePath));

        [Command]
        public static void InvokeConfig(string filePath) {
            Debug.Log($"Executing config, {(File.Exists(filePath) ? "<color=green><b>Found</b></color>" : "<color=red><b>Not found</b></color>")} at: {filePath}");
            var lines = File.ReadAllLines(filePath);
            // Parse to remove comments
            for (var i = 0; i < lines.Length; i++) {
                var match = commentRegex.Match(lines[i]);
                lines[i] = lines[i].Remove(match.Index, match.Length);
            }

            InvokeCommands(lines);
        }


        [Command]
        public static void ReadAutoExec() => ReadConfigStreamingAssets(AUTOEXEC_NAME);

        [Command]
        public static void ReadConfigStreamingAssets(string relativePath) => ReadConfigFile(Path.Combine(Application.streamingAssetsPath, relativePath));

        [Command]
        public static void ReadConfigFile(string filePath) {
            var lines = File.ReadAllLines(filePath);
            // Add colours to comments
            for (var i = 0; i < lines.Length; i++) {
                var match = commentRegex.Match(lines[i]);
                var start = "<color=green>";
                var end = "</color>";
                lines[i] = $"[{i}] {lines[i].Insert(match.Index, start).Insert(match.Index + match.Length + start.Length, end)}";
            }

            Debug.Log(string.Join("\n", Enumerable.Prepend(lines, $"Reading {lines.Length} lines from {filePath}")));
        }

        [Command]
        public static void InvokeCommands(IEnumerable<string> commands) {
            var console = QFSW.QC.QuantumConsole.Instance ? QFSW.QC.QuantumConsole.Instance : Object.FindObjectOfType<QFSW.QC.QuantumConsole>();
            int count = 0;
            foreach (var command in commands) {
                try {
                    if (string.IsNullOrWhiteSpace(command)) continue;
                    console.InvokeCommand(command.Trim());
                } catch (Exception ex) {
                    Debug.LogError($"[{count}] {command} failed, reason: {ex}");
                } finally {
                    count++;
                }
            }
        }

        public static IEnumerable<CommandData> GetOrderedCommands() => QuantumConsoleProcessor.GetAllCommands().OrderBy(x => x.CommandName).ThenBy(x => x.ParamCount);

        public static string GetCommandList(bool forceReload) {
            QuantumConsoleProcessor.GenerateCommandTable(false, forceReload);

            try {
                var sb = new StringBuilder();
                foreach (var command in GetOrderedCommands()) {
                    // Name line
                    sb.Append(command.CommandName);
                    sb.Append(' ');
                    sb.Append(command.IsStatic ? "[Static]" : "[Instance]");
                    if (!command.IsStatic) {
                        sb.Append(' ');
                        sb.Append($"[{command.MonoTarget.ToString()}]");
                    }
                    sb.AppendLine();

                    // Description Line
                    if (command.HasDescription) {
                        sb.Append('\t');
                        sb.Append("description: ");
                        var desc = command.CommandDescription;
                        // If it's a multiline description put it on a new line
                        if (desc.Contains('\n')) {
                            desc = desc.Insert(0, "\n");
                            // Tab any new lines to match the current indent
                            desc = desc.Replace("\n", $"\n\t\t");
                        }

                        sb.AppendLine(desc);
                    }
                    
                    // Signature Line
                    if (command.ParamCount > 0) {
                        sb.Append("\tsignature: ");
                        var paramNames = command.ParameterSignature.Split(' ');
                        var paramTypes = command.ParamTypes.Select(x => x.Name).ToArray();
                        for (var i = 0; i < paramNames.Length; i++) {
                            sb.Append($"<{paramTypes[i]}> {paramNames[i]}");
                            if (i < paramNames.Length - 1) sb.Append($", ");
                        }

                        sb.AppendLine();
                    }

                    // Return Line
                    try {
                        if (command.MethodData.ReturnType != typeof(void)) {
                            sb.Append("\treturns: ");
                            sb.Append($"<{command.MethodData.ReturnType.Name}>");
                            sb.AppendLine();
                        }
                    } catch (NotImplementedException) {
                        // Ignore
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            } finally {
                QuantumConsoleProcessor.ClearCommandTable();
            }
        }
    }
    
#if UNITY_EDITOR
    public class ExportCommandList : UnityEditor.Build.IPostprocessBuildWithReport {
        public int callbackOrder { get; }
        
        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report) => Export(report.summary.outputPath);

        public static void Export(string path) {
            var name = Path.GetFileNameWithoutExtension(path);
            var buildDirectory = Directory.GetParent(path).ToString();
            var exportLocation = Path.Combine(buildDirectory, "ConsoleCommandList.txt");
            File.WriteAllText(exportLocation,  ConfigCommands.GetCommandList(true));
        }
    }
#endif
}