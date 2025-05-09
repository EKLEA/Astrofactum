using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditWorldController : MonoBehaviour
{
	public static EditWorldController Instance;
	public BuildingController buildingController;
	public ActionsGrid actionsGrid;
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
		if(action!=null&&action is BaseAction)
		{
		    buildingController.ResetEvents();
		    (action as BaseAction).Reset();
		}
		
		if (id== null) 
		{
			actionsGrid.Enable();
			action= new BaseAction();
			action.SetUpAction(ActionTypes.BaseAction);
			(action as BaseAction).OnUIOpen+=buildingController.Init;
			(action as BaseAction).endOfAction+=buildingController.OnUIClosedInvoke;
		}
		else
		{
			var obj = InfoDataBase.actionsBase.GetInfo(id);
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
			actionsGrid.Disable();
			action.SetUpAction(obj.actionType);
			
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
		actionsGrid.Enable();
	}
	
}

