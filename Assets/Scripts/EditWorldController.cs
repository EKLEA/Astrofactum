using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditWorldController : MonoBehaviour
{
	public static EditWorldController Instance;
	public Material previewMaterial; 
	
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
	public void SetUpController(List<ActionButton> buttons)
	{
		foreach (ActionButton button in buttons)
		{
			button.OnActionButtonClicked+=SetUpActions;
		}
	}
	void Update()
	{
		action.Update();
	}
	Action currentLeftClickAction;
	Action currentRightClickAction;
	public void LeftClick()
	{
		currentLeftClickAction?.Invoke();
	}
	public void RightClick()
	{
		currentRightClickAction?.Invoke();
	}
	ActionWithWorld action;
	void SetUpActions(string id, ActionTypes type)
	{
		if(type==ActionTypes.BuildFoundation)
		{
			action=new BuildStructure(id);
			action.SetUpAction(1);
			
		}
		else if(type==ActionTypes.EditTerrain)
		{
			action=new EditTerrain();
			action.SetUpAction(2);
		}
		else 
		{
			action = new BaseAction();
			action.SetUpAction(1);
		}
		
		action.previewMaterial=previewMaterial;
		action.endOfAction+=ClearAction;
		currentLeftClickAction=action.LeftClick;
		currentRightClickAction=action.RightClick;
	}
	void ClearAction()
	{
		if (action!=null)action.endOfAction-=ClearAction;
		SetUpActions(null,ActionTypes.BasicAction);
	}
	
}

