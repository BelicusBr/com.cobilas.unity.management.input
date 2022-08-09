using System;
using System.Xml;
using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
using KeyPressType = CobilasInputManager.KeyPressType;
using InputManagerType = CobilasInputManager.InputManagerType;
#if UNITY_EDITOR
    public static class ConvertCobilasInputManager {
        public enum cimFlag {
            _default = 0,
            _fixed = 1,
            _custom = 2
        }
#else
    internal static class ConvertCobilasInputManager {
        internal enum cimFlag {
            _default = 0,
            _fixed = 1,
            _custom = 2
        }
#endif

        public static InputCapsule[] AssembleInputCapsuleList(ElementTag tags, InputCapsule[] inputs, bool reset) {

            tags.ForEach(new Action<ElementTag>((t) => {
                switch (t.GetElementAttribute("flag").Value.ValueToString) {
                    case "init" when !reset:
                        inputs = GetCIMInit(t);
                        break;
                    case "default":
                        inputs = SetInputFuncs(t, inputs);
                        break;
                    case "fixed":
                        inputs = SetInputFuncs(t, inputs);
                        break;
                    case "custom":
                        inputs = SetInputFuncs(t, inputs);
                        break;
                }
            }));
            return inputs;
        }

        public static void AssembleInputCapsuleConfigs(out ElementTag tag, InputCapsule[] inputs) {
            ElementTag element = new ElementTag("cim", new ElementAttribute("version", CobilasInputManager.cimVersion));
            AddCIMInit(element, inputs);
            AddCIMDefault(element, inputs, cimFlag._default);
            AddCIMDefault(element, inputs, cimFlag._fixed);
            tag = element;
        }

        public static void AssembleInputCapsuleCustom(out ElementTag tag, InputCapsule[] inputs) {
            ElementTag element = new ElementTag("cim", new ElementAttribute("version", CobilasInputManager.cimVersion));
            AddCIMDefault(element, inputs, cimFlag._custom);
            tag = element;
        }

        private static InputCapsule[] GetCIMInit(ElementTag tag) {
            InputCapsule[] res = null;

            tag.ForEach(new Action<ElementTag>((t) => {
                switch (t.Name) {
                    case "UseSecondaryCommandKeys":
                        CobilasInputManager.UseSecondaryCommandKeys = t.GetElementAttribute("status").Value.ValueToBool;
                        break;
                    case "InputCapsule":
                        string InputName = "";
                        string InputID = "";
                        InputManagerType type = InputManagerType.MixedCommand;
                        bool Hidden = false;
                        bool FixedInput = false;
                        t.ForEach((st) => {
                            switch (st.Name) {
                                case "InputName":
                                    InputName = st.Value.ValueToString;
                                    break;
                                case "InputID":
                                    InputID = st.Value.ValueToString;
                                    break;
                                case "InputType":
                                    type = (InputManagerType)st.Value.ValueToInt;
                                    break;
                                case "InputStatus":
                                    Hidden = st.GetElementAttribute(nameof(Hidden)).Value.ValueToBool;
                                    FixedInput = st.GetElementAttribute(nameof(FixedInput)).Value.ValueToBool;
                                    break;
                            }
                        });
                        ArrayManipulation.Add(new InputCapsule(InputName, InputID, Hidden, FixedInput, type), ref res);
                        break;
                }
            }));

            return res;
        }

        private static InputCapsule[] SetInputFuncs(ElementTag tag, InputCapsule[] inputs) {
            tag.ForEach(new Action<ElementTag>((t) => {
                InputCapsule input = null;
                t.ForEach(new Action<ElementTag>((st) => {
                    switch (st.Name) {
                        case "InputID":
                            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++)
                                if (inputs[I].InputID == st.Value.ValueToString) {
                                    input = inputs[I];
                                    break;
                                }
                            break;
                        case "InputMain" when input != null:
                            InputValue[] inputs1 = null;
                            st.ForEach(new Action<ElementTag>((imst) => {
                                if (imst.Name != "Empty") {
                                    ArrayManipulation.Add(
                                        new InputValue(
                                            (KeyCode)imst.GetElementAttribute("key").Value.ValueToInt,
                                            (KeyPressType)imst.GetElementAttribute("pressType").Value.ValueToInt,
                                            imst.GetElementAttribute("displayName").Value.ValueToString
                                            ),ref inputs1);
                                }
                            }));
                            input.SetInputMain(inputs1);
                            break;
                        case "SecondaryMain" when input != null:
                            inputs1 = null;
                            st.ForEach(new Action<ElementTag>((imst) => {
                                if (imst.Name != "Empty") {
                                    ArrayManipulation.Add(
                                        new InputValue(
                                            (KeyCode)imst.GetElementAttribute("key").Value.ValueToInt,
                                            (KeyPressType)imst.GetElementAttribute("pressType").Value.ValueToInt,
                                            imst.GetElementAttribute("displayName").Value.ValueToString
                                            ), ref inputs1);
                                }
                            }));
                            input.SetSecondaryInput(inputs1);
                            break;
                    }
                }));
            }));
            return inputs;
        }

        private static void AddCIMInit(ElementTag tag, InputCapsule[] inputs) {
            tag.Add(tag = new ElementTag("cim",
                new ElementAttribute("flag", "init"),
                    new ElementTag("UseSecondaryCommandKeys",
                        new ElementAttribute("status", CobilasInputManager.UseSecondaryCommandKeys)
                    )
                ));

            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++) {
                tag.Add(new ElementTag("InputCapsule", 
                            new ElementTag("InputName", inputs[I].InputName),
                            new ElementTag("InputID", inputs[I].InputID),
                            new ElementTag("InputType", (int)inputs[I].InputType),
                            new ElementTag("InputStatus",
                                new ElementAttribute("Hidden", inputs[I].IsHidden),
                                new ElementAttribute("FixedInput", inputs[I].IsFixedInput)
                                )
                            )
                    );
            }
        }

        private static void AddCIMDefault(ElementTag tag, InputCapsule[] inputs, cimFlag flag) {
            tag.Add(tag = new ElementTag("cim",
                new ElementAttribute("flag", flag.ToString().Replace("_", ""))
                ));

            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++) {
                if (((flag == cimFlag._default || flag == cimFlag._custom) && inputs[I].IsFixedInput) ||
                    (flag == cimFlag._fixed && !inputs[I].IsFixedInput)) continue;

                tag.Add(tag = new ElementTag("InputCapsule"));
                tag.Add(new ElementTag("InputID", inputs[I].InputID));
                if (inputs[I].InputMainCount > 0) {
                    tag.Add(tag = new ElementTag("InputMain"));
                    for (int J = 0; J < inputs[I].InputMainCount; J++) {
                        tag.Add(new ElementTag("Input",
                                new ElementAttribute("key", (int)inputs[I].InputMain[J].MyKey),
                                new ElementAttribute("pressType", (int)inputs[I].InputMain[J].PressType),
                                new ElementAttribute("displayName", inputs[I].InputMain[J].DisplayName)
                            ));
                    }
                    tag = tag.Parent;
                } else tag.Add(tag = new ElementTag("Empty"));

                if (inputs[I].SecondaryInputCount > 0) {
                    tag.Add(tag = new ElementTag("SecondaryMain"));
                    for (int J = 0; J < inputs[I].SecondaryInputCount; J++) {
                        tag.Add(new ElementTag("Input",
                                new ElementAttribute("key", (int)inputs[I].SecondaryInput[J].MyKey),
                                new ElementAttribute("pressType", (int)inputs[I].SecondaryInput[J].PressType),
                                new ElementAttribute("displayName", inputs[I].SecondaryInput[J].DisplayName)
                            ));
                    }
                    tag = tag.Parent;
                } else tag.Add(new ElementTag("Empty"));
                tag = tag.Parent;
            }
        }
    }
}
