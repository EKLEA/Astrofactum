using System;
using System.Collections;
using UnityEngine;

public interface IAmTickable 
{
    
	public virtual void SetUpLogic()
	{
	    
	}    
	public float CurrentProcent{get;}
	public ProcessionState state{get;}
	public event Action<ProcessionState> onStateChanged;
    public virtual void Tick(float deltaTime)
    {
    
    }
}
public enum ProcessionState
{
    Processed,
    AwaitForOutput,
    AwaitForInput,
}