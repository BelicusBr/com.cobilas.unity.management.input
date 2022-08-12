using UnityEngine;
using Cobilas.Unity.Management.InputManager;

public class cim_TDS001 : MonoBehaviour {
    private void Start() 
        => Debug.LogAssertionFormat("InputCaps: {0}", CobilasInputManager.InputCapsuleCount);
    
    private void Update() {
        string InputID1 = "ID_TDS0";
        if (CobilasInputManager.ButtonPressed(InputID1))
          Debug.LogAssertionFormat("Press:{0}", InputID1);
        InputID1 = "ID_TDS1";
        if (CobilasInputManager.ButtonPressedDown(InputID1))
            Debug.LogAssertionFormat("Press:{0}", InputID1);
        InputID1 = "ID_TDS2";
        if (CobilasInputManager.ButtonPressedUp(InputID1))
            Debug.LogAssertionFormat("Press:{0}", InputID1);
    }
}
