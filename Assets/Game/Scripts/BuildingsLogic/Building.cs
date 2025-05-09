using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
	public string id {get {return _id;}}
	
    protected string _id;
	public virtual void Init(string id)
	{
		_id=id;
	}
	public virtual void Destroy()
	{
	    DestroyImmediate(this.gameObject);
	}
}
