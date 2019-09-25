using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.Engine
{

	public class SSAction : ScriptableObject     //动作基类      
	{

		public bool enable = true;   //动作是否进行                   
		public bool destroy = false;   //动作是否该被销毁                 

		public GameObject gameobject;                   
		public Transform transform;                     
		public ISSActionCallback callback;   //利用接口实现游戏的通信           

		protected SSAction() {}                        

		public virtual void Start()    //异常处理                
		{
			throw new System.NotImplementedException();
		}

		public virtual void Update()
		{
			throw new System.NotImplementedException();
		}
	}

	public class SSMoveToAction : SSAction       //实现物体的移动，单动作
	{
		public Vector3 target;  //目的地      
		public float speed;     //速度      

		private SSMoveToAction(){}

		public static SSMoveToAction GetSSAction(Vector3 target, float speed) 
		{
			SSMoveToAction action = ScriptableObject.CreateInstance<SSMoveToAction>();
			action.target = target;
			action.speed = speed;
			return action;
		}

		public override void Update() 
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed*Time.deltaTime);
			if (this.transform.position == target) 
			{
                //动作完成时销毁该动作并告诉管理者动作已经完成。
				this.destroy = true;
				this.callback.SSActionEvent(this);      
			}
		}

		public override void Start() {}
	}

	public class SequenceAction: SSAction, ISSActionCallback //组合动作
	{           
		public List<SSAction> sequence;    //动作的列表
		public int repeat = -1;           //无限循环做组合中的动作
		public int start = 0;             //当前动作的索引

		public static SequenceAction GetSSAcition(int repeat, int start, List<SSAction> sequence) 
		{
			SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
			action.repeat = repeat;
			action.sequence = sequence;
			action.start = start;
			return action;
		}

		public override void Update() //每一帧完成当前动作
		{
			if (sequence.Count == 0) return;
			if (start < sequence.Count) 
			{
				sequence[start].Update();   //一个组合中的一个动作执行完后会调用接口,所以这里看似没有start++实则是在回调接口函数中实现   
			}
		}

		public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
			int intParam = 0, string strParam = null, Object objectParam = null)
		{
			source.destroy = false;          //先保留动作，如果是无限循环动作组合之后还要使用
			this.start++;
			if (this.start >= sequence.Count) 
			{
				this.start = 0;
				if (repeat > 0) repeat--;
				if (repeat == 0) 
				{
					this.destroy = true;   //整个组合动作被删除           
					this.callback.SSActionEvent(this); //告诉组合动作管理对象动作已经完成
				}
			}
		}

		public override void Start() //为每一个动作注入游戏对象
		{
			foreach(SSAction action in sequence) 
			{
				action.gameobject = this.gameobject;
				action.transform = this.transform;
				action.callback = this;   //组合动作的每个小动作的回调是这个组合动作             
				action.Start();
			}
		}

		void OnDestroy() {}
	}

    //动作事件接口，所有事件管理者都必须实现这个接口来进行事件调度。
	public enum SSActionEventType : int { Started, Competeted }  
	public interface ISSActionCallback  
	{  
		void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
			int intParam = 0, string strParam = null, Object objectParam = null);  
	} 

	public class SSActionManager: MonoBehaviour, ISSActionCallback    //动作管理基类                  //action管理器
	{   

		private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    //将执行的动作的字典集合,int为key，SSAction为value
		private List<SSAction> waitingAdd = new List<SSAction>();                       //等待去执行的动作列表
		private List<int> waitingDelete = new List<int>();                              //等待删除的动作的key                

		protected void Update() 
		{
			foreach(SSAction ac in waitingAdd)                                         
			{
				actions[ac.GetInstanceID()] = ac;       //获取动作的ID作为key                               //获取动作实例的ID作为key
			}
			waitingAdd.Clear();

			foreach(KeyValuePair<int, SSAction> kv in actions) 
			{
				SSAction ac = kv.Value;
				if (ac.destroy) 
				{
					waitingDelete.Add(ac.GetInstanceID());
				} 
				else if (ac.enable) 
				{
					ac.Update();
				}
			}

			foreach(int key in waitingDelete) 
			{
				SSAction ac = actions[key];
				actions.Remove(key);
				Destroy(ac);
			}
			waitingDelete.Clear();
		}

        //把游戏对象与动作绑定，并绑定该动作事件的消息接收者
		public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager) 
		{
			action.gameobject = gameobject;
			action.transform = gameobject.transform;
			action.callback = manager;                                               
			waitingAdd.Add(action);                                                    
			action.Start();
		}

		public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
			int intParam = 0, string strParam = null, Object objectParam = null) {}
	}		
}