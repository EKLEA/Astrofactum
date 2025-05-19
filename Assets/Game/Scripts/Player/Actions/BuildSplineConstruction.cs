using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildSplineConstruction : ActionWithWorld
{
    BuildingInfo buildingInfo;
    BuildingStructure buildingStructure;
	SplineParent splineParent;
	PhantomParent phantomObject; 
    Port port,firstport,secondport,lastPort;
    SplineType splineType=>SplineType.StraightAngle;
    string _id;
    List<GameObject>crossColliders=new();
    SplineParent prevSpline;
    bool connected;
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
		splineParent.LogicInPort.arrow.Enable();
		splineParent.LogicOutPort.arrow.Enable();
	}
	public override void UpdateFunc()
	{
		ChangePos(hit.point,hit.collider);
		crossColliders=splineParent.GetAllCollisionsAlongSpline().ToList();
		crossColliders.Remove(firstport?.GetComponentInParent<Building>().gameObject);
		canAction=ValidateBuild(currentPos);
		port= hit.collider.GetComponent<Port>();
		phantomObject?.ChangeColor(canAction);
		CheckPoint();
	}
	public override void RightClick()
	{
		if (pointCount>=1)
		{
			pointCount=0;
			firstport=null;
			splineParent.Reset();
			
		}
		else 
		{
		    ActionL();
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
			if(port!=null)
            {
				if(port.portDir==PortDir.Out)
				{
				    splineParent.SetUpInPort(port);
				    splineParent.SetFirstPointSpline(currentPos,Quaternion.Euler(0,currentRot,0));
				}
            	else
            	{
            	    splineParent.SetUpOutPort(port);
            	    splineParent.SetSecondPointSpline(currentPos,Quaternion.Euler(0,currentRot,0));
            	}
            }
            else
            {
				splineParent.SetFirstPointSpline(currentPos,Quaternion.Euler(0,currentRot,0));
            }
            firstport?.arrow.Disable();
			firstport=port;
			
        }
        else 
        {
			if(secondport!=null)
			{
			    ActionL();
			}
			else
			{
			    if(port!=null)
			    {
			        if(firstport==null|| firstport.portDir==PortDir.Out&&port.portDir==PortDir.In) splineParent.SetUpOutPort(port);
			    	else splineParent.SetUpInPort(port);
			    	secondport=port;
			    }
			    else splineParent.SetUpOutPort(port);
			    splineParent.DrawSpline(splineType,SplineState.Passive);
			    buildingStructure.AddPoint(phantomObject);
			    splineParent.LogicInPort.arrow.Disable();
				splineParent.LogicOutPort.arrow.Disable();
				prevSpline=splineParent;
				NewPhantom();
				if(secondport!=null) ActionL();
				if(firstport==null)
				{
					firstport=prevSpline.OutPorts[0];
				}
				else
				{
					if(firstport.portDir==PortDir.Out)
					{
						firstport=prevSpline.OutPorts[0];
						splineParent.SetUpInPort(firstport);
					}
					else
					{
						firstport=prevSpline.InPorts[0];
						splineParent.SetUpOutPort(firstport);
					}
				}
			}
			
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
		else
		{
			var rot=port==null?Quaternion.Euler(0,currentRot/45*45,0):port.point.transform.rotation;
			
			
			if(firstport==null||firstport.portDir==PortDir.Out) splineParent.SetSecondPointSpline(currentPos,rot);
			else splineParent.SetFirstPointSpline(currentPos,rot);
			splineParent.DrawSpline(splineType,SplineState.Active);
			
		}
	}
	public override void ActionL()
	{
		if (phantomObject != null) 
			MonoBehaviour.DestroyImmediate(phantomObject.gameObject);
		
		phantomObject = null;
		port = null;          // <- Обнуляем port
		firstport = null;     // <- Обнуляем firstport
		secondport = null;    // <- Обнуляем secondport
		
		if (buildingStructure != null && buildingStructure._buildings.Count == 0)
		{
			MonoBehaviour.DestroyImmediate(buildingStructure.gameObject);
			buildingStructure = null;
		}
		
		if (buildingStructure != null) 
			buildingStructure.Init();
		
		base.ActionL();
	}
	public override void MouseWheelRotation(float Value)
	{
	
		currentRot=  Value*30+currentRot;
	}

	protected Vector3 SnapToGrid(Vector3 point)
	{
		float x = Mathf.Round(point.x / 2f) * 2f;
		float z = Mathf.Round(point.z / 2f) * 2f;
		return new Vector3(x, point.y, z);
	}
	public override bool ValidateBuild(Vector3 pos)
	{	
		if(connected) return true;
		if(splineParent.GetLenght()>splineParent.maxLenght)return false;
		if(pointCount>0)
			if(!splineParent.ValidateSpline()) return false;
		
		foreach(var crossCollider in crossColliders)
			if(crossCollider!=null&&crossCollider.tag=="Building")return false;
			
		return true;
	}
	protected void ChangePos(Vector3 pos, Collider collider)
	{
		var amBuilding= collider.GetComponent<Building>();
		var obj=collider.gameObject;
		var bx = collider as BoxCollider;
		if (amBuilding!=null)
		{
			if(amBuilding.id==null) return;
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
			connected=false;
		}
		else
		{	
			if(port!=null&&(pointCount==0||firstport!=null&&firstport.portDir!=port.portDir||firstport==null&&port.portDir==PortDir.In))
			{
			    currentPos=port.point.position;
			    currentRot=port.transform.rotation.eulerAngles.y;
			    connected=true;
			    lastPort=port;
			    lastPort.arrow.Enable();
			    if(port.portDir==PortDir.In)splineParent.LogicOutPort.arrow.Disable();
			    else splineParent.LogicInPort.arrow.Disable();
			}
			else
			{
			    currentPos=pos;
			    connected=false;
			    if (lastPort != null)
				{
					lastPort.arrow.Disable();
					lastPort = null;
				}
			}
		}
		
	}
	
}