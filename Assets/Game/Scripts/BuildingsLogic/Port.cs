using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Port : MonoBehaviour
{
	public PortDir portDir{get {return _portDir;}}
	public PortType portType{get {return _portType;}}
	public Transform point;
	[SerializeField] PortDir _portDir;
	[SerializeField] PortType _portType;
	
	public IWorkWithItems  fromBuilding;
	public IWorkWithItems  toBuilding;
	public Action<Port> onItemRemovedFromSlot;
	public Slot transferSlot
	{
	    get {return _slot;}
	    set
	    {
	    	if(value!=null&& _slot==null)
	    	{
	    	    _slot = value;
	    	    GetItemFromBuilding();
	    	}
	    }
	}
	Slot _slot;
	public void Ping()
	{
		if (fromBuilding != null && toBuilding != null) GetItemFromBuilding();
		else
		{
		    fromBuilding?.UpdateBuilding();
		    toBuilding?.UpdateBuilding();
		}
	}
	
	public void GetItemFromBuilding()
	{	
		if (fromBuilding == null || toBuilding == null)
			return;
		if(fromBuilding.IAmSetUped==false||toBuilding.IAmSetUped==false) return;
		if(!toBuilding.CanAdd) return;	
		if(fromBuilding.IsRemovedNow)  return;
		if(_slot==null) return;
		else
		{
			if(!toBuilding.CanAddM(_slot.Id)) return;
			
			fromBuilding.IsRemovedNow=true;
			var t= _slot.RemoveItem(_slot.MaxInPack);
			if(t==0)
			{
				
			    fromBuilding.IsRemovedNow=false;
			    return;
			}
			
			toBuilding.AddToBuilding(_slot.Id, t);
			if(_slot.Count==0)
			{
				onItemRemovedFromSlot?.Invoke(this);
				_slot=null;
			}
		
			fromBuilding.IsRemovedNow=false;
		}
	}
		
}
	
public enum PortType
{
	solid,
	liquid
}
public enum PortDir
{
	In,
	Out,
	UnSelected
}