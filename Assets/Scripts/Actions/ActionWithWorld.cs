using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ActionWithWorld
{
	public event Action endOfAction;
	public abstract void LeftClick();
	public abstract void RightClick();
	protected virtual void onActionEnded()
	{
		endOfAction?.Invoke();
	}
}
