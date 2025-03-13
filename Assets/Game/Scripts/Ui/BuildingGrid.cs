using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingGrid : UIController
{
    public override void Init(string type)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Dictionary<string,BuildingInfo> infos = new Dictionary<string,BuildingInfo>(InfoDataBase.buildingBase.GetBase()
        .Where(f=>f.Value.buildingType==(BuildingsTypes)Enum.Parse(typeof(BuildingsTypes), type))
		.ToDictionary(f => f.Key, f => f.Value as BuildingInfo));
        foreach (var en in infos)
        {
            ActionButton s =Instantiate(UIManager.Instance.actionButtonExample,transform);
            s.SetUpButton(en.Key,this);
        }
    
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        EditWorldController.Instance.SetUpAction(id);
        this.gameObject.SetActive(false);
    }
}