using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : Button
{
	public event Action<string> OnActionButtonClicked;
	string _id;
	public void SetUpButton(string id)
	{
		_id=id;
		onClick.AddListener(SetUpInfo);
	}
	public void SetUpInfo()
	{
		OnActionButtonClicked?.Invoke(_id);
	}
}
