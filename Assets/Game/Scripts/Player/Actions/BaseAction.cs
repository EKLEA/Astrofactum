using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BaseAction : ActionWithWorld
{
	//public event Action<Building> OnUIOpen;
	//bool UIOpen;
	//Building building;
	public override void LeftClick()
	{	
		/*if (!UIOpen)
		{
			building=hit.collider.GetComponent<Building>() ;
			if(building!= null)
			{
				OnUIOpen?.Invoke(building);
				UIOpen=true;
			}
		}*/
	}

	public override void RightClick()
	{
		
	}
    public override void UpdateFunc()
    {
       // base.UpdateFunc();
    }
    public void Reset()
    {
        //OnUIOpen=null;
    }
}