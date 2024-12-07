using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildStructure : ActionWithWorld
{
	public Vector3 currentRot;
	public BuildingStructure _buildingStructure;
	GameObject renderPoint;
	Vector3 currentPos;
	void SetUpMeshes()
	{
		renderPoint=new GameObject("render point");
		foreach (var building in _buildingStructure.buildings)
		{
			var gm = InfoDataBase.buildingBase.GetInfo(building.Value).prefab;
			var phantomParentObject=new GameObject("phantomParentObject");
			
			
			phantomParentObject.transform.position=building.Key.Item1+currentPos;
			phantomParentObject.transform.rotation=Quaternion.Euler(building.Key.Item2+currentRot);
			if(gm.transform.childCount>0)
			{
				foreach (Transform child in gm.transform)
				{
					var meshFilter = child.GetComponent<MeshFilter>();
					if (meshFilter != null)
					{
						var mesh = meshFilter.sharedMesh;
						GameObject phantomObject = new GameObject("PhantomObject");
						
						phantomObject.transform.rotation = child.rotation;
						phantomObject.transform.localScale= child.localScale;

						
						MeshFilter newMeshFilter = phantomObject.AddComponent<MeshFilter>();
						MeshRenderer newMeshRenderer = phantomObject.AddComponent<MeshRenderer>();

					
						newMeshFilter.sharedMesh = mesh;

						
						newMeshRenderer.material = previewMaterial;
						phantomObject.transform.parent=phantomParentObject.transform;
					}
				}
			}
			else
			{
				var meshFilter = gm.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					var mesh = meshFilter.sharedMesh;
						GameObject phantomObject = new GameObject("PhantomObject");
						
						phantomObject.transform.rotation = gm.transform.rotation;
						phantomObject.transform.localScale= gm.transform.localScale;

						
						MeshFilter newMeshFilter = phantomObject.AddComponent<MeshFilter>();
						MeshRenderer newMeshRenderer = phantomObject.AddComponent<MeshRenderer>();

					
						newMeshFilter.sharedMesh = mesh;

						
						newMeshRenderer.material = previewMaterial;
						phantomObject.transform.parent=phantomParentObject.transform;
				}
			}
			phantomParentObject.transform.parent=renderPoint.transform;
		}
	}
	public override void Update()
	{
		if(renderPoint==null)SetUpMeshes();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			canAction=ValidateBuild(hit.point,hit.collider.GetComponent<BuildingLogicBase>());
			
			renderPoint.transform.position=currentPos;
		}
		
	}
	public BuildStructure(string id)
	{
		if( InfoDataBase.buildingBase.GetInfo(id) != null)
			_buildingStructure=new BuildingStructure(id);
		else
			_buildingStructure=InfoDataBase.structuresBase.GetInfo(id);
	}
	public override void Action()
	{
		//запуск коротины
	}
	public override void AddPoint()
	{
		points.Add(currentPos);
		foreach (var building in _buildingStructure.buildings)
		{
				
			var gm=MonoBehaviour.Instantiate(InfoDataBase.buildingBase.GetInfo(building.Value).prefab, building.Key.Item1+currentPos, Quaternion.Euler(building.Key.Item2.x,building.Key.Item2.y,building.Key.Item2.z));
				
			gm.layer=LayerMask.NameToLayer("Building");
			BuildingLogicBase logic =gm.GetComponent<BuildingLogicBase>();
			logic.id=building.Value;
			logic.buildingType=InfoDataBase.buildingBase.GetInfo(building.Value).buildingType;
				
		}
	}
	public override void LeftClick()
	{
		base.LeftClick();
		MonoBehaviour.DestroyImmediate(renderPoint);
	}
	
	bool ValidateBuild(Vector3 pos,BuildingLogicBase buildingLogicBase)
	{
		List<Collider> objs = new List<Collider>();

		foreach (var col in _buildingStructure.colliders.Keys)
		{
			objs.AddRange(Physics.OverlapBox( col.Item1+pos+Vector3.up*_buildingStructure.colliders[col].y/2, _buildingStructure.colliders[col]*0.95f/2, Quaternion.Euler(col.Item2), LayerMask.GetMask("Building")));///

			if (objs.Count > 0) 
				return false;
		}
		if (buildingLogicBase!=null)
		{
			if (buildingLogicBase.buildingType!=BuildingsTypes.Foundation) currentPos=pos;
			else currentPos=SnapToGrid(pos);
		}
		else
		{
			currentPos=pos;
		}
		return true;
	}

	public override void RightClick()
	{
		MonoBehaviour.DestroyImmediate(renderPoint);
		base.RightClick();
		
	}
	Vector3 SnapToGrid(Vector3 point)
	{
		float x = Mathf.Round(point.x / 1f) * 1f;
		float z = Mathf.Round(point.z / 1f) * 1f;
		return new Vector3(x, point.y, z);
	}

	
}
