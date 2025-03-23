using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class BuildingFactory 
{
    public static BuildingLogicBase Create(BuildingInfo data, Vector3 position,Quaternion rotation,Transform parent)
    {
        GameObject obj = GameObject.Instantiate(data.prefab, position, rotation, parent);
        BuildingLogicBase building = obj.GetComponent<BuildingLogicBase>();
        building.Init(data.id);
        return building;
    }
    public static BuildingLogicBase Create(BuildingInfo data, Vector3 position,Quaternion rotation)
    {
       return Create(data, position,rotation,null);
    }
    
}
public class PhantomCreater
{
    public static PhantomParent CreatePhantomObject(BuildingLogicBase building)
    {
        PhantomParent ph = InfoDataBase.freaturesBase.GetInfo(building.id).actionType==ActionTypes.BuildManyPointStructure?
        building.AddComponent<PhantomSpline>():building.AddComponent<PhantomObject>();
        ph.Init();
        return ph;
    }
}