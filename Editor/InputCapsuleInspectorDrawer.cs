using System;
using UnityEditor;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public sealed class InputCapsuleInspectorDrawer : IDisposable {

        private SerializedProperty p_inputType;
        private SerializedProperty p_inputName;
        private SerializedProperty p_inputID;
        private SerializedProperty p_isHidden;
        private SerializedProperty p_isFixedInput;
        private InputValueInfoList r_inputMain;
        private InputValueInfoList r_secondaryInput;

        public InputCapsuleInspectorDrawer(Action repaint, SerializedObject serializedObject) {
            p_inputType = serializedObject.FindProperty("inputType");
            p_inputName = serializedObject.FindProperty("displayName");
            p_inputID = serializedObject.FindProperty("_ID");
            p_isHidden = serializedObject.FindProperty("isHidden");
            p_isFixedInput = serializedObject.FindProperty("isFixedInput");
            r_inputMain = new InputValueInfoList(repaint, TitleProperty.tt_InputMain, serializedObject, serializedObject.FindProperty("triggerFirst"));
            r_secondaryInput = new InputValueInfoList(repaint, TitleProperty.tt_SecondaryInput, serializedObject, serializedObject.FindProperty("secondaryTrigger"));
        }

        public void DrawStatus() {
            EditorGUILayout.LabelField(TitleProperty.tt_UseSecondaryCommandKeys, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(TitleProperty.GetUseSecondaryCommandKeysValue());
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(TitleProperty.tt_UseUseMultipleKeys, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(TitleProperty.GetUseUseMultipleKeysValue());
            EditorGUI.indentLevel--;
        }

        public void DrawProperties() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(TitleProperty.mk_Property, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(p_inputType, TitleProperty.tt_InputType);
            EditorGUILayout.PropertyField(p_inputName, TitleProperty.tt_InputName);
            EditorGUILayout.PropertyField(p_inputID, TitleProperty.tt_InputID);
            EditorGUILayout.PropertyField(p_isHidden, TitleProperty.tt_IsHidden);
            EditorGUILayout.PropertyField(p_isFixedInput, TitleProperty.tt_IsFixedInput);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        public void DrawLists() {
            r_inputMain.DrawList();
            EditorGUI.BeginDisabledGroup(!CobilasInputManager.UseSecondaryCommandKeys);
            r_secondaryInput.DrawList();
            EditorGUI.EndDisabledGroup();
        }

        public void Dispose() {
            p_inputType =
            p_inputName =
            p_inputID =
            p_isHidden =
            p_isFixedInput = null;
            if (r_inputMain != null)
                r_inputMain.Dispose();
            if (r_secondaryInput != null)
                r_secondaryInput.Dispose();
            r_inputMain =
            r_secondaryInput = null;
        }
    }
}
