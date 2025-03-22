using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PhantomCreater
{
    public static PhantomParent CreatePhantomObject(BuildingLogicBase building)
    {
        PhantomParent ph =InfoDataBase.buildingBase.GetInfo(building.id).actionType==ActionTypes.BuildManyPointStructure?
        building.AddComponent<PhantomSpline>(): building.AddComponent<PhantomObject>();
        ph.Init();
        return ph;
    }
}