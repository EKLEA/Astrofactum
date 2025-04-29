using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ActionTypes 
{
    BaseAction,
	BuildStructure,
	BuildManyPointStructure,
	EditTerrain,
	BuildTerrainStructure,
}
public static class BuildModeExtensions
{
    public static int GetMinPoints(this ActionTypes type)
    {
        return type switch
        {
            //ActionTypes.BaseAction=>0,
            ActionTypes.BuildStructure => 1,
            ActionTypes.BuildManyPointStructure => 2,
            ActionTypes.EditTerrain => 2,
            ActionTypes.BuildTerrainStructure => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}