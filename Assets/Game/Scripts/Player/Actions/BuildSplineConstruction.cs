using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildSplineConstruction : ActionWithWorld
{
    BuildingInfo buildingInfo;
    BuildingStructure buildingStructure;
	SplineParent splineParent;
	PhantomParent phantomObject; 
    Port p1, p2;
    Vector3 pos1, pos2;
    Quaternion q1,q2;
    SplineType splineType=>SplineType.StraightAngle;
    string _id;
    public BuildSplineConstruction(string id)
	{
		
	    _id=id;
		newPhantom();
		canAction=true;
	}    
	void newPhantom()
	{
	    buildingInfo=InfoDataBase.buildingBase.GetInfo(_id);
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
		
		phantomObject.ChangeColor(canAction);
		CheckPoint();
		
	}
	public override void RightClick()
	{
		if(pointCount==1)
		{
		    MonoBehaviour.DestroyImmediate(phantomObject.gameObject);
			phantomObject=null;
			newPhantom();
		}
		else if(pointCount>1)
		{
		    ActionF();
		}
		else onActionEnded();
		
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
        
        Port port = hit.collider.GetComponent<Port>();
        if(pointCount==0)
        {
            if(port!=null)
            {
                p1 =port;
                p1.PortUpdateTo(splineParent);
            }
            pos1=currentPos;
            q1=Quaternion.Euler(phantomObject.transform.rotation.x,currentRot,phantomObject.transform.rotation.z);
            splineParent.FirstPointSpline((p1,pos1,q1));
        }
        else if(pointCount==1)
        {
            if(port!=null)
            {
                p2 =port;
                p2.PortUpdateFrom(splineParent);
            }
            pos2=currentPos;
            q2=Quaternion.Euler(phantomObject.transform.rotation.x,
            q1.eulerAngles.y+(splineType==SplineType.StraightAngle? (int)(currentRot/45)*45:currentRot),
            phantomObject.transform.rotation.z);
            
            splineParent.SecondPointSpline((p2,pos2,q1),splineType,SplineState.Active);
            
           
            var newSplineParent=BuildingFactory.Create(buildingInfo,currentPos,q1) as SplineParent;
            
            newSplineParent.FirstPointSpline((p1,pos1,q1));
            newSplineParent.SecondPointSpline((p2,pos2,q2),splineType,SplineState.Passive);
            
		    PhantomParent newphantomObject=PhantomCreater.CreatePhantomObject(newSplineParent);
		    newphantomObject.transform.parent=buildingStructure.transform;
		    buildingStructure.AddPoint(newphantomObject);
        }
        /*else
        {
			p1=p2;
			p1.PortUpdateTo(splineParent);
            if(port!=null)
            {
                p2 =port;
                p2.PortUpdateFrom(splineParent);
            }
        }*/
        base.AddPoint();
        
	}
	
	public void CheckPoint()
	{
	    if(pointCount==0)
	    {
	        phantomObject.transform.position=currentPos;
		    phantomObject.transform.rotation=Quaternion.Euler(phantomObject.transform.rotation.x,currentRot,phantomObject.transform.rotation.z);
		    
	    }
	    else if(pointCount>0)
	    {
	        splineParent.SecondPointSpline((p2,currentPos,Quaternion.Euler(phantomObject.transform.rotation.x,q1.eulerAngles.y+(splineType==SplineType.StraightAngle? (int)(currentRot/45)*45:currentRot),phantomObject.transform.rotation.z)),splineType,SplineState.Active);
	    }
	}
	public override void ActionF()
	{
		buildingStructure.Init();
		MonoBehaviour.DestroyImmediate(phantomObject.gameObject);
		phantomObject=null;
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
		var amBuilding= collider.GetComponent<BuildingLogicBase>();
		var obj=collider.gameObject;
		var bx = collider as BoxCollider;
		if (amBuilding!=null)
		{
			var buildingLogic=InfoDataBase.buildingBase.GetInfo(amBuilding.id).prefab.GetComponent<BuildingLogicBase>();
			var tempBuilding = buildingInfo.prefab.GetComponent<BuildingLogicBase>();
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
			Port port=collider.GetComponent<Port>();
			if(port!=null) currentPos=port.point.position;
			else currentPos=pos;
		}
	}
}