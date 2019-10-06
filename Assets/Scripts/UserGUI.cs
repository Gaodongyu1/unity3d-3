using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Engine;

public class UserGUI : MonoBehaviour {
	public UserAction action;
	private GUIStyle MyStyle;
	private GUIStyle MyButtonStyle;
	public int if_win_or_not;
	Judge judger;

	void Start(){
		action = Director.get_Instance ().curren as UserAction;

		MyStyle = new GUIStyle ();
		MyStyle.fontSize = 40;
		MyStyle.normal.textColor = new Color (255f, 0, 0);
		MyStyle.alignment = TextAnchor.MiddleCenter;

		MyButtonStyle = new GUIStyle ("button");
		MyButtonStyle.fontSize = 30;

		judger = new Judge();
	}
	void OnGUI(){
		judger.judge(this,MyStyle,MyButtonStyle);
	}
}
