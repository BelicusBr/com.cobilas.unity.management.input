using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cobilas.Unity.Management.InputManager {
    public class CobilasInputManagerSettings : ScriptableObject {
        [SerializeField]
        private bool useMultipleKeys;
        [SerializeField]
        private bool useSecondaryCommandKeys;

        public bool UseMultipleKeys => useMultipleKeys;
        public bool UseSecondaryCommandKeys => useSecondaryCommandKeys;

        internal void SetSettings() {
            useMultipleKeys = CobilasInputManager.UseMultipleKeys;
            useSecondaryCommandKeys = CobilasInputManager.UseSecondaryCommandKeys;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public static CobilasInputManagerSettings GetCobilasInputManagerSettings() {
            CobilasInputManagerSettings instance = CreateInstance<CobilasInputManagerSettings>();
            instance.name = "cim_settings";
            instance.useMultipleKeys = CobilasInputManager.UseMultipleKeys;
            instance.useSecondaryCommandKeys = CobilasInputManager.UseSecondaryCommandKeys;
            return instance;
        }
    }
}
