using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfoDataBase
{
	public static DataBase<string,BuildingInfo> buildingBase;
	public static DataBase<string,BuildingStructure> structuresBase;
	public static void InitBases()
	{
		buildingBase= new DataBase<string,BuildingInfo>("Buildings", building=>building.id);
		structuresBase= new DataBase<string,BuildingStructure>("Structures", building=>building.idStructure);
	}
}
