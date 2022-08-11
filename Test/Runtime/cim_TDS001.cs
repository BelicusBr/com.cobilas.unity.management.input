using System.Collections;
using UnityEngine;
using Cobilas.Unity.Management.InputManager;

public class cim_TDS001 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log($"InputCaps: {CobilasInputManager.InputCapsuleCount}");
    }

    // Update is called once per frame
    void Update()
    {
        string ID = "ID_TDS0";
        if (CobilasInputManager.ButtonPressed(ID))
            Debug.Log($"Press:{ID}");
        ID = "ID_TDS1";
        if (CobilasInputManager.ButtonPressedDown(ID))
            Debug.Log($"Press:{ID}");
        ID = "ID_TDS2";
        if (CobilasInputManager.ButtonPressedUp(ID))
            Debug.Log($"Press:{ID}");
    }
}