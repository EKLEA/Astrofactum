using System;
using TMPro;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
	[SerializeField] InventorySlotView[] _slots;
	[SerializeField] TMP_Text _textOwner;
	
	public string OwnerID
	{
		get=>_textOwner.text;
		set=>_textOwner.text = value;
	}
	
	public InventorySlotView GetInventorySlotView(int index)
	{
		return _slots[index];
	}
}