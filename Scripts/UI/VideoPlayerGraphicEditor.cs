
using RyanQuagliataUnity.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts {
    [CustomEditor(typeof(VideoPlayerGraphic), true)]
    [CanEditMultipleObjects]
    public class VideoPlayerGraphicEditor : Editor {
        private SerializedProperty videoPlayerProperty;
        private SerializedProperty colorProperty;
        private SerializedProperty textureProperty;
        
        protected void OnEnable() {
            videoPlayerProperty = serializedObject.FindProperty("VideoPlayer");
            colorProperty = serializedObject.FindProperty("m_Color");
            textureProperty = serializedObject.FindProperty("m_Texture");
        }
        public override void OnInspectorGUI() {
            var videoPlayerGraphic = (VideoPlayerGraphic)target;
            //base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(videoPlayerProperty);
            colorProperty.colorValue = EditorGUILayout.ColorField(new GUIContent(colorProperty.displayName), colorProperty.colorValue, true, false, false);
            Debug.Log(colorProperty.colorValue);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(textureProperty);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif