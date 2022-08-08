using System;
using System.Xml;
using UnityEngine;
using Cobilas.Collections;
using Cobilas.Unity.Management.InputManager;

namespace Cobilas.Unity.Editor.Management.InputManager {
    using cimFlag = ConvertCobilasInputManager.cimFlag;
    using KeyPressType = CobilasInputManager.KeyPressType;
    using InputManagerType = CobilasInputManager.InputManagerType;
    public static class ConvertCobilasInputManagerEditor {

        public static InputCapsuleInfo[] AssembleInputCapsuleList(ElementTag tags, InputCapsuleInfo[] inputs) {
            tags.ForEach(new Action<ElementTag>((t) => {
                switch (t.GetElementAttribute("flag").Value.ValueToString) {
                    case "init":
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

        public static void AssembleInputCapsuleConfigs(out ElementTag tag, InputCapsuleInfo[] inputs) {
            ElementTag element = new ElementTag("cim", new ElementAttribute("version", CobilasInputManager.cimVersion));
            AddCIMInit(element, inputs);
            AddCIMDefault(element, inputs, cimFlag._default);
            AddCIMDefault(element, inputs, cimFlag._fixed);
            tag = element;
        }

        public static void AssembleInputCapsuleCustom(out ElementTag tag, InputCapsuleInfo[] inputs) {
            ElementTag element = new ElementTag("cim", new ElementAttribute("version", CobilasInputManager.cimVersion));
            AddCIMDefault(element, inputs, cimFlag._custom);
            tag = element;
        }

        private static InputCapsuleInfo[] GetCIMInit(ElementTag tag) {
            InputCapsuleInfo[] res = null;

            tag.ForEach(new Action<ElementTag>((t) => {
                switch (t.Name) {
                    case "UseSecondaryCommandKeys":
                        CobilasInputManager.UseSecondaryCommandKeys = t.GetElementAttribute("status").Value.ValueToBool;
                        break;
                    case "InputCapsuleInfo":
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
                        ArrayManipulation.Add(new InputCapsuleInfo(InputName, InputID, Hidden, FixedInput, type), ref res);
                        break;
                }
            }));

            return res;
        }

        private static InputCapsuleInfo[] SetInputFuncs(ElementTag tag, InputCapsuleInfo[] inputs) {
            tag.ForEach(new Action<ElementTag>((t) => {
                InputCapsuleInfo input = null;
                t.ForEach(new Action<ElementTag>((st) => {
                    switch (st.Name) {
                        case "InputID":
                            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++)
                                if (inputs[I].inputID == st.Value.ValueToString) {
                                    input = inputs[I];
                                    break;
                                }
                            break;
                        case "InputMain" when input != null:
                            InputValueInfo[] inputs1 = null;
                            st.ForEach(new Action<ElementTag>((imst) => {
                                if (imst.Name != "Empty") {
                                    ArrayManipulation.Add(
                                        new InputValueInfo(
                                            (KeyCode)imst.GetElementAttribute("key").Value.ValueToInt,
                                            (KeyPressType)imst.GetElementAttribute("pressType").Value.ValueToInt,
                                            imst.GetElementAttribute("displayName").Value.ValueToString
                                            ), ref inputs1);
                                }
                            }));
                            input.inputMain = inputs1;
                            break;
                        case "SecondaryMain" when input != null:
                            inputs1 = null;
                            st.ForEach(new Action<ElementTag>((imst) => {
                                if (imst.Name != "Empty") {
                                    ArrayManipulation.Add(
                                        new InputValueInfo(
                                            (KeyCode)imst.GetElementAttribute("key").Value.ValueToInt,
                                            (KeyPressType)imst.GetElementAttribute("pressType").Value.ValueToInt,
                                            imst.GetElementAttribute("displayName").Value.ValueToString
                                            ), ref inputs1);
                                }
                            }));
                            input.secondaryInput = inputs1;
                            break;
                    }
                }));
            }));
            return inputs;
        }

        private static void AddCIMInit(ElementTag tag, InputCapsuleInfo[] inputs) {
            tag.Add(tag = new ElementTag("cim",
                new ElementAttribute("flag", "init"),
                    new ElementTag("UseSecondaryCommandKeys",
                        new ElementAttribute("status", CobilasInputManager.UseSecondaryCommandKeys)
                    )
                ));

            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++) {
                tag.Add(new ElementTag("InputCapsuleInfo",
                            new ElementTag("InputName", inputs[I].inputName),
                            new ElementTag("InputID", inputs[I].inputID),
                            new ElementTag("InputType", (int)inputs[I].inputType),
                            new ElementTag("InputStatus",
                                new ElementAttribute("Hidden", inputs[I].isHidden),
                                new ElementAttribute("FixedInput", inputs[I].isFixedInput)
                                )
                            )
                    );
            }
        }

        private static void AddCIMDefault(ElementTag tag, InputCapsuleInfo[] inputs, cimFlag flag) {
            tag.Add(tag = new ElementTag("cim",
                new ElementAttribute("flag", flag.ToString().Replace("_", ""))
                ));

            for (int I = 0; I < ArrayManipulation.ArrayLength(inputs); I++) {
                if (((flag == cimFlag._default || flag == cimFlag._custom) && inputs[I].isFixedInput) ||
                    (flag == cimFlag._fixed && !inputs[I].isFixedInput)) continue;

                tag.Add(tag = new ElementTag("InputCapsuleInfo"));
                tag.Add(new ElementTag("InputID", inputs[I].inputID));
                if (inputs[I].InputMainCount > 0) {
                    tag.Add(tag = new ElementTag("InputMain"));
                    for (int J = 0; J < inputs[I].InputMainCount; J++) {
                        tag.Add(new ElementTag("Input",
                                new ElementAttribute("key", (int)inputs[I].inputMain[J].myKey),
                                new ElementAttribute("pressType", (int)inputs[I].inputMain[J].pressType),
                                new ElementAttribute("displayName", inputs[I].inputMain[J].displayName)
                            ));
                    }
                    tag = tag.Parent;
                } else tag.Add(tag = new ElementTag("Empty"));

                if (inputs[I].SecondaryInputCount > 0) {
                    tag.Add(tag = new ElementTag("SecondaryMain"));
                    for (int J = 0; J < inputs[I].SecondaryInputCount; J++) {
                        tag.Add(new ElementTag("Input",
                                new ElementAttribute("key", (int)inputs[I].secondaryInput[J].myKey),
                                new ElementAttribute("pressType", (int)inputs[I].secondaryInput[J].pressType),
                                new ElementAttribute("displayName", inputs[I].secondaryInput[J].displayName)
                            ));
                    }
                    tag = tag.Parent;
                } else tag.Add(new ElementTag("Empty"));
                tag = tag.Parent;
            }
        }

    }
}
