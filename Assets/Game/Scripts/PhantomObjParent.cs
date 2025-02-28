using System.Collections;
using UnityEngine;

public class PhantomObjParent : MonoBehaviour,IAmBuilding
{
  
	protected Material previewMaterialTrue{get{return InfoDataBase.materialBase.GetInfo("True");}} 
	protected Material previewMaterialFalse{get{return InfoDataBase.materialBase.GetInfo("False");}}
	public string id {get{return _id;}}

    public bool IsWork => throw new System.NotImplementedException();

    public GameObject gmForMesh;
	protected string _id;
	protected virtual void SetUpPhantom(){}
	protected virtual void AddPartMesh(Transform obj){}
	public virtual void ChangeColor(bool flag){}
	public virtual void UnPhantom(){}
	public virtual void Init(string id){_id=id;}

    public void Proccessing()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ProccessingCoroutine()
    {
        throw new System.NotImplementedException();
    }
}
public class PhantomObjCreater
{
	public static PhantomObjParent CreatePhantomObject(string id,Transform parent, Vector3 pos,float roty)
	{
		GameObject gm =new GameObject(id);
		gm.transform.position = pos;
		gm.transform.rotation = Quaternion.Euler(0, roty,0);
		gm.transform.parent = parent;
		var info = InfoDataBase.buildingBase.GetInfo(id);
		PhantomObjParent s;
		
		switch(info.actionType)
		{
			
			case ActionTypes.BuildStructure:
				s=gm.AddComponent<PhantomObject>();
				break;
			case ActionTypes.BuildManyPointStructure:
				s=gm.AddComponent<PhantomSplineObject>();
				break;
			default:
				s=gm.AddComponent<PhantomObjParent>();
				break;
		}
		
		var gmForMes=new GameObject("gmForMesh");
		gmForMes.transform.position = pos;
		gmForMes.transform.rotation = Quaternion.Euler(0, roty,0);
		s.gmForMesh=gmForMes;
		s.Init(id);
		return s;
	}
}
