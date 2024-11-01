using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildStructure : ActionWithWorld
{
	bool canBuild=true;	
	public Vector3 currentPos;
	public BuildingStructure _buildingStructure;
	public override void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			currentPos=hit.point;
			canBuild=ValidateBuild(hit.point);
			Debug.Log(canBuild);
		}
	}
	public BuildStructure(string id)
	{
		if( InfoDataBase.buildingBase.GetInfo(id) != null)
			_buildingStructure=new BuildingStructure(id);
		else
			_buildingStructure=InfoDataBase.structuresBase.GetInfo(id);
	}
	
	public override void LeftClick()
	{
		if(canBuild)
		{
			foreach (var building in _buildingStructure.buildings)
			{
				
				var gm=MonoBehaviour.Instantiate(InfoDataBase.buildingBase.GetInfo(building.Value).prefab, building.Key.Item1+currentPos, Quaternion.Euler(building.Key.Item2.x,building.Key.Item2.y,building.Key.Item2.z));
				
				gm.layer=LayerMask.NameToLayer("Building");
				
			}
			if(!Input.GetButton("hold"))
			{
				onActionEnded();
			}
		}
	}
	
	bool ValidateBuild(Vector3 pos)
	{
		// Инициализируем список objs при каждом вызове, чтобы избежать накопления объектов
		List<Collider> objs = new List<Collider>();

		foreach (var col in _buildingStructure.colliders.Keys)
		{
			// Используем GetMask для корректной проверки на слое "Building"
			objs.AddRange(Physics.OverlapBox( col.Item1+pos, _buildingStructure.colliders[col]/2, Quaternion.Euler(col.Item2), LayerMask.GetMask("Building")));

			// Если найдены пересечения, сразу возвращаем false
			if (objs.Count > 0) 
				return false;
		}

		// Если пересечений нет, возвращаем true
		return true;
	}

	public override void RightClick()
	{
		onActionEnded();
	}

	
}
