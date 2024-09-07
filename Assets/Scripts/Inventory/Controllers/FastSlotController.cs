using System;
using UnityEngine;

public class  FastSlotController
{
	HudSlotView _view;
	IReadOnlyInventorySlot _slot;
	public FastSlotController (IReadOnlyInventorySlot slot, HudSlotView view)
	{
		_view=view;
		_slot=slot;
		
		slot.ItemIDChanged+=OnSlotItemIDChanged;
		slot.ItemAmountChanged+=OnSlotAmountChanged;
		
		view.SetUpSlot(slot);
	}
	private void OnSlotAmountChanged(int newAmount)
	{
		_view.SetUpSlot(_slot);
	}

	private void OnSlotItemIDChanged(string newItemID)
	{
		_view.SetUpSlot(_slot);
	}
}