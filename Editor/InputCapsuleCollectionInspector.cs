using System;
using UnityEditor;
using UnityEngine;
using Cobilas.Collections;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CustomEditor(typeof(InputCapsuleCollection))]
    public class InputCapsuleCollectionInspector : UnityEditor.Editor {

        private SerializedInputCapsule[] s_capsules;
        private SerializedProperty p_foldout_list;
        private SerializedProperty p_capsules;
        private InputCapsule[] f_capsules;
        private string[] fs_capsules;
        private bool[] f_foldout_list;

        private void OnEnable() {
            p_capsules = serializedObject.FindProperty("capsules");
            p_foldout_list = serializedObject.FindProperty("foldout_list");
            fs_capsules = p_capsules.GetValue<string[]>();
            f_foldout_list = p_foldout_list.GetValue<bool[]>();

            if (fs_capsules == null) fs_capsules = new string[0];
            if (f_foldout_list == null) f_foldout_list = new bool[0];

            f_capsules = JsonToInputCapsule(fs_capsules);

            s_capsules = new SerializedInputCapsule[f_capsules.Length];

            for (int I = 0; I < s_capsules.Length; I++)
                s_capsules[I] = new SerializedInputCapsule(Repaint, f_capsules[I]);
        }

        private void OnDisable()
            => DeserializeInputCapsuleList();

        private InputCapsule[] JsonToInputCapsule(string[] list) {
            InputCapsule[] res = new InputCapsule[list.Length];
            for (int I = 0; I < res.Length; I++)
                res[I] = InputCapsuleJson.Editor_CloneInputCapsule(list[I]);
            return res;
        }

        private string[] InputCapsuleToJson(InputCapsule[] list) {
            string[] res = new string[list.Length];
            for (int I = 0; I < res.Length; I++)
                res[I] = JsonUtility.ToJson(list[I]);
            return res;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawStatus();
            EditorGUILayout.EndVertical();
            EditorGUI.BeginChangeCheck();
            int size = EditorGUILayout.IntField("Size list", f_capsules.Length);
            if (EditorGUI.EndChangeCheck()) {
                Resize(size);
                p_capsules.SetValue(InputCapsuleToJson(f_capsules));
                p_foldout_list.SetValue(f_foldout_list);
            }

            for (int I = 0; I < size; I++) {
                EditorGUI.BeginChangeCheck();
                if (f_foldout_list[I] = EditorGUILayout.Foldout(f_foldout_list[I], f_capsules[I].DisplayName)) {
                    s_capsules[I].OnGUI();
                }
                if (EditorGUI.EndChangeCheck()) {
                    p_capsules.SetValue(InputCapsuleToJson(f_capsules));
                    p_foldout_list.SetValue(f_foldout_list);
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void Resize(int size) {
            size = size < 0 ? 0 : size;
            Array.Resize(ref f_capsules, size);
            Array.Resize(ref f_foldout_list, size);
            DeserializeInputCapsuleList();
            Array.Resize(ref s_capsules, size);

            for (int I = 0; I < size; I++) {
                if (f_capsules[I] == null)
                    s_capsules[I] = new SerializedInputCapsule(Repaint, f_capsules[I] = CreateInstance<InputCapsule>());
                else s_capsules[I] = new SerializedInputCapsule(Repaint, f_capsules[I]);
            }
        }

        private void DrawStatus() {
            EditorGUILayout.LabelField(TitleProperty.tt_UseSecondaryCommandKeys, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(TitleProperty.GetUseSecondaryCommandKeysValue());
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(TitleProperty.tt_UseUseMultipleKeys, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(TitleProperty.GetUseUseMultipleKeysValue());
            EditorGUI.indentLevel--;
        }

        private void DeserializeInputCapsuleList() {
            for (int I = 0; I < ArrayManipulation.ArrayLength(s_capsules); I++)
                s_capsules[I].Dispose();
        }

        private sealed class SerializedInputCapsule : IDisposable {
            private SerializedObject serialized;
            private InputCapsuleInspectorDrawer drawer;

            public SerializedInputCapsule(Action repaint, InputCapsule capsule) {
                serialized = new SerializedObject(capsule);
                drawer = new InputCapsuleInspectorDrawer(repaint, serialized);
            }

            public void OnGUI() {
                serialized.Update();
                drawer.DrawProperties();
                drawer.DrawLists();
                serialized.ApplyModifiedProperties();
            }

            public void Dispose() {
                serialized.Dispose();
                serialized = null;
                drawer.Dispose();
                drawer = null;
            }
        }
    }
}
