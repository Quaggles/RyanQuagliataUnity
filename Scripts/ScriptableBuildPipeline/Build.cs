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

        public static void Build() => BuildTo(GetConfig(), GetBuildFolder());

        public static BuildConfiguration GetConfig() => AssetDatabase.LoadAssetAtPath<BuildConfiguration>(CommandLineArguments.ReadArgValue("-BUILD_CONFIGURATION"));
        public static string GetBuildFolder() => CommandLineArguments.ReadArgValue("-BUILD_FOLDER");

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
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            
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
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) { /*Ignore*/ }
            
            try {
                var version = CommandLineArguments.ReadArgValue("-BUILD_NUMBER");
                PlayerSettings.bundleVersion = version;
                
                if (buildTarget == BuildTarget.iOS || Application.platform == RuntimePlatform.OSXPlayer)
                    PlayerSettings.iOS.buildNumber = version;

                if (buildTarget == BuildTarget.Android) {
                    var match = new Regex(@"^(\d{1})\.(\d{1})\.(\d{1,3})$").Match(version);
                    if (!match.Success) throw new InvalidOperationException($"BUILD_NUMBER {version} is not in the format #{{1}}.#{{1}}.#{{1-3}} for conversion to Android bundleVersionCode");
                    int bundleVersionCode = 0;
                    bundleVersionCode += int.Parse(match.Groups[1].Value) * 10000;
                    bundleVersionCode += int.Parse(match.Groups[2].Value) * 1000;
                    bundleVersionCode += int.Parse(match.Groups[3].Value);
                    PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
                }
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) { /*Ignore*/ }
            
            var additionalil2CppArgs = PlayerSettings.GetAdditionalIl2CppArgs();
            try {
                if (CommandLineArguments.IsArgumentPresent("-BUILD_SCRIPTING_IL2CPP_DISABLE_JUMP_THREADING_OPTIMISER")){
                    PlayerSettings.SetAdditionalIl2CppArgs(additionalil2CppArgs + " --compiler-flags=-d2ssa-cfg-jt-");
                    Debug.Log($"[{typeof(ScriptableBuildPipeline)}] PlayerSettings.additionalIl2CppArgs = {PlayerSettings.GetAdditionalIl2CppArgs()}");
                }
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) { /*Ignore*/ }
            
            var applicationIdentifier = PlayerSettings.applicationIdentifier;
            try {
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, CommandLineArguments.ReadArgValue("-APPLICATION_IDENTIFIER"));
                Debug.Log($"[{typeof(ScriptableBuildPipeline)}] PlayerSettings.applicationIdentifier = {PlayerSettings.GetApplicationIdentifier(buildTargetGroup)}");
            } catch (CommandLineArguments.CommandLineArgumentNotFoundException) { /*Ignore*/ }
            
            var outputBuildDirectory = buildConfiguration.GetComponentOrDefault<OutputBuildDirectory>();
            outputBuildDirectory.OutputDirectory = path;
            buildConfiguration.RemoveComponent<OutputBuildDirectory>();
            buildConfiguration.SetComponent(outputBuildDirectory);
            Debug.Log($"[{typeof(ScriptableBuildPipeline)}] {typeof(OutputBuildDirectory)} OutputBuildDirectory = {path}");

            try {
                Debug.Log($"[{nameof(ScriptableBuildPipeline)}] Building to {buildConfiguration.GetOutputBuildDirectory()}");
                buildConfiguration.Build();
                Debug.Log($"[{nameof(ScriptableBuildPipeline)}] Completed building to {buildConfiguration.GetOutputBuildDirectory()}");
                buildConfiguration.RestoreAsset();
            } finally {
                // Restore settings
                PlayerSettings.graphicsJobs = graphicsJobs;
                PlayerSettings.SetAdditionalIl2CppArgs(additionalil2CppArgs);
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, applicationIdentifier);
            }
        }
    }
}