using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildStructure : ActionWithWorld
{
	BuildingStructure _buildingStructure;
	public BuildStructure(string id)
	{
		if( InfoDataBase.buildingBase.GetInfo(id) != null)
			_buildingStructure=new BuildingStructure(id);
		else
			_buildingStructure=InfoDataBase.structuresBase.GetInfo(id);
	}
	
	public override void LeftClick()
	{
		foreach (var building in _buildingStructure.buildings)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				MonoBehaviour.Instantiate(InfoDataBase.buildingBase.GetInfo(building.Value).prefab, building.Key.Item1+hit.point, Quaternion.Euler(building.Key.Item2.x,building.Key.Item2.y,building.Key.Item2.z));
				
			}
		}
		if(!Input.GetButton("hold"))
		{
			onActionEnded();
		}
	}
	
	public override void RightClick()
	{
		onActionEnded();
	}
}
