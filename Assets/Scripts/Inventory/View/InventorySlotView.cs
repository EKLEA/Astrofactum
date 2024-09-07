using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotView : MonoBehaviour,IDropHandler
{
	public event Action<InventorySlotView> OnItemSwitched;
	public DraggableItem ItemPrefab;
	
	DraggableItem _draggableItem;
	public DraggableItem InventoryItem
	{
		get
		{
			return _draggableItem;
		}
		set
		{
			
			_draggableItem=value;
			if(_draggableItem!=null) _draggableItem.image.raycastTarget=true;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		if(InventoryItem!=null) InventoryItem.image.raycastTarget=false;
		
		GameObject dropped=eventData.pointerDrag;
		DraggableItem draggableItem=dropped.GetComponent<DraggableItem>();
		
		draggableItem.parentAfterDrag.GetComponent<InventorySlotView>().InventoryItem=InventoryItem;
		if(InventoryItem!=null) InventoryItem.transform.SetParent(draggableItem.parentAfterDrag);
		
		InventoryItem=draggableItem;
		OnItemSwitched?.Invoke(draggableItem.parentAfterDrag.GetComponent<InventorySlotView>());
		
		InventoryItem.parentAfterDrag=transform;
		//PlayerController.Instance.playerState.isDragging=false;
		
	}
	
	public void RefreshItem(string itemID, int amount)
	{
		if(itemID==null)
		{
			if(InventoryItem!=null) 
			{
				DestroyImmediate(InventoryItem.gameObject);
				InventoryItem=null;
			}
		}
		else
		{
			if(InventoryItem==null) InventoryItem=Instantiate(ItemPrefab,this.transform);
			InventoryItem.SetUpItem(itemID,amount);
		}
	}
}