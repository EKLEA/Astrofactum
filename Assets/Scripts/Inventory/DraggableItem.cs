using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : UIItem, IBeginDragHandler, IDragHandler, IEndDragHandler
{	
	[HideInInspector] public Transform parentAfterDrag;	
	[HideInInspector] public Transform parentBeforeDrag;	
	RectTransform _rectTransform=> GetComponent<RectTransform>();
	Canvas canvas => GetComponentInParent<Canvas>();
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		
		image.raycastTarget=false;
		parentAfterDrag=_rectTransform.parent;
		_rectTransform.SetParent(canvas.transform);
		_rectTransform.SetAsLastSibling();
		//PlayerController.Instance.playerState.isDragging=true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		_rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log(parentAfterDrag);
		_rectTransform.SetParent(parentAfterDrag);
		
		_rectTransform.SetAsFirstSibling();
		image.raycastTarget=true;
		
	}
}
	