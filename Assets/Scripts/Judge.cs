using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {}     
    public void judge(UserGUI userGUI, GUIStyle MyStyle, GUIStyle MyButtonStyle) {
        if (userGUI.if_win_or_not == -1) {
			GUI.Label (new Rect (Screen.width/2-Screen.width/8, 50, 100, 50), "Game Over", MyStyle);
            if (GUI.Button (new Rect (Screen.width/2-Screen.width/8, Screen.height/2+100, 150, 50), "Restart", MyButtonStyle)) {
			    userGUI.if_win_or_not = 0;
			    userGUI.action.restart ();
		    }
        }
        else if (userGUI.if_win_or_not == 1) {
			GUI.Label (new Rect (Screen.width/2-Screen.width/8, 50, 100, 50), "You Win", MyStyle);
            if (GUI.Button (new Rect (Screen.width/2-Screen.width/8, Screen.height/2+100, 150, 50), "Restart", MyButtonStyle)) {
			    userGUI.if_win_or_not = 0;
			    userGUI.action.restart ();
		    }
		}
    }
}
