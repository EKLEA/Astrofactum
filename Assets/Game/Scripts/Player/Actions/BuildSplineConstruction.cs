using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildSplineConstruction :BuildConstruction
{
    public BuildSplineConstruction(string id) : base(id){}
    
    Port first,last;
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
        if(port==null)
        {
            var st =MonoBehaviour.Instantiate((buildingInfo as SplineInfo).splineHandler,buildingStructure.transform);
            st.transform.position=currentPos;
            st.transform.rotation=Quaternion.Euler(0,currentRot,0);
            if(first==null) first=st.GetComponent<SplineHandler>().OutPorts[0];
            else if(last==null) last=st.GetComponent<SplineHandler>().inPorts[0];
        }
        else
        {
            if(first==null) first=port;
            else if(last==null) last=port;
        }
        if(first!=null&&last!=null)
        {
            buildingStructure.AddPoint(PhantomCreater.CreatePhantomSplne(buildingInfo.id,first,last,SplineType.StraightAngle));
            first=last;
            last=null;
        }
		base.AddPoint();
		
	}
}