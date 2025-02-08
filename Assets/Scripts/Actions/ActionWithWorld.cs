using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionWithWorld
{
	public event Action endOfAction;	public int minCount;
	public List<Vector3> points=new();
	public bool canAction;
	protected Vector3 currentPos;
	protected RaycastHit hit;
	protected float currentRot;
	protected PhantomObject phantomObject;
	
	public virtual void AddPoint()
	{
		points.Add(currentPos);
	}
	public virtual void SetPhantomPoint(PhantomObject obj){ phantomObject=obj; }
	public virtual void ActionF(){}
	public virtual void LeftClick()
	{
		if(canAction)
		{
			AddPoint();
			if(!Input.GetButton("hold")&&points.Count>=minCount)
			{
				
				ActionF();
				onActionEnded();
			}
		}
	}
	public virtual void RightClick()
	{
		if(points.Count>=minCount) ActionF();//добавить то, что при отмене действия, возвращается к поинт 0
		else onActionEnded();
		
	}
	public virtual void SetUpAction(int minCount)
	{
		this.minCount = minCount;
	}
	public virtual void MouseWheelRotation(float Value)
	{
		
	}
	public void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			UpdateFunc();
		}
		
	}
	public virtual void UpdateFunc(){}
	public virtual bool ValidateBuild(Vector3 pos){ return false; }
	protected virtual void onActionEnded()
	{
		endOfAction?.Invoke();
	}
}
