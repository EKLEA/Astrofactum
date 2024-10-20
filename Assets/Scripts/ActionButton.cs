using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : Button
{
	public event Action<string, ActionTypes> OnActionButtonClicked;
	string _id;
	ActionTypes _type;
	public void SetUpButton(string id, ActionTypes type)
	{
		_id=id;
		_type = type;
		onClick.AddListener(SetUpInfo);
	}
	public void SetUpInfo()
	{
		OnActionButtonClicked?.Invoke(_id,_type);
	}
}
