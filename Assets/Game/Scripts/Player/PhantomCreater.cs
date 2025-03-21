using Unity.Mathematics;
using UnityEngine;

public class PhantomCreater
{
    public static PhantomObject CreatePhantomObject(string id,Quaternion rot,Vector3 pos)
    {
        var info= InfoDataBase.buildingBase.GetInfo(id);
        var phantomObject=MonoBehaviour.Instantiate(info.prefab);
        phantomObject.GetComponent<BuildingLogicBase>()?.Init(id);
        phantomObject.transform.rotation=rot;
        phantomObject.transform.position=pos;
        var ph =phantomObject.AddComponent<PhantomObject>();
        ph.Init(id);
        return ph;
        
    }
     public static PhantomSpline CreatePhantomSplne(string id,Port p1,Port p2,SplineType type)
    {
        var info= InfoDataBase.buildingBase.GetInfo(id) as SplineInfo;
        var phantomObject=MonoBehaviour.Instantiate(info.splineLogic);
        phantomObject.transform.position=(p1.transform.position+p2.transform.position)/2;
        phantomObject.GetComponent<SplineParent>().Init(id);
        phantomObject.GetComponent<SplineParent>().CreateSpline(p1,p2,type);
        var ph = phantomObject.AddComponent<PhantomSpline>();;
        ph.Init(id);
        return ph;
    }
}