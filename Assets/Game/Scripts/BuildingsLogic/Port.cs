using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Port : MonoBehaviour
{
	public PortDir portDir{get {return _portDir;}}
	public PortType portType{get {return _portType;}}
	public Transform point;
	[SerializeField] PortDir _portDir;
	[SerializeField] PortType _portType;
	
	public IWorkWithItems  fromBuilding;
	public IWorkWithItems  toBuilding;
	
	public BeltSpline inbelt;
	public BeltSpline Outbelt;
	
	public void PortUpdate()
	{
		
		fromBuilding?.ResetRemoveEvent();
	    if(fromBuilding!=null) fromBuilding.OnItemsCanRemoved+=GetItemFromBuilding;
	}
	public void Ping()
	{
		fromBuilding?.Ping();
	    toBuilding?.Ping();
	}
	public void GetItemFromBuilding(string id)
	{
	
		if (fromBuilding == null || toBuilding == null)
			return;
		if(fromBuilding.IAmSetUped==false||toBuilding.IAmSetUped==false) return;
		
		if(!toBuilding.CanAdd) return;
		if(fromBuilding.IsRemovedNow)  return;
		
		else
		{
			SlotTransferArgs transferSlot;
			int max = InfoDataBase.itemInfoBase.GetInfo(id).maxCountInPack;
			transferSlot = fromBuilding.RemoveFromBuilding(id, max);
			fromBuilding.IsRemovedNow=true;
			toBuilding.AddToBuilding(transferSlot.Id, transferSlot.Amount);
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