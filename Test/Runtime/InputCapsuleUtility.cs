using System;
using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
    public static class InputCapsuleUtility {
        public const string DN_None = "DNNone";
        public const string Button_Left = "Button Left";
        public const string Button_Right = "Button Right";
        public const string Button_Mid = "Button Mid";
        public const string Trigger_3 = "Trigger 3";
        public const string Trigger_4 = "Trigger 4";
        public const string Trigger_5 = "Trigger 5";
        public const string Trigger_6 = "Trigger 6";
        public const string Keypad0 = "KP:0";
        public const string Keypad1 = "KP:1";
        public const string Keypad2 = "KP:2";
        public const string Keypad3 = "KP:3";
        public const string Keypad4 = "KP:4";
        public const string Keypad5 = "KP:5";
        public const string Keypad6 = "KP:6";
        public const string Keypad7 = "KP:7";
        public const string Keypad8 = "KP:8";
        public const string Keypad9 = "KP:9";
        public const string KeypadPlus = "KP:+";
        public const string KeypadMinus = "KP:-";
        public const string KeypadEnter = "Enter";
        public const string KeypadDivide = "KP:/";
        public const string KeypadMultiply = "KP:*";

        public const string Alert = "Alert";
        public const string SingleQuote = "'";
        public const string DoubleQuote = "\"";
        public const string FormFeed = "FormFeed";
        public const string HorizontalTab = "Tab";
        public const string BackSlash = "BackSlash";
        public const string Backspace = "Backspace";
        public const string VerticalTab = "VerticalTab";

        public static string MouseButtonToDisplayName(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.Mouse0: return Button_Left;
                case KeyCode.Mouse1: return Button_Right;
                case KeyCode.Mouse2: return Button_Mid;
                case KeyCode.Mouse3: return Trigger_3;
                case KeyCode.Mouse4: return Trigger_4;
                case KeyCode.Mouse5: return Trigger_5;
                case KeyCode.Mouse6: return Trigger_6;
                default: return DN_None;
            }
        }

        public static string CharacterToDisplayName(char character) {
            switch (character.InterpretEscapeSequence()) {
                case EscapeSequence.Alert: return Alert;
                case EscapeSequence.FormFeed: return FormFeed;
                case EscapeSequence.BackSlash: return BackSlash;
                case EscapeSequence.Backspace: return Backspace;
                case EscapeSequence.SingleQuote: return SingleQuote;
                case EscapeSequence.DoubleQuote: return DoubleQuote;
                case EscapeSequence.VerticalTab: return VerticalTab;
                case EscapeSequence.HorizontalTab: return HorizontalTab;
                default:return DN_None;
            }
        }

        public static string KeyPadToDisplayName(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.Keypad0: return Keypad0;
                case KeyCode.Keypad1: return Keypad1;
                case KeyCode.Keypad2: return Keypad2;
                case KeyCode.Keypad3: return Keypad3;
                case KeyCode.Keypad4: return Keypad4;
                case KeyCode.Keypad5: return Keypad5;
                case KeyCode.Keypad6: return Keypad6;
                case KeyCode.Keypad7: return Keypad7;
                case KeyCode.Keypad8: return Keypad8;
                case KeyCode.Keypad9: return Keypad9;
                case KeyCode.KeypadPlus: return KeypadPlus;
                case KeyCode.KeypadEnter: return KeypadEnter;
                case KeyCode.KeypadMinus: return KeypadMinus;
                case KeyCode.KeypadDivide: return KeypadDivide;
                case KeyCode.KeypadMultiply: return KeypadMultiply;
                default: return DN_None;
            }
        }

        public static void ChangeTriggerInInputCapsule(InputCapsule capsule, KeyPressType pressType, bool SecondaryTrigger) {
            InputCapsuleTrigger[] triggers = GetKeyInput.GetInputCapsuleTriggers();
            if (!ArrayManipulation.EmpytArray(triggers)) {
                if (!capsule.IsFixedInput) capsule.SetIsChange(true);
                triggers[triggers.Length - 1].SetKeyPressType(pressType);
                if (SecondaryTrigger) SetSecondaryTrigger(capsule, triggers);
                else SetTriggerFirst(capsule, triggers);
            }
        }

        internal static void SetTriggerFirst(InputCapsule capsule, params InputCapsuleTrigger[] triggers) {
            if (!capsule.IsFixedInput) 
                capsule.SetTriggerFirst(triggers);
        }

        internal static void SetSecondaryTrigger(InputCapsule capsule, params InputCapsuleTrigger[] triggers) {
            if (!capsule.IsFixedInput)
                capsule.SetSecondaryTrigger(triggers);
        }
    }
}
