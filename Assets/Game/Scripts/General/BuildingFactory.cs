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