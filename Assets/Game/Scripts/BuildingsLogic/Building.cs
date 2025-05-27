using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
	public string id {get {return _id;}}
	
    protected string _id;
    public string title{get;private set;}
	public bool canDestroy = true;
	public virtual void Init(string id)
	{
		_id=id;
		title=InfoDataBase.buildingBase.GetInfo(id).title;
	}
	public virtual void Destroy()
	{
		var s=GetComponentInParent<BuildingStructure>();
		if(s!=null&&s.GetComponentsInChildren<Building>().Length<2)DestroyImmediate(s.gameObject);
		else DestroyImmediate(this.gameObject);
	}
}
