using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InfoDataBase
{
	public static DataBase<string,FreatureInfo> freaturesBase;
	public static DataBase<string,ActionInfo> actionsBase;
	public static DataBase<string,BuildingInfo> buildingBase;
	public static DataBase<string,TerrainInfo> terrainBase;
	public static DataBase<string,Material> materialBase;
	public static DataBase<string,ItemInfo> itemInfoBase;
	public static DataBase<string,BuildingStructure> structuresBase;
	public static IReadOnlyDictionary<string,Recipe> recipeBase;
	public static void InitBases()
	{
		freaturesBase= new DataBase<string,FreatureInfo>("Freatures", freature=>freature.id);
		
		actionsBase=new DataBase<string,ActionInfo>(freaturesBase.GetBase()
		.Where(f=>f.Value.FreatureType==FreatureType.Action)
		.ToDictionary(f=>f.Key,f=>f.Value as ActionInfo));
		
		itemInfoBase=new DataBase<string, ItemInfo>(freaturesBase.GetBase()
		.Where(f=>f.Value.FreatureType==FreatureType.item)
		.ToDictionary(f=>f.Key,f=>f.Value as ItemInfo));
		
		materialBase= new DataBase<string,Material>("Materials", material=>material.name);
		
		buildingBase= new DataBase<string,BuildingInfo>(actionsBase.GetBase()
		.Where(f=>f.Value.actionType==ActionTypes.BuildStructure||f.Value.actionType==ActionTypes.BuildManyPointStructure)
		.ToDictionary(f => f.Key, f => f.Value as BuildingInfo));
		
		terrainBase= new DataBase<string,TerrainInfo>(actionsBase.GetBase()
		.Where(f=>f.Value.actionType==ActionTypes.EditTerrain)
		.ToDictionary(f => f.Key, f => f.Value as TerrainInfo));
		
		recipeBase=RecipeManager.LoadRecipesFromJson();
		
		
	}
}
