using UnityEngine;

public class FoundationLogic : BuildingLogicBase
{
	float maxWeight;
	float currWeight;
	public override void Init(string id)
	{
		base.Init(id);
		maxWeight = InfoDataBase.buildingBase.GetInfo(id).tier*200;
	}
	public bool BuildOnStructure(float weight)
	{
		if(currWeight+weight>maxWeight) return false;
		else 
		{
			currWeight+=weight;
			return true;
		}
	}
}