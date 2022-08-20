using Cobilas;
using UnityEngine;
using Cobilas.Unity.Management.InputManager;

public class cmi_TDS001 : MonoBehaviour {

    private Interrupter _interrupter;
    public string txt;

    private void Start() {
        _interrupter = new Interrupter(3, true);
        Debug.Log(txt = string.Format("InputCaps: {0}", CobilasInputManager.InputCapsuleCount));
    }
    
    private void Update() {
        string InputID1 = "ID_MoveDown";
        if (CobilasInputManager.ButtonPressed(InputID1)) {
            //_interrupter[0] = true;
            Debug.Log(string.Format("Press:{0}", InputID1));
        }
        InputID1 = "ID_MoveUp";
        if (CobilasInputManager.ButtonPressed(InputID1)) {
            //_interrupter[1] = true;
            Debug.Log(string.Format("Press:{0}", InputID1));
        }
        InputID1 = "ID_MoveLeft";
        if (CobilasInputManager.ButtonPressed(InputID1)) {
            //_interrupter[2] = true;
            Debug.Log(string.Format("Press:{0}", InputID1));
        }
        InputID1 = "ID_MoveRight";
        if (CobilasInputManager.ButtonPressed(InputID1)) {
            //_interrupter[2] = true;
            Debug.Log(string.Format("Press:{0}", InputID1));
        }
    }

    private void OnGUI() {
        //Rect rect = new Rect(0f, 0f, 130f, 25f);
        //GUI.Label(new Rect(130f, 0f, 330f, 25f), txt);
        //if (GUI.Button(new Rect(460f, 0f, 130f, 25f), "Sair"))
        //    Application.Quit();
        //_ = GUI.Toggle(rect, _interrupter[0], "KP_0");
        //rect.y += 25f;
        //_ = GUI.Toggle(rect, _interrupter[1], "KP_1");
        //rect.y += 25f;
        //_ = GUI.Toggle(rect, _interrupter[2], "KP_2");
    }
}
