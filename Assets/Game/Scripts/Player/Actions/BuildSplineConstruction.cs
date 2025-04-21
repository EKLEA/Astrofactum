using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildSplineConstruction : ActionWithWorld
{
    BuildingInfo buildingInfo;
    BuildingStructure buildingStructure;
	SplineParent splineParent;
	PhantomParent phantomObject; 
    Quaternion q1,q2;
    Port port,firstport;
    SplineType splineType=>SplineType.StraightAngle;
    string _id;
    public BuildSplineConstruction(string id)
	{
	    _id=id;
		buildingInfo=InfoDataBase.buildingBase.GetInfo(_id);
		NewPhantom();
		canAction=true;
	}    
	void NewPhantom()
	{
		splineParent=BuildingFactory.Create(buildingInfo,currentPos,Quaternion.Euler(0,currentRot,0)) as SplineParent;
		splineParent.resolution.GenerateMeshAlongSpline();
		phantomObject=PhantomCreater.CreatePhantomObject(splineParent);
		foreach (var s in phantomObject.transform.GetComponentsInChildren<Transform>())
		{
		    s.gameObject.layer=LayerMask.NameToLayer("Ignore Raycast");
		}
	}
	public override void UpdateFunc()
	{
		
		ChangePos(hit.point,hit.collider);
		//canAction=ValidateBuild(currentPos);
		port= hit.collider.GetComponent<Port>();
		phantomObject.ChangeColor(canAction);
		CheckPoint();
	}
	public override void RightClick()
	{
		if (pointCount==1)
		{
			pointCount=0;
			splineParent.Reset();
		}
		else if(pointCount==0)
		{
			
			MonoBehaviour.DestroyImmediate(phantomObject.gameObject);
		    ActionF();
		}
		
	}
	public override void AddPoint()
	{
        if(buildingStructure==null)
         {
            if(hit.collider.transform.GetComponentInParent<BuildingStructure>()!=null && hit.collider.transform.GetComponentInParent<BuildingStructure>().gameObject.tag=="Structure")
                buildingStructure=hit.collider.transform.GetComponentInParent<BuildingStructure>();
            else
            {
                GameObject obj = new GameObject("BuildingStructure");
                buildingStructure = obj.AddComponent<BuildingStructure>();
            }
        }
        
        
        if(pointCount==0)
        {
        	q1=Quaternion.Euler(Vector3.zero);
            if(port==null)q1= Quaternion.Euler(0,currentRot,0);
            var args = (port, currentPos, q1);
            
            if(port==null)
				splineParent.SetFirstPointSpline(args);
			else
			{
				if(port.portDir==PortDir.Out) splineParent.SetFirstPointSpline(args);
				
				else splineParent.SetSecondPointSpline(args);
			}
			
			firstport=port;
			
        }
        else if(pointCount==1)
        {
        	q2=Quaternion.Euler(Vector3.zero);
			if(port==null) q2 = Quaternion.Euler(0,splineType==SplineType.StraightAngle? (int)(currentRot/45)*45:currentRot,0);
			var args = (port, currentPos, q2);
			
			if(firstport==null)
			{
			    if(port==null||port.portDir==PortDir.In)
			    	splineParent.SetSecondPointSpline(args);
			}
			else
			{
				if(firstport.portDir==PortDir.Out)
				{
				    if(port==null||port.portDir==PortDir.In)
				    	splineParent.SetSecondPointSpline(args);
				}
				else
				{
				    if(port==null|| port.portDir==PortDir.Out)
				    	splineParent.SetFirstPointSpline(args);
				}
			}
			
			
			splineParent.DrawSpline(splineType,SplineState.Passive);
			buildingStructure.AddPoint(phantomObject);
        }
		else
		{
		    
		}
        base.AddPoint();
        
	}
	
	public void CheckPoint()
	{
		if (pointCount == 0)
		{
			phantomObject.transform.position = currentPos;
			phantomObject.transform.rotation = Quaternion.Euler(0, currentRot, 0);
		}
		else if (pointCount > 0)
		{
			Quaternion rot=Quaternion.Euler(Vector3.zero);
			if(port==null) rot = Quaternion.Euler(0,splineType==SplineType.StraightAngle? (int)(currentRot/45)*45:currentRot,0);

			var args = (port, currentPos, rot);
			
			if(firstport==null)
			{
			    if(port==null||port.portDir==PortDir.In)
			    	splineParent.SetSecondPointSpline(args);
			}
			else
			{
				if(firstport.portDir==PortDir.Out)
				{
				    if(port==null||port.portDir==PortDir.In)
				    	splineParent.SetSecondPointSpline(args);
				}
				else
				{
				    if(port==null|| port.portDir==PortDir.Out)
				    	splineParent.SetFirstPointSpline(args);
				}
			}
			
			splineParent.DrawSpline(splineType, SplineState.Active);
		}
	}
	public override void ActionF()
	{
		buildingStructure?.Init();
		base.ActionF();
		
	}
	public override void MouseWheelRotation(float Value)
	{
	
		currentRot=  MathF.Round(Value*(splineType==SplineType.StraightAngle?90:30)+currentRot);
	}

	protected Vector3 SnapToGrid(Vector3 point)
	{
		float x = Mathf.Round(point.x / 2f) * 2f;
		float z = Mathf.Round(point.z / 2f) * 2f;
		return new Vector3(x, point.y, z);
	}
	public override bool ValidateBuild(Vector3 pos)
	{
		return false;
	}
	protected void ChangePos(Vector3 pos, Collider collider)
	{
		var amBuilding= collider.GetComponent<Building>();
		var obj=collider.gameObject;
		var bx = collider as BoxCollider;
		if (amBuilding!=null)
		{
			var buildingLogic=InfoDataBase.buildingBase.GetInfo(amBuilding.id).prefab.GetComponent<Building>();
			if (buildingLogic is FoundationLogic)
			{
				currentPos=SnapToGrid(pos);
				
				
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
			if(port!=null&&firstport?.portDir!=port.portDir) 
			{
			    currentPos=port.point.position;
			    currentRot=port.point.rotation.eulerAngles.y;
			}
			else currentPos=pos;
		}
	}
}