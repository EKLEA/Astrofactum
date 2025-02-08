using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildConstruction : ActionWithWorld
{
	PhantomObject phantomParentObject;
	BuildingInfo buildingInfo;
	List<(float,string)> buildings;
	
	
	public override void UpdateFunc()
	{
		
		phantomParentObject.gameObject.layer=LayerMask.NameToLayer("Ignore Raycast");
		ChangePos(hit.point,hit.collider.GetComponent<BuildingLogicBase>());
		canAction=ValidateBuild(currentPos);
		
			
		phantomParentObject.transform.position=currentPos;
		phantomParentObject.transform.rotation=Quaternion.Euler(phantomParentObject.transform.rotation.x,currentRot,phantomParentObject.transform.rotation.z);
		
	}
	public BuildConstruction(string id)
	{
		buildingInfo=InfoDataBase.buildingBase.GetInfo(id);
		buildings=new();
		phantomObject.Init(buildingInfo);
		phantomParentObject=phantomObject; 
	}
	public override void AddPoint()
	{
		points.Add(currentPos);
		buildings.Add((currentRot,buildingInfo.id));
	}
	public override void ActionF()
	{
		GameObject obj = new GameObject("BuildingStructure");
		var buildingStructure = obj.AddComponent<BuildingStructure>();
		buildingStructure.Init(points,buildings);
		
		MonoBehaviour.DestroyImmediate(phantomParentObject);
	}
	public override void MouseWheelRotation(float Value)
	{
		currentRot=MathF.Round(Value*30+currentRot);
	}

	public override void RightClick()
	{
		MonoBehaviour.DestroyImmediate(phantomParentObject);
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
		Vector3 col=buildingInfo.prefab.GetComponent<BoxCollider>().size;
		objs.AddRange(Physics.OverlapBox( pos+Vector3.up*col.y/2, col/2, quaternion.identity, LayerMask.GetMask("Building")|LayerMask.GetMask("Phantom")));///

		if (objs.Count > 0) 
			return false;
		
		return true;
	}
	void ChangePos(Vector3 pos, BuildingLogicBase buildingLogicBase)
	{
		if (buildingLogicBase!=null)
		{
			if (buildingLogicBase is FoundationLogic) currentPos=pos;
			else currentPos=SnapToGrid(pos);
		}
		else
		{
			currentPos=pos;
		}
	}
	

}
