using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class InventoryGridController
{
	readonly List<InventorySlotController> _slotController=new();// публик сделать если будет меняться размер инвентаря
	IReadOnlyInventoryGrid _inventoryGrid;
	public InventoryGridController(IReadOnlyInventoryGrid inventory,InventoryView view)
	{
		var size = inventory.Size;
		var slots = inventory.GetSlots();
		_inventoryGrid=inventory;
		var lineLength=size.y;
		
		for(int i=0; i<size.x; i++)
		{
			for(int j=0; j<size.y; j++)
			{
				var index=i*lineLength+j;
				var slotView=view.GetInventorySlotView(index);
				var slot=slots[i,j];
				_slotController.Add(new InventorySlotController(slot,slotView));
				_slotController.Last().OnInventorySlotSwitched+=SwitchViewSlots;
			}
		}
		view.OwnerID=inventory.OwnerID;
	}
	void SwitchViewSlots(InventorySlotView toSlot, InventorySlotView fromSlot)
	{
		var exInv=_inventoryGrid as InventoryGrid;
		int indexA=toSlot.transform.GetSiblingIndex();
		int indexB=fromSlot.transform.GetSiblingIndex();
		exInv.SwitchSlots(new Vector2Int(indexA/exInv.Size.y,indexA%exInv.Size.y),new Vector2Int(indexB/exInv.Size.y,indexB%exInv.Size.y));
	}
}