using System;
using UnityEngine;

public class BaseAction : ActionWithWorld
{
	public event Action<Building> OnUIOpen;
	public event Action OnUIClose;
	Building building;
	public override void LeftClick()
	{	
		building=hit.collider.GetComponent<Building>() ;
		if(building!= null)
		{
			OnUIOpen?.Invoke(building);
			Debug.Log("open");
		}
	}

	public override void RightClick()
	{
		ActionL();
	}
    public override void UpdateFunc()
    {
       // base.UpdateFunc();
    }
    public override void Reset()
    {
		base.Reset();
        OnUIOpen=null;
    }
}