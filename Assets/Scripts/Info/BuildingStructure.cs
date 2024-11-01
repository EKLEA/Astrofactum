using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingStructure:UnityEngine.Object
{
	public string idStructure;
	public Dictionary<(Vector3,Vector3),string> buildings=new();
	public Dictionary<(Vector3,Vector3),Vector3> colliders=new();
	public BuildingStructure(string id)
	{
		buildings.Add((Vector3.zero,Vector3.zero),id);
		colliders.Add((Vector3.zero,Vector3.zero),InfoDataBase.buildingBase.GetInfo(id).prefab.GetComponent<BoxCollider>().size);
		
	}
	public BuildingStructure(Dictionary<(Vector3,Vector3),string> _buildings)
	{
		foreach (var building in _buildings.Keys)
		{
			buildings.Add((building.Item1,building.Item2),_buildings[building]);
			colliders.Add((building.Item1,building.Item2),InfoDataBase.buildingBase.GetInfo(_buildings[building]).prefab.GetComponent<BoxCollider>().size);
		}
	}
}
