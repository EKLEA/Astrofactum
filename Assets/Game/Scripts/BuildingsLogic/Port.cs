using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Port : MonoBehaviour
{
	public PortDir portDir{get {return _portDir;}}
	public PortType portType{get {return _portType;}}
	public event Action OnItemDeleted;
	public Transform point;
	[SerializeField] PortDir _portDir;
	[SerializeField] PortType _portType;
	
	BuildingWithPorts _fromBuilding;
	BuildingWithPorts _toBuilding;
	
	public void PortUpdateFrom(BuildingWithPorts fromBuilding)
	{
	    _fromBuilding = fromBuilding;
	    fromBuilding.OnItemsCanRemoved+=GetItemFromBuilding;
	    
	}
	public void PortUpdateTo( BuildingWithPorts toBuilding)
	{
	    _toBuilding = toBuilding;
	    toBuilding.OnItemsCanAdded+=AddToBuilding;
	}
	public void GetItemFromBuilding(Slot item)
	{
		SlotTransferArgs args =_toBuilding.AddToBuilding(item);
		if(args.remainingSlot.IsEmpty) OnItemDeleted?.Invoke();
		else item=args.remainingSlot;
	}
	public void AddToBuilding(Slot item)
	{
	    SlotTransferArgs args =_fromBuilding.AddToBuilding(item);
	    if(args.remainingSlot.IsEmpty) OnItemDeleted?.Invoke();
		
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