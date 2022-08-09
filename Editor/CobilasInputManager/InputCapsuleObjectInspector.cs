using UnityEditor;
using UnityEngine;
using Cobilas.Unity.Management.InputManager;
using UEEditor = UnityEditor.Editor;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CustomEditor(typeof(InputCapsuleObject))]
    public class InputCapsuleObjectInspector : UEEditor {

        private SerializedProperty p_inputType;
        private SerializedProperty p_inputName;
        private SerializedProperty p_inputID;
        private SerializedProperty p_isHidden;
        private SerializedProperty p_isFixedInput;
        private InputValueInfoList r_inputMain;
        private InputValueInfoList r_secondaryInput;
        private TitleProperty titles = new TitleProperty();
        private Vector2 scrollView;

        private void OnEnable() {
            SerializedProperty p_input = serializedObject.FindProperty("input");
            p_inputType = p_input.FindPropertyRelative("inputType");
            p_inputName = p_input.FindPropertyRelative("inputName");
            p_inputID = p_input.FindPropertyRelative("inputID");
            p_isHidden = p_input.FindPropertyRelative("isHidden");
            p_isFixedInput = p_input.FindPropertyRelative("isFixedInput");

            r_inputMain = new InputValueInfoList(this, titles.tt_InputMain, titles, p_input.FindPropertyRelative("inputMain"));
            r_secondaryInput = new InputValueInfoList(this, titles.tt_SecondaryInput, titles, p_input.FindPropertyRelative("secondaryInput"));
        }

        private void OnDisable() {
            p_inputType.Dispose();
            p_inputName.Dispose();
            p_inputID.Dispose();
            p_isHidden.Dispose();
            p_isFixedInput.Dispose();
            r_inputMain.Dispose();
            r_secondaryInput.Dispose();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            scrollView = EditorGUILayout.BeginScrollView(scrollView);

            EditorGUILayout.LabelField(titles.tt_UseSecondaryCommandKeys, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(titles.GetUseSecondaryCommandKeysValue());
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(titles.mk_Property, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(p_inputType, titles.tt_InputType);
            EditorGUILayout.PropertyField(p_inputName, titles.tt_InputName);
            EditorGUILayout.PropertyField(p_inputID, titles.tt_InputID);
            EditorGUILayout.PropertyField(p_isHidden, titles.tt_IsHidden);
            EditorGUILayout.PropertyField(p_isFixedInput, titles.tt_IsFixedInput);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            r_inputMain.DrawList();
            if (CobilasInputManager.UseSecondaryCommandKeys)
                r_secondaryInput.DrawList();
            
            EditorGUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();
        }

        public sealed class TitleProperty {
            private GUIContent mk_property;
            private GUIContent tt_inputType;
            private GUIContent tt_inputName;
            private GUIContent tt_inputID;
            private GUIContent tt_isHidden;
            private GUIContent tt_isFixedInput;
            private GUIContent tt_inputMain;
            private GUIContent tt_secondaryInput;
            private GUIContent tt_useSecondaryCommandKeys;
            private GUIContent tt_myKey;
            private GUIContent tt_displayName;
            private GUIContent tt_pressType;
            private const string txt_activated = "Activated";
            private const string txt_disabled = "Disabled";

            private GUIContent vv_useSecondaryCommandKeys;

            public GUIContent mk_Property => mk_property;
            public GUIContent tt_InputType => tt_inputType;
            public GUIContent tt_InputName => tt_inputName;
            public GUIContent tt_InputID => tt_inputID;
            public GUIContent tt_IsHidden => tt_isHidden;
            public GUIContent tt_IsFixedInput => tt_isFixedInput;
            public GUIContent tt_InputMain => tt_inputMain;
            public GUIContent tt_SecondaryInput => tt_secondaryInput;
            public GUIContent tt_UseSecondaryCommandKeys => tt_useSecondaryCommandKeys;
            public GUIContent tt_MyKey => tt_myKey;
            public GUIContent tt_DisplayName => tt_displayName;
            public GUIContent tt_PressType => tt_pressType;

            public TitleProperty() {
                vv_useSecondaryCommandKeys = new GUIContent();
                mk_property = new GUIContent("Properties");
                tt_inputType = new GUIContent("Input type");
                tt_inputName = new GUIContent("Input name");
                tt_inputID = new GUIContent("Input ID");
                tt_isHidden = new GUIContent("Is hidden");
                tt_isFixedInput = new GUIContent("Is fixed input");
                tt_inputMain = new GUIContent("Input main");
                tt_secondaryInput = new GUIContent("Secondary input");
                tt_useSecondaryCommandKeys = new GUIContent("Use secondary command keys");
                tt_myKey = new GUIContent("My key");
                tt_displayName = new GUIContent("Display name");
                tt_pressType = new GUIContent("Press type");
            }

            public GUIContent GetUseSecondaryCommandKeysValue() {
                vv_useSecondaryCommandKeys.text = CobilasInputManager.UseSecondaryCommandKeys ? txt_activated : txt_disabled;
                return vv_useSecondaryCommandKeys;
            }
        }
    }
}