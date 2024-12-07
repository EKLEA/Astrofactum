using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionWithWorld
{
	public event Action endOfAction;
	public Material previewMaterial; 
	public int minCount;
	public List<Vector3> points=new();
	public bool canAction;
	public virtual void AddPoint(){}
	public virtual void Action(){}
	public virtual void LeftClick()
	{
		if(canAction)
		{
			AddPoint();
			if(!Input.GetButton("hold")&&points.Count>=minCount)
			{
				Action();
				onActionEnded();
			}
		}
	}
	public virtual void SetUpAction(int minCount)
	{
		this.minCount = minCount;
	}
	public virtual void RightClick()
	{
		if(points.Count>=minCount) Action();
		else onActionEnded();
		
	}
	public virtual void Update() {}
	protected virtual void onActionEnded()
	{
		endOfAction?.Invoke();
	}
}
