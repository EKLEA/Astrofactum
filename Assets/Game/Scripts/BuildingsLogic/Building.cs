using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
	public string id {get {return _id;}}

    public bool IsWork{get{return _isWork;} private set{_isWork = value;}}
    protected bool _isWork;
    protected string _id;
	public virtual void Init(string id)
	{
		_id=id;
	}
}
