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
	protected BuildingInfo buildingInfo;
	protected BuildingStructure buildingStructure;
	
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
		phantomObject=PhantomObjCreater.CreatePhantomObject(id,EditWorldController.Instance.transform,currentPos,currentRot);
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
		buildingStructure.AddPoint((buildingInfo.id,currentPos,currentRot));
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

	protected Vector3 SnapToGrid(Vector3 point)
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
	

	
	protected void ChangePos(Vector3 pos, Collider collider)
	{
		var amBuilding= collider.GetComponent<IAmBuilding>();
		var obj=collider.gameObject;
		var bx = collider as BoxCollider;
		if (amBuilding!=null)
		{
			var buildingLogic=InfoDataBase.buildingBase.GetInfo(amBuilding.id).prefab.GetComponent<IAmBuilding>();
			var tempBuilding = buildingInfo.prefab.GetComponent<IAmBuilding>();
			if (buildingLogic is FoundationLogic)
			{
				currentPos=SnapToGrid(pos);
				if(tempBuilding is FoundationLogic)
				{
					if(Math.Abs(pos.x-obj.transform.position.x)>=Math.Abs(bx.size.x/2-4)&&Math.Abs(pos.x-obj.transform.position.x)<=Math.Abs(bx.size.x/2)) 
					{
						Vector3 right = obj.transform.rotation * Vector3.right;
						currentPos = obj.transform.position + bx.size.x * right * Mathf.Sign(pos.x - obj.transform.position.x);
						currentRot=obj.transform.rotation.eulerAngles.y;
					}
					else if(Math.Abs(pos.z-obj.transform.position.z)>=Math.Abs(bx.size.z/2-4) &&Math.Abs(pos.z-obj.transform.position.z)<=Math.Abs(bx.size.z/2))
					{
						Vector3 forward = obj.transform.rotation * Vector3.forward;
						currentPos = obj.transform.position + bx.size.z * forward * Mathf.Sign(pos.z - obj.transform.position.z);
						currentRot=obj.transform.rotation.eulerAngles.y;	
					}
					
				}
				
				if(Math.Abs(pos.x-obj.transform.position.x)<=4&&Math.Abs(pos.x-obj.transform.position.x)>=0
				&& Math.Abs(pos.z-obj.transform.position.z)<=4&&Math.Abs(pos.z-obj.transform.position.z)>=0)
				{
					currentPos=obj.transform.position+Vector3.up*obj.GetComponent<BoxCollider>().size.y;
				}
			} 
			else currentPos=pos;
		}
		else
		{
			currentPos=pos;
		}
	}
	

}

