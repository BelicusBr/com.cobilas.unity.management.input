using UnityEditor;
using Cobilas.Unity.Management.InputManager;
using UEEditor = UnityEditor.Editor;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CustomEditor(typeof(CobilasInputManagerSettings))]
    public class CobilasInputManagerSettingsInspector : UEEditor {
        private SerializedProperty p_useMultipleKeys;
        private SerializedProperty p_useSecondaryCommandKeys;

        private void OnEnable() {
            p_useMultipleKeys = serializedObject.FindProperty("useMultipleKeys");
            p_useSecondaryCommandKeys = serializedObject.FindProperty("useSecondaryCommandKeys");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.indentLevel++;
            _ = EditorGUILayout.ToggleLeft("Use multiple keys", p_useMultipleKeys.boolValue, EditorStyles.boldLabel);
            _ = EditorGUILayout.ToggleLeft("Use secondary command keys", p_useSecondaryCommandKeys.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}