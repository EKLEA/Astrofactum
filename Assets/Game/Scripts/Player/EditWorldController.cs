using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditWorldController : MonoBehaviour
{
	public static EditWorldController Instance;
	Action currentLeftClickAction;
	Action currentRightClickAction;
	Action<float> currentMouseWheelRotAction;
	
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
		DontDestroyOnLoad(gameObject);
		ClearAction();
	}
	void Update()
	{
		action.Update();
	}
	
	public void LeftClick()
	{
		currentLeftClickAction?.Invoke();
	}
	public void RightClick()
	{
		currentRightClickAction?.Invoke();
	}
	public void MouseWheelRotation(float value)
	{
		currentMouseWheelRotAction?.Invoke(value);
	}
	ActionWithWorld action;
	public void SetUpAction(string id)
	{
		
		if (id== null) 
		{
			action= new BaseAction();
			action.SetUpAction(0);
		}
		else
		{
			var obj = InfoDataBase.freaturesBase.GetInfo(id);
			switch(obj.actionType)
			{
				case ActionTypes.BuildStructure: 
					action= new BuildConstruction(id);
					break;
				case ActionTypes.BuildManyPointStructure:
					action= new BuildSplineConstruction(id);
					break;
				case ActionTypes.EditTerrain: action = new EditTerrain(id); break;
			}
			action.SetUpAction(obj.minPoints);
			
		}
		
		action.endOfAction+=ClearAction;
		currentLeftClickAction=action.LeftClick;
		currentRightClickAction=action.RightClick;
		currentMouseWheelRotAction=action.MouseWheelRotation;		
	}
	void ClearAction()
	{
		if (action!=null)action.endOfAction-=ClearAction;
		action=null;
		SetUpAction(null);
	}
	
}

