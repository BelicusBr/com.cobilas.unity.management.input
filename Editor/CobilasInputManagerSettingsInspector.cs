using UnityEditor;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CustomEditor(typeof (CobilasInputManagerSettings))]
    public class CobilasInputManagerSettingsInspector : UnityEditor.Editor {
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
            EditorGUILayout.ToggleLeft("Use multiple keys", p_useMultipleKeys.boolValue, EditorStyles.boldLabel);
            EditorGUILayout.ToggleLeft("Use secondary command keys", p_useSecondaryCommandKeys.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
