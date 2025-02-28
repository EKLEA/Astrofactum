using System.Collections.Generic;
using UnityEngine;

public class BuildSplineConstruction : BuildConstruction
{
	protected PhantomObjParent splineHandler;
	public BuildSplineConstruction(string id) : base(id){}

	public override void UpdateFunc()
	{
		base.UpdateFunc();
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
}