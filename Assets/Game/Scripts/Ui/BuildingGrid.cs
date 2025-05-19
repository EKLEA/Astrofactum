using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BuildingGrid : UIController
{
    public GameObject grid;
    UIManager uiManager=>UIManager.Instance;
    WorldController worldController=>WorldController.Instance;
    public override void Init(string type)
    {
        foreach (Transform child in grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Dictionary<string,BuildingInfo> infos = new Dictionary<string,BuildingInfo>(InfoDataBase.buildingBase.GetBase()
        .Where(f=>f.Value.buildingType==(BuildingsTypes)Enum.Parse(typeof(BuildingsTypes), type))
		.ToDictionary(f => f.Key, f => f.Value as BuildingInfo));
		foreach(var key in worldController.NotAllowedBuilding)
		{
            infos.Remove(key.id);
		}
        foreach (var en in infos)
        {
            ActionButton s =Instantiate(uiManager.actionButtonExample,grid.transform);
            s.SetUpButton(en.Key,en.Value.title,en.Value.icon,this);
        }
    
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        EditWorldController.Instance.SetUpAction(id);
        Disable();
    }
}