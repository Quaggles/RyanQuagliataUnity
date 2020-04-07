#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Video;

namespace RyanQuagliataUnity.UI {
    [CustomEditor(typeof(VideoPlayerGraphic), true)]
    [CanEditMultipleObjects]
    public class VideoPlayerGraphicEditor : Editor {
        private SerializedProperty videoPlayerProperty;
        private SerializedProperty setNativeResolutionOnAwakeProperty;
        private SerializedProperty colorProperty;
        private SerializedProperty textureProperty;

        protected void OnEnable() {
            videoPlayerProperty = serializedObject.FindProperty(nameof(VideoPlayerGraphic.VideoPlayer));
            setNativeResolutionOnAwakeProperty = serializedObject.FindProperty(nameof(VideoPlayerGraphic.SetNativeSizeOnAwake));
            colorProperty = serializedObject.FindProperty("m_Color");
            textureProperty = serializedObject.FindProperty("m_Texture");
        }

        public override void OnInspectorGUI() {
            var videoPlayerGraphic = (VideoPlayerGraphic) target;
            //base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(videoPlayerProperty);
            EditorGUILayout.PropertyField(setNativeResolutionOnAwakeProperty);
            colorProperty.colorValue = EditorGUILayout.ColorField(new GUIContent(colorProperty.displayName), colorProperty.colorValue, true, false, false);
            if (GUILayout.Button("Set Native Size")) videoPlayerGraphic.SetNativeSize();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(textureProperty);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif