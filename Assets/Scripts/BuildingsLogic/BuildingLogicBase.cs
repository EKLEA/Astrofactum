using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogicBase : MonoBehaviour, IAmBuilding
{
	public GameObject[] inPorts => GameObject.FindGameObjectsWithTag("In");
	public  GameObject[] OutPorts => GameObject.FindGameObjectsWithTag("Out");

	public string id {get {return id;}}
	string _id;
	public virtual void Init(string id)
	{
		_id=id;
	}
}
