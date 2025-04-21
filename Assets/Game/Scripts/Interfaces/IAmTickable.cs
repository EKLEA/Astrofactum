using System.Collections;
using UnityEngine;

public interface IAmTickable 
{
    
	public virtual void SetUpLogic()
	{
	    
	}    
	public bool IsProcessed{get;}
	public float CurrentProcent{get;}
    public void Tick(float deltaTime)
    {
    
    }
}