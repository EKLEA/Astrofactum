using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Port : MonoBehaviour,IDisposable
{
	public PortDir portDir{get {return _portDir;}}
	public PortType portType{get {return _portType;}}
	public event Action OnItemDeleted;
	public Transform point;
	[SerializeField] PortDir _portDir;
	[SerializeField] PortType _portType;
	
	IWorkWithItems  _fromBuilding;
	IWorkWithItems  _toBuilding;
	
	public BeltSpline inbelt;
	public BeltSpline Outbelt;
	
	public void PortUpdateFrom(IWorkWithItems  fromBuilding)
	{
	
		_fromBuilding?.ResetRemoveEvent();
	    _fromBuilding = fromBuilding;
	    
	    fromBuilding.OnItemsCanRemoved+=GetItemFromBuilding;
	    fromBuilding.Ping();
	}
	public void PortUpdateTo(IWorkWithItems  toBuilding)
	{
		_toBuilding?.ResetAddEvent();
	    _toBuilding = toBuilding;
	    toBuilding.OnItemsCanAdded+=GetItemFromBuilding;
	    toBuilding.Ping();
	}
	public void GetItemFromBuilding(string id)
	{
		
		if (_fromBuilding == null || _toBuilding == null)
			return;
		if(_fromBuilding.IAmSetUped==false||_toBuilding.IAmSetUped==false) return;
		if(!_toBuilding.CanAdd) return;
		SlotTransferArgs transferSlot;

		if (id == null)
		{
			var outSlots = _fromBuilding.outSlots;
			if  ( outSlots.Count > 0&&outSlots[^1] != null)
			{
				string lastId = outSlots[^1].Id;
				int max = InfoDataBase.itemInfoBase.GetInfo(lastId).maxCountInPack;
				transferSlot = _fromBuilding.RemoveFromBuilding(lastId, max);
				//скольковзяли
			}
			else
			{
				Debug.LogWarning("No items in outSlots to remove.");
				return;
			}
		}
		else
		{
			int max = InfoDataBase.itemInfoBase.GetInfo(id).maxCountInPack;
			transferSlot = _fromBuilding.RemoveFromBuilding(id, max);
		}
		
		_toBuilding.AddToBuilding(transferSlot.Id, transferSlot.Amount);
	}

    public void Dispose()
    {
        OnItemDeleted=null;
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