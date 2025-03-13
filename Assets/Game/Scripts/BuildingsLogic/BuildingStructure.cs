using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class BuildingStructure : MonoBehaviour, IAmSctructure
{
	public int countOfReqDrones { get {return _currentDrones;}}//Придумать формулу для расчётов дронов

	public float timeOfBuild => throw new NotImplementedException();//считать через дронов

	public float currentTimeOfBuild => throw new NotImplementedException();//считать через дронов

	public bool isEnouhtItems {get{ return itemsToBuild.Count == currentItemsToBuild.Count &&
	itemsToBuild.All(kv => 
		currentItemsToBuild.TryGetValue(kv.Key, out int count) && count == kv.Value);}} 

	public IAmDronNetworkPart closestDronNetworkPart { get => throw new NotImplementedException();  }

	public int maxCountOfDrones => throw new NotImplementedException();//считать через постройки

	public Dictionary<Item, int> itemsToBuild {get{return _itemsToBuild;}}

	public Dictionary<Item, int> currentItemsToBuild {get{return _currentItemsToBuild;}}

	public event Action endOfBuildingStructure;
	public event Action endOfColItems;
	private int _currentDrones;
	Dictionary<Item, int> _itemsToBuild;

	Dictionary<Item, int> _currentItemsToBuild;
	List<PhantomParent> _buildings = new();
	public void Init()
	{
		gameObject.layer=LayerMask.NameToLayer("Phantom");
		gameObject.tag="Structure";

	}
	public void AddPoint(PhantomParent phantom)
	{
		phantom.transform.parent=this.transform;
		_buildings.Add(phantom);
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			foreach (var s in _buildings)
				s.UnPhantom();
			DestroyImmediate(gameObject);
		}
		
	}
	
}
