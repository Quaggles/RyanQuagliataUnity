using System;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Unity.Build;
using Unity.Build.Classic;
using Unity.Build.Common;
using UnityEditor;
using UnityEngine;

namespace RyanQuagliataUnity.ScriptableBuildPipeline {
    public class ScriptableBuildPipeline {
        [Button]
        public static void BuildTo(string buildConfigurationPath, string path) {
            var config = AssetDatabase.LoadAssetAtPath<BuildConfiguration>(buildConfigurationPath);
            if (config == null) throw new NullReferenceException($"No asset of type {nameof(BuildConfiguration)} found at {buildConfigurationPath}");
            BuildTo(config, path);
        }

        public static void Build() {
            var buildConfiguration = CommandLineArguments.ReadArgValue("-BUILD_CONFIGURATION");
            var buildFolder = CommandLineArguments.ReadArgValue("-BUILD_FOLDER");
            BuildTo(buildConfiguration, buildFolder);
        }

        protected static bool SetBuildComponentValue<T> (BuildConfiguration buildConfiguration, string argName, Action<T, string> action) where T : IBuildComponent {
            try {
                var argument = CommandLineArguments.ReadArgValue(argName);
                var component = buildConfiguration.GetComponentOrDefault<T>();
                action.Invoke(component, argument);
                
                // Component must be removed and then set to apply changes
                buildConfiguration.RemoveComponent<T>();
                buildConfiguration.SetComponent(component);
                Debug.Log($"[{typeof(ScriptableBuildPipeline)}] {typeof(T)} {argName} = {argument}");
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                return false;
            }

            return true;
        }

        [Button]
        public static void BuildTo(BuildConfiguration buildConfiguration, string path) {
            /*
             * Debug
             * Develop
             * Release
             */
            void ProfileConfiguration(ClassicBuildProfile profile, string s) => profile.Configuration = (BuildType) Enum.Parse(typeof(BuildType), s, true);
            SetBuildComponentValue<ClassicBuildProfile>(buildConfiguration, "-BUILD_PROFILE_CONFIGURATION", ProfileConfiguration);

            /* 
             * Mono2x
             * IL2CPP
             * WinRTDotNET
             */
            void ScriptingBackend(ClassicScriptingSettings profile, string s) => profile.ScriptingBackend = (ScriptingImplementation) Enum.Parse(typeof(ScriptingImplementation), s, true);
            SetBuildComponentValue<ClassicScriptingSettings>(buildConfiguration, "-BUILD_SCRIPTING_BACKEND", ScriptingBackend);

            /*
             * Debug
             * Release
             * Master
             */
            void Il2CppCompilerConfiguration(ClassicScriptingSettings profile, string s) => profile.Il2CppCompilerConfiguration = (Il2CppCompilerConfiguration) Enum.Parse(typeof(Il2CppCompilerConfiguration), s, true);;
            SetBuildComponentValue<ClassicScriptingSettings>(buildConfiguration, "-BUILD_SCRIPTING_IL2CPP_CONFIGURATION", Il2CppCompilerConfiguration);

            var graphicsJobs = PlayerSettings.graphicsJobs;
            try {
                PlayerSettings.graphicsJobs = bool.Parse(CommandLineArguments.ReadArgValue("-GRAPHICS_JOBS"));
                Debug.Log($"[{typeof(ScriptableBuildPipeline)}] PlayerSettings.graphicsJobs = {PlayerSettings.graphicsJobs}");
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                // Ignore
            }
            
            try {
                var version = CommandLineArguments.ReadArgValue("-BUILD_NUMBER");
                PlayerSettings.bundleVersion = version;
                PlayerSettings.iOS.buildNumber = version;
                var match = new Regex(@"^(\d+)\.(\d+)\.(\d+)$").Match(version);
                if (!match.Success) throw new InvalidOperationException($"BUILD_NUMBER is not in the format #.#.#");
                int bundleVersionCode = 0;
                bundleVersionCode += int.Parse(match.Groups[1].Value) * 10000;
                bundleVersionCode += int.Parse(match.Groups[2].Value) * 100;
                bundleVersionCode += int.Parse(match.Groups[3].Value);
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                // Ignore
            }
            
            var additionalil2CppArgs = PlayerSettings.GetAdditionalIl2CppArgs();
            try {
                if (CommandLineArguments.IsArgumentPresent("-BUILD_SCRIPTING_IL2CPP_DISABLE_JUMP_THREADING_OPTIMISER")){
                    PlayerSettings.SetAdditionalIl2CppArgs(additionalil2CppArgs + " --compiler-flags=-d2ssa-cfg-jt-");
                    Debug.Log($"[{typeof(ScriptableBuildPipeline)}] PlayerSettings.additionalIl2CppArgs = {PlayerSettings.GetAdditionalIl2CppArgs()}");
                }
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
                // Ignore
            }
            
            var outputBuildDirectory = buildConfiguration.GetComponentOrDefault<OutputBuildDirectory>();
            outputBuildDirectory.OutputDirectory = path;
            buildConfiguration.RemoveComponent<OutputBuildDirectory>();
            buildConfiguration.SetComponent(outputBuildDirectory);
            Debug.Log($"[{typeof(ScriptableBuildPipeline)}] {typeof(OutputBuildDirectory)} OutputBuildDirectory = {path}");
            
            Debug.Log($"[{nameof(ScriptableBuildPipeline)}] Building to {buildConfiguration.GetOutputBuildDirectory()}");
            buildConfiguration.Build();
            Debug.Log($"[{nameof(ScriptableBuildPipeline)}] Completed building to {buildConfiguration.GetOutputBuildDirectory()}");
            buildConfiguration.RestoreAsset();

            // Restore settings
            PlayerSettings.graphicsJobs = graphicsJobs;
            PlayerSettings.SetAdditionalIl2CppArgs(additionalil2CppArgs);
        }
    }
}