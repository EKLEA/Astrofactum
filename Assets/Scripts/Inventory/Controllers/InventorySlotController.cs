using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventorySlotController 
{
	readonly InventorySlotView _view;
	readonly IReadOnlyInventorySlot _slot;
	public event Action<InventorySlotView,InventorySlotView> OnInventorySlotSwitched;
	public InventorySlotController(IReadOnlyInventorySlot slot, InventorySlotView view)//вьюшку можно переделать под интерфейс
	{
		_view=view;
		_slot=slot;
		slot.ItemIDChanged+=OnSlotItemIDChanged;
		slot.ItemAmountChanged+=OnSlotAmountChanged;
		view.OnItemSwitched+=OnItemSwitched;
		view.RefreshItem(slot.ItemID,slot.Amount);
	}

	private void OnItemSwitched(InventorySlotView fromslot)
	{
	   OnInventorySlotSwitched?.Invoke(_view,fromslot);
	}

	private void OnSlotAmountChanged(int newAmount)
	{
		_view.RefreshItem(_slot.ItemID,newAmount);
	}

	private void OnSlotItemIDChanged(string newItemID)
	{
		_view.RefreshItem(newItemID,_slot.Amount);
	}
}