using Unity.VisualScripting;
using UnityEngine;

public interface IHavePorts
{
	Port[] inPorts { get;}
	Port[] OutPorts { get;}
	public virtual void AddItemToBuilding(Item item)
	{
	    
	}
	public virtual void RemoveItemFromBuilding(Item item)
	{
	    
	}
}