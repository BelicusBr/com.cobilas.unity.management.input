using UnityEngine;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    public static class TitleProperty {
        private static GUIContent mk_property;
        private static GUIContent tt_inputType;
        private static GUIContent tt_inputName;
        private static GUIContent tt_inputID;
        private static GUIContent tt_isHidden;
        private static GUIContent tt_isFixedInput;
        private static GUIContent tt_inputMain;
        private static GUIContent tt_secondaryInput;
        private static GUIContent tt_useSecondaryCommandKeys;
        private static GUIContent tt_useUseMultipleKeys;
        private static GUIContent tt_myKey;
        private static GUIContent tt_displayName;
        private static GUIContent tt_pressType;
        private const string txt_activated = "Activated";
        private const string txt_disabled = "Disabled";
        private static GUIContent vv_useSecondaryCommandKeys;

        public static GUIContent mk_Property => mk_property;
        public static GUIContent tt_InputType => tt_inputType;
        public static GUIContent tt_InputName => tt_inputName;
        public static GUIContent tt_InputID => tt_inputID;
        public static GUIContent tt_IsHidden => tt_isHidden;
        public static GUIContent tt_IsFixedInput => tt_isFixedInput;
        public static GUIContent tt_InputMain => tt_inputMain;
        public static GUIContent tt_SecondaryInput => tt_secondaryInput;
        public static GUIContent tt_UseSecondaryCommandKeys => tt_useSecondaryCommandKeys;
        public static GUIContent tt_UseUseMultipleKeys => tt_useUseMultipleKeys;
        public static GUIContent tt_MyKey => tt_myKey;
        public static GUIContent tt_DisplayName => tt_displayName;
        public static GUIContent tt_PressType => tt_pressType;

        static TitleProperty() {
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
            tt_useUseMultipleKeys = new GUIContent("Use multiple keys");
        }

        public static GUIContent GetUseUseMultipleKeysValue() {
            vv_useSecondaryCommandKeys.text = CobilasInputManager.UseMultipleKeys ? txt_activated : txt_disabled;
            return vv_useSecondaryCommandKeys;
        }

        public static GUIContent GetUseSecondaryCommandKeysValue() {
            vv_useSecondaryCommandKeys.text = CobilasInputManager.UseSecondaryCommandKeys ? txt_activated : txt_disabled;
            return vv_useSecondaryCommandKeys;
        }
    }
}