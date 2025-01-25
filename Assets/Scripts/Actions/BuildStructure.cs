using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildStructure : ActionWithWorld
{
	float currentRot;
	public BuildingStructure _buildingStructure;
	GameObject renderPoint;
	
	
	public override void UpdateFunc()
	{
		if(renderPoint==null)SetUpMeshes();
		
		canAction=ValidateBuild(currentPos);
		ChangePos(_hit.point,_hit.collider.GetComponent<BuildingLogicBase>());
			
		renderPoint.transform.position=currentPos;
		renderPoint.transform.rotation=Quaternion.Euler(renderPoint.transform.rotation.x,currentRot,renderPoint.transform.rotation.z);
		
	}
	public BuildStructure(string id)
	{
		if( InfoDataBase.buildingBase.GetInfo(id) != null)
			_buildingStructure=new BuildingStructure(id);
		else
			_buildingStructure=InfoDataBase.structuresBase.GetInfo(id);
	}
	public override void ActionF()
	{
		foreach(var point in points)
		{
			foreach (var building in _buildingStructure.buildings)
			{
					
				var gm=MonoBehaviour.Instantiate(InfoDataBase.buildingBase.GetInfo(building.Value).prefab, building.Key.Item1+point, Quaternion.Euler(building.Key.Item2.x,building.Key.Item2.y+currentRot,building.Key.Item2.z));
					
				gm.layer=LayerMask.NameToLayer("Building");
				BuildingLogicBase logic =gm.GetComponent<BuildingLogicBase>();
				logic.id=building.Value;
				logic.buildingType=InfoDataBase.buildingBase.GetInfo(building.Value).buildingType;
					
			}
		}
		
		MonoBehaviour.DestroyImmediate(renderPoint);
	}

	public override void MouseWheelRotation(float Value)
	{
		currentRot=MathF.Round(Value*30+currentRot);
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
	public override bool ValidateBuild(Vector3 pos)
	{
		List<Collider> objs = new List<Collider>();

		foreach (var col in _buildingStructure.colliders.Keys)
		{
			objs.AddRange(Physics.OverlapBox( col.Item1+pos+Vector3.up*_buildingStructure.colliders[col].y/2, _buildingStructure.colliders[col]*0.95f/2, Quaternion.Euler(col.Item2), LayerMask.GetMask("Building")));///

			if (objs.Count > 0) 
				return false;
		}
		
		return true;
	}
	void ChangePos(Vector3 pos, BuildingLogicBase buildingLogicBase)
	{
		if (buildingLogicBase!=null)
		{
			if (buildingLogicBase.buildingType!=BuildingsTypes.Foundation) currentPos=pos;
			else currentPos=SnapToGrid(pos);
		}
		else
		{
			currentPos=pos;
		}
	}
	void SetUpMeshes()
	{
		renderPoint=new GameObject("render point");
		foreach (var building in _buildingStructure.buildings)
		{
			var gm = InfoDataBase.buildingBase.GetInfo(building.Value).prefab;
			var phantomParentObject=new GameObject("phantomParentObject");
			
			
			phantomParentObject.transform.position=building.Key.Item1+currentPos;
			phantomParentObject.transform.rotation=Quaternion.Euler(building.Key.Item2);
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

}
