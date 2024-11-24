using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ActionWithWorld
{
	public event Action endOfAction;
	public Material previewMaterial; 
	public abstract void LeftClick();
	public abstract void RightClick();
	public abstract void Update();
	protected virtual void onActionEnded()
	{
		endOfAction?.Invoke();
	}
}
