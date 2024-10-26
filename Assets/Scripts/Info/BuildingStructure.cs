using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStructure:UnityEngine.Object
{
	public string idStructure;
	public Dictionary<(Vector3,Vector3),string> buildings=new();
	public BuildingStructure(string id)
	{
		buildings.Add((Vector3.zero,Vector3.zero),id);
	}
}
