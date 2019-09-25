using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Engine;

public class UserGUI : MonoBehaviour {
	private UserAction action;
	private GUIStyle MyStyle;
	private GUIStyle MyButtonStyle;
	public int if_win_or_not;
	public int show;

	void Start(){
		action = Director.get_Instance ().curren as UserAction;

		MyStyle = new GUIStyle ();
		MyStyle.fontSize = 40;
		MyStyle.normal.textColor = new Color (255f, 0, 0);
		MyStyle.alignment = TextAnchor.MiddleCenter;

		MyButtonStyle = new GUIStyle ("button");
		MyButtonStyle.fontSize = 30;
	}
	void reStart(){
		if (GUI.Button (new Rect (Screen.width/2-Screen.width/8, Screen.height/2+100, 150, 50), "Restart", MyButtonStyle)) {
			if_win_or_not = 0;
			action.restart ();
		}
	}
	void IsPause(){
		if (GUI.Button (new Rect (Screen.width / 2 - 350, Screen.height / 2 + 100, 150, 50), "Pause", MyButtonStyle)) {
			if (Director.cn_move == 0) {
				action.pause ();
				Director.cn_move = 1;
			} 
		} else if (GUI.Button (new Rect (Screen.width-Screen.width/2 + 150, Screen.height / 2 + 100, 150, 50), "Continue", MyButtonStyle)) {
			if (Director.cn_move == 1) {
				action.Coninu();
				Director.cn_move = 0;
			}
		}
	}
	void JudgeIfShow(){
		if (GUI.Button (new Rect (530, 500, 100, 30), "Guide", MyButtonStyle)) {
			if (show == 1)
				show = 0;
			else
				show = 1;
		}
	}
	void IfShow(){
		if (show == 1) {
			GUI.Label(new Rect(Screen.width / 2 - 85, 10, 200, 50), "让全部牧师和恶魔都渡河", MyStyle);
			GUI.Label(new Rect(Screen.width / 2 - 120, 50, 250, 50), "每一边恶魔数量都不能多于牧师数量", MyStyle);
			GUI.Label (new Rect (Screen.width / 2 - 85, 90, 250, 50), "点击牧师、恶魔、船移动, 方块为牧师, 球体为恶魔", MyStyle);
		}
	}
	void OnGUI(){
		IsPause ();
		reStart ();
		JudgeIfShow ();
		IfShow ();
		if(Director.cn_move == 1)
			GUI.Label (new Rect (Screen.width/2-Screen.width/8, 50, 100, 50), "Pausing", MyStyle);
		if (if_win_or_not == -1) {
			GUI.Label (new Rect (Screen.width/2-Screen.width/8, 50, 100, 50), "Game Over", MyStyle);
			IsPause ();
			reStart ();
		} else if (if_win_or_not == 1) {
			GUI.Label (new Rect (Screen.width/2-Screen.width/8, 50, 100, 50), "You Win", MyStyle);
			IsPause ();
			reStart ();
		}
	}
}