using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Engine;

public class MySceneActionManager:SSActionManager  
{                  
	private SSMoveToAction move_boat;     
	private SequenceAction move_role;     

	public MySceneController sceneController;

	protected void Start()
	{
		sceneController = (MySceneController)Director.get_Instance().curren;
		sceneController.actionManager = this;
	}
	public void moveBoat(GameObject boat, Vector3 target, float speed)
	{
		move_boat = SSMoveToAction.GetSSAction(target, speed);
		this.RunAction(boat, move_boat, this);
	}

	public void moveRole(GameObject role, Vector3 middle_pos, Vector3 end_pos,float speed)
	{
		SSAction action1 = SSMoveToAction.GetSSAction(middle_pos, speed);
		SSAction action2 = SSMoveToAction.GetSSAction(end_pos, speed);
		move_role = SequenceAction.GetSSAcition(1, 0, new List<SSAction>{action1, action2});
		this.RunAction(role, move_role, this);
	}
}