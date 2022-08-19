using System;
using UnityEditor;
using UnityEngine;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    [CustomEditor(typeof(InputCapsule))]
    public class InputCapsuleInspector : UnityEditor.Editor {
        private InputCapsuleInspectorDrawer drawer;

        private void OnEnable() {
            drawer = new InputCapsuleInspectorDrawer(Repaint, serializedObject);
        }

        private void OnDisable() {
            if (drawer != null) {
                drawer.Dispose();
                drawer = null;
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            drawer.DrawStatus();
            drawer.DrawProperties();
            drawer.DrawLists();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
