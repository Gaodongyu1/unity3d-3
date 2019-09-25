using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Engine
{
	public class Director: System.Object
	{
		public static int cn_move = 0;//0是暂停，1是可以移动
		private static Director _instance;
		public SceneController curren{ get; set;}
		public static Director get_Instance(){
			if (_instance == null)
			{
				_instance = new Director();
			}
			return _instance;
		}
	}

	public interface SceneController
	{
		void loadResources();
	}

	public interface UserAction
    {
		void moveboat();
		void isClickChar (MyCharacterController tem_char);
		void restart();
		void pause();
		void Coninu();
	}
	
	public class CoastController
    {
		readonly GameObject coast;
		readonly Vector3 from_pos = new Vector3(9,1,0);
		readonly Vector3 to_pos = new Vector3(-9,1,0);
		readonly Vector3[] postion;
		readonly int TFflag;//-1->to, 1->from

		private MyCharacterController[] passengerPlaner;

		public CoastController(string to_or_from){
			postion = new Vector3[] {
				new Vector3 (6.5f, 2.25f, 0),
				new Vector3 (7.5f, 2.25f, 0),
				new Vector3 (8.5f, 2.25f, 0),
				new Vector3 (9.5f, 2.25f, 0),
				new Vector3 (10.5f, 2.25f, 0),
				new Vector3 (11.5f, 2.25f, 0)
			};
			passengerPlaner = new MyCharacterController[6];
			if(to_or_from == "from"){
				coast = Object.Instantiate(Resources.Load("Prefabs/Coast", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
				coast.name = "from";
				TFflag = 1;
			}
			else{
				coast = Object.Instantiate(Resources.Load("Prefabs/Coast", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
				coast.name = "to";
				TFflag = -1;
			}
		}
		public int getTFflag(){
			return TFflag;
		}
		public MyCharacterController getOffCoast(string object_name){
			for(int i=0; i<passengerPlaner.Length; i++){
				if(passengerPlaner[i] != null && passengerPlaner[i].getName() == object_name){
					MyCharacterController myCharacter = passengerPlaner[i];
					passengerPlaner[i] = null;
					return myCharacter;
				}
			}
			return null;
		}
		public int getEmptyIndex(){
			for(int i=0; i<passengerPlaner.Length; i++){
				if(passengerPlaner[i] == null){
					return i;
				}
			}
			return -1;
		}
		public Vector3 getEmptyPosition(){
			int index = getEmptyIndex();
			Vector3 pos = postion[index];
			pos.x *= TFflag;
			return pos;
		}
		public void getOnCoast(MyCharacterController myCharacter){
			int index = getEmptyIndex();
			passengerPlaner[index] = myCharacter;
		}
		public int[] getCharacterNum(){
			int[] count = {0,0};
			for(int i=0; i<passengerPlaner.Length; i++){
				if(passengerPlaner[i] == null) continue;
				if(passengerPlaner[i].getType() == 0) count[0]++;
				else count[1]++;
			}
			return count;
		}
		public void reset(){
			passengerPlaner = new MyCharacterController[6];
		}
	}

	public class MyCharacterController{
		private GameObject character;
		readonly ClickGUI clickgui;
		readonly int Ctype;//0->priset, 1->devil
		private bool isOnboat;
		private CoastController coastcontroller;

		public MyCharacterController(string Myname){
			if(Myname == "priest"){
				character = Object.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity,null) as GameObject;
				Ctype = 0;
			}
			else{
				character = Object.Instantiate(Resources.Load("Prefabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity,null) as GameObject;
				Ctype = 1;
			}
			clickgui = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
			clickgui.setController(this);
		}
		public GameObject getGameObject(){
			return character;
		}
		public int getType(){
			return Ctype;
		}
		public void setName(string name){
			character.name = name;
		}
		public string getName(){
			return character.name;
		}
		public void setPosition(Vector3 postion){
			character.transform.position = postion;
		}
		public void getOnBoat(BoatController tem_boat){
			coastcontroller = null;
			character.transform.parent = tem_boat.getGameObject ().transform;
			isOnboat = true;
		}
		public void getOnCoast(CoastController coastCon){
			coastcontroller = coastCon;
			character.transform.parent = null;
			isOnboat = false;
		}
		public bool _isOnBoat(){
			return isOnboat;
		}
		public CoastController getCoastController(){
			return coastcontroller;
		}
		public void reset(){
			coastcontroller = (Director.get_Instance ().curren as MySceneController).fromCoast;
			getOnCoast(coastcontroller);
			setPosition (coastcontroller.getEmptyPosition ());
			coastcontroller.getOnCoast (this);
		}
		public void Mypause(){
			Director.cn_move = 1;
		}
		public void MyConti(){
			Director.cn_move = 0;
		}
	}

	public class BoatController{
		readonly GameObject boat;
		readonly Vector3 fromPos = new Vector3 (5, 1, 0);
		readonly Vector3 toPos = new Vector3 (-5, 1, 0);
		readonly Vector3[] from_pos;
		readonly Vector3[] to_pos;
		private int TFflag;//-1->to, 1->from
		private MyCharacterController[] passenger = new MyCharacterController[2];
		public float move_speed = 20;                                         

		public BoatController(){
			TFflag = 1;
			from_pos = new Vector3[]{ new Vector3 (4.5f, 1.5f, 0), new Vector3 (5.5f, 1.5f, 0) };
			to_pos = new Vector3[]{ new Vector3 (-5.5f, 1.5f, 0), new Vector3 (-4.5f, 1.5f, 0) };
			boat = Object.Instantiate (Resources.Load ("Prefabs/Boat", typeof(GameObject)), fromPos, Quaternion.identity, null) as GameObject;
			boat.name = "boat";
			boat.AddComponent (typeof(ClickGUI));

		}
		public Vector3 boatMove(){
			if (TFflag == 1) {
				TFflag = -1;
				return new Vector3 (-5, 1, 0);
			} 
            else {
				TFflag = 1;
				return new Vector3 (5, 1, 0);
			}
		}
		public void getOnBoat(MyCharacterController tem_cha){
			int index = getEmptyIndex ();
			passenger [index] = tem_cha;
		}
		public MyCharacterController getOffBoat(string object_name){
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null && passenger [i].getName () == object_name) {
					MyCharacterController temp_character = passenger [i];
					passenger [i] = null;
					return temp_character;
				}
			}
			return null;
		}
		public int getEmptyIndex(){
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null)
					return i;
			}
			return -1;
		}
		public bool IfEmpty(){
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null)
					return false;
			}
			return true;
		}
		public Vector3 getEmptyPosition(){
			Vector3 pos;
			int index = getEmptyIndex ();
			if (TFflag == 1) {
				pos = from_pos [index];
			} else {
				pos = to_pos [index];
			}
			return pos;
		}
		public GameObject getGameObject(){
			return boat;
		}
		public int getTFflag(){
			return TFflag;
		}
		public int[] getCharacterNum(){
			int[] count = { 0, 0 };
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null)
					continue;
				if (passenger [i].getType () == 0) {
					count [0]++;
				} else {
					count [1]++;
				}
			}
			return count;
		}
		public void reset(){
			if (TFflag == -1) {
				boatMove ();
			}
			boat.transform.position = new Vector3 (5, 1, 0);
			passenger = new MyCharacterController[2];
		}
		public void Mypause(){
			Director.cn_move = 1;
		}
		public void MyConti(){
			Director.cn_move = 0;
		}
	}
}