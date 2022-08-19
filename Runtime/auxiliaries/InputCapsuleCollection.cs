using UnityEngine;
using Cobilas.Collections;

namespace Cobilas.Unity.Management.InputManager {
    [CreateAssetMenu(fileName = "new InputCapsuleCollection", menuName = "CobilasInputManager/Input Capsule Collection")]
    public class InputCapsuleCollection : ScriptableObject {
        [SerializeField] private string[] capsules;
#if UNITY_EDITOR
        [SerializeField] private bool[] foldout_list;
#endif

        public InputCapsule[] Capsules {
            get {
                InputCapsule[] res = new InputCapsule[ArrayManipulation.ArrayLength(capsules)];
                for (int I = 0; I < res.Length; I++)
                    res[I] = InputCapsule.CloneInputCapsule(InputCapsuleJson.JsonToInputCapsuleJson(capsules[I]));
                return res;
            }
        }
    }
}