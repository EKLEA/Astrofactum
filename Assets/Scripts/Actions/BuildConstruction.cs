using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildConstruction : ActionWithWorld
{
	BuildingInfo buildingInfo;
	BuildingStructure buildingStructure;
	
	public override void UpdateFunc()
	{
		phantomObject.gameObject.layer=LayerMask.NameToLayer("Ignore Raycast");
		ChangePos(hit.point,hit.collider);
		canAction=ValidateBuild(currentPos);
		phantomObject.ChangeColor(canAction);
			
		phantomObject.transform.position=currentPos;
		phantomObject.transform.rotation=Quaternion.Euler(phantomObject.transform.rotation.x,currentRot,phantomObject.transform.rotation.z);
		
	}
	public BuildConstruction(string id)
	{
		buildingInfo=InfoDataBase.buildingBase.GetInfo(id);
		phantomObject = MonoBehaviour.Instantiate(EditWorldController.Instance.phantomObject,currentPos,Quaternion.Euler(0,currentRot,0));
		phantomObject.Init(buildingInfo.id);
	}
	public override void AddPoint()
	{
		if(buildingStructure==null)
		{
			if(hit.collider.transform.parent!=null && hit.collider.transform.parent.tag=="Structure")
			buildingStructure=hit.collider.transform.parent.gameObject.GetComponent<BuildingStructure>();
			else
			{
				GameObject obj = new GameObject("BuildingStructure");
				buildingStructure = obj.AddComponent<BuildingStructure>();
			}
		}
		buildingStructure.AddPoint(currentPos,(currentRot,buildingInfo.id));
		base.AddPoint();
		
	}
	public override void ActionF()
	{
		buildingStructure.Init();
		
		MonoBehaviour.DestroyImmediate(phantomObject.gameObject);
		MonoBehaviour.DestroyImmediate(phantomObject);
		phantomObject=null;
		base.ActionF();
		
	}
	public override void MouseWheelRotation(float Value)
	{
		currentRot=MathF.Round(Value*30+currentRot);
	}

	Vector3 SnapToGrid(Vector3 point)
	{
		float x = Mathf.Round(point.x / 2f) * 2f;
		float z = Mathf.Round(point.z / 2f) * 2f;
		return new Vector3(x, point.y, z);
	}
	public override bool ValidateBuild(Vector3 pos)
	{
		List<Collider> objs = new List<Collider>();
		Vector3 col=phantomObject.GetComponent<BoxCollider>().size;
		objs.AddRange(Physics.OverlapBox( pos+Vector3.up*(col.y/2+0.01f), col/2*0.99f, Quaternion.Euler(0,currentRot,0), LayerMask.GetMask("Building")|LayerMask.GetMask("Phantom")));///

		if (objs.Count > 0) 
			return false;
		
		return true;
	}
	

	
	void ChangePos(Vector3 pos, Collider collider)
	{
		var amBuilding= collider.GetComponent<IAmBuilding>();
		var obj=collider.gameObject;
		var bx = collider as BoxCollider;
		
		if (amBuilding!=null)
		{
			var buildingLogic=InfoDataBase.buildingBase.GetInfo(amBuilding.id).prefab.GetComponent<IAmBuilding>();
			if (buildingLogic is FoundationLogic)
			{
				if(Math.Abs(pos.x-obj.transform.position.x)<=4&&Math.Abs(pos.x-obj.transform.position.x)>=0
				&& Math.Abs(pos.z-obj.transform.position.z)<=4&&Math.Abs(pos.z-obj.transform.position.z)>=0)
				{
					currentPos=obj.transform.position+Vector3.up*buildingInfo.prefab.GetComponent<BoxCollider>().size.y;
				}
				else if(Math.Abs(pos.x-obj.transform.position.x)>=Math.Abs(bx.size.x/2-4)&&Math.Abs(pos.x-obj.transform.position.x)<=Math.Abs(bx.size.x/2)) 
					currentPos=obj.transform.position+bx.size.x*Vector3.right*Math.Sign(pos.x-obj.transform.position.x);
				else if(Math.Abs(pos.z-obj.transform.position.z)>=Math.Abs(bx.size.z/2-4) &&Math.Abs(pos.z-obj.transform.position.z)<=Math.Abs(bx.size.z/2))
					currentPos=obj.transform.position+bx.size.z*Vector3.forward*Math.Sign(pos.z-obj.transform.position.z);			
				else currentPos=SnapToGrid(pos);
			} 
			
		}
		else
		{
			currentPos=pos;
		}
	}
	

}
