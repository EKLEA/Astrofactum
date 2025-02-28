using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InfoDataBase
{
	public static DataBase<string,FreatureInfo> freaturesBase;
	public static DataBase<string,BuildingInfo> buildingBase;
	public static DataBase<string,TerrainInfo> terrainBase;
	public static DataBase<string,Material> materialBase;
	
	public static DataBase<string,BuildingStructure> structuresBase;
	public static void InitBases()
	{
		freaturesBase= new DataBase<string,FreatureInfo>("Freatures", freature=>freature.id);
		materialBase= new DataBase<string,Material>("Materials", material=>material.name);
		
		buildingBase= new DataBase<string,BuildingInfo>(freaturesBase.GetBase()
		.Where(f=>f.Value.actionType==ActionTypes.BuildStructure)
		.ToDictionary(f => f.Key, f => f.Value as BuildingInfo));
		
		terrainBase= new DataBase<string,TerrainInfo>(freaturesBase.GetBase()
		.Where(f=>f.Value.actionType==ActionTypes.EditTerrain)
		.ToDictionary(f => f.Key, f => f.Value as TerrainInfo));
		
	}
}
