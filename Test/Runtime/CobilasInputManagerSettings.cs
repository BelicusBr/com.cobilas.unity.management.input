using UnityEngine;

namespace Cobilas.Unity.Management.InputManager {
    public class CobilasInputManagerSettings : ScriptableObject {
        [SerializeField] private bool useMultipleKeys;
        [SerializeField] private bool useSecondaryCommandKeys;

        public bool UseMultipleKeys => useMultipleKeys;
        public bool UseSecondaryCommandKeys => useSecondaryCommandKeys;

        internal void SetSettings() {
            useMultipleKeys = CobilasInputManager.UseMultipleKeys;
            useSecondaryCommandKeys = CobilasInputManager.UseSecondaryCommandKeys;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public static CobilasInputManagerSettings GetCobilasInputManagerSettings() {
            CobilasInputManagerSettings settings = CreateInstance<CobilasInputManagerSettings>();
            settings.name = "cim_settings";
            settings.useMultipleKeys = CobilasInputManager.UseMultipleKeys;
            settings.useSecondaryCommandKeys = CobilasInputManager.UseSecondaryCommandKeys;
            return settings;
        }
    }
}