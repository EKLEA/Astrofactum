using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionWithWorld
{
	public event Action endOfAction;	public int minCount;
	public bool canAction;
	protected Vector3 currentPos;
	protected RaycastHit hit;
	protected float currentRot;
	protected int pointCount;
	
	public virtual void AddPoint()
	{
		pointCount++;
	}
	public virtual void ActionF(){ onActionEnded();}
	public virtual void LeftClick()
	{
		if(canAction)
		{
			AddPoint();
			if(!Input.GetButton("hold")&&pointCount>=minCount)
			{
				
				ActionF();
				onActionEnded();
			}
		}
	}
	public virtual void RightClick()
	{
		if(pointCount>=minCount) ActionF();//добавить то, что при отмене действия, возвращается к поинт 0
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
