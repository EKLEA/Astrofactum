using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : Button
{
	[SerializeField] public TextMeshProUGUI textMeshPro;
	string _id;
	UIController _controller;
	public void SetUpButton(string id,UIController controller)
	{
		textMeshPro= GetComponentInChildren<TextMeshProUGUI>();	
		_id=id;
		textMeshPro.text=id;
		_controller = controller;
		onClick.AddListener(InvokeInfo);
		
	}
	void InvokeInfo()
	{
		_controller.InvokeMethod(_id,this);
	}
}
