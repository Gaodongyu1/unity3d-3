using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Engine;

public class MySceneController : MonoBehaviour, SceneController, UserAction{
	readonly Vector3 water_pos = new Vector3 (0, 0.5f, 0);
	UserGUI user;
	public CoastController fromCoast;
	public CoastController toCoast;
	public BoatController boat;
	private List<MyCharacterController> team;
	public MySceneActionManager actionManager;

	void Awake(){
		Director director = Director.get_Instance ();
		director.curren = this;
		user = gameObject.AddComponent<UserGUI> () as UserGUI;
		team = new List<MyCharacterController>();
		loadResources ();
	}
	public void loadResources(){
		GameObject water = Instantiate (Resources.Load ("Prefabs/Water", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
		water.name = "water";

		fromCoast = new CoastController ("from");
		toCoast = new CoastController ("to");
		boat = new BoatController ();

		for (int i = 0; i < 3; i++) {
			MyCharacterController tem = new MyCharacterController ("priest");
			tem.setName ("priest" + i);
			tem.setPosition (fromCoast.getEmptyPosition ());
			tem.getOnCoast (fromCoast);
			fromCoast.getOnCoast (tem);
			team.Add (tem);
		}
		for (int i = 0; i < 3; i++) {
			MyCharacterController tem = new MyCharacterController ("devil");
			tem.setName ("devil" + i);
			tem.setPosition (fromCoast.getEmptyPosition ());
			tem.getOnCoast (fromCoast);
			fromCoast.getOnCoast (tem);
			team.Add (tem);
		}
	}
	public void moveboat(){
		if (Director.cn_move == 1)
			return;
		if (boat.IfEmpty ())
			return;
		actionManager.moveBoat(boat.getGameObject(),boat.boatMove(),boat.move_speed);
		//检查游戏是否结束了
		user.if_win_or_not = checkGameOver();
	}
	public void isClickChar (MyCharacterController tem_char){
		if (Director.cn_move == 1)
			return;
		if (tem_char._isOnBoat ()) {
			CoastController tem_coast;
			if (boat.getTFflag () == -1) {
				tem_coast = toCoast;
			} else {
				tem_coast = fromCoast;
			}
			boat.getOffBoat (tem_char.getName ());

			Vector3 end_pos = tem_coast.getEmptyPosition ();                                       
			Vector3 middle_pos = new Vector3(tem_char.getGameObject().transform.position.x, end_pos.y, end_pos.z); 
			actionManager.moveRole(tem_char.getGameObject(), middle_pos, end_pos, 50); 

			tem_char.getOnCoast (tem_coast);
			tem_coast.getOnCoast (tem_char);
		} 
        else {
			CoastController tem_coast2 = tem_char.getCoastController ();
			if (boat.getEmptyIndex () == -1)
				return;
			if (boat.getTFflag () != tem_coast2.getTFflag ())
				return;
			tem_coast2.getOffCoast (tem_char.getName());

			Vector3 end_pos = boat.getEmptyPosition ();                                           
			Vector3 middle_pos = new Vector3(end_pos.x, tem_char.getGameObject().transform.position.y, end_pos.z);
			actionManager.moveRole(tem_char.getGameObject(), middle_pos, end_pos, 50); 


			tem_char.getOnBoat (boat);
			boat.getOnBoat (tem_char);
		}
		//检查游戏是否结束
		user.if_win_or_not = checkGameOver();
	}
	public void restart(){
		Director.cn_move = 0;
		boat.reset ();
		fromCoast.reset ();
		toCoast.reset ();
		foreach (MyCharacterController i in team) {
			i.reset ();
		}
	}
	public void pause(){
		boat.Mypause ();
		foreach (MyCharacterController i in team) {
			i.Mypause();
		}
	}
	public void Coninu (){
		boat.MyConti ();
		foreach (MyCharacterController i in team) {
			i.MyConti();
		}
	}
	private int checkGameOver(){
		if (Director.cn_move == 1)
			return 0;
		int from_priest = 0;
		int from_devil = 0;
		int to_priest = 0;
		int to_devil = 0;

		int[] from_count = fromCoast.getCharacterNum ();
		from_priest = from_count [0];
		from_devil = from_count [1];

		int[] to_count = toCoast.getCharacterNum ();
		to_priest = to_count [0];
		to_devil = to_count [1];

		if (to_devil + to_priest == 6)
			return 1;//you win
		int[] boat_count = boat.getCharacterNum();
		if (boat.getTFflag () == 1) {
			from_priest += boat_count [0];
			from_devil += boat_count [1];
		} else {
			to_priest += boat_count [0];
			to_devil += boat_count [1];
		}
		if (from_priest < from_devil && from_priest > 0)
			return -1;//you lose
		if(to_priest < to_devil && to_priest > 0)
			return -1;//you lose
		return 0;//not yet finish
	}
}