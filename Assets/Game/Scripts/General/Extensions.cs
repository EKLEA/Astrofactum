using System;
using UnityEngine;
public static class Extensoins
{
    public static int GetMinPoints(this ActionTypes type)
    {
        return type switch
        {
            ActionTypes.BaseAction=>0,
            ActionTypes.BuildStructure => 1,
            ActionTypes.BuildManyPointStructure => 2,
            ActionTypes.EditTerrain => 2,
            ActionTypes.BuildTerrainStructure => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    public static Color GetColorOfState(this ProcessionState state)
    {
        var settings = TickManager.Instance.stateColorSettings;
        
        return state switch
        {
            ProcessionState.Processed => settings.processedColor,
            ProcessionState.AwaitForInput => settings.waitingExitColor,
            ProcessionState.AwaitForOutput => settings.waitingEntryColor,
            _ => settings.defaultColor
        };
    }
}