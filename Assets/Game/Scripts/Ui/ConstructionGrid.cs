using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ConstructionGrid : UIController
{
    public UIController buildingsGrid;
    RectTransform rectTransform;
    public Transform buttons;
    public override void Init()
    {
        foreach (var en in Enum.GetValues(typeof(BuildingsTypes)))
        {
            ActionButton s =Instantiate(UIManager.Instance.actionButtonExample,buttons);
            s.SetUpButton(en.ToString(),this);
            s.interactable=InfoDataBase.buildingBase.GetBase().Where(f=>f.Value.buildingType==(BuildingsTypes)Enum.Parse(typeof(BuildingsTypes), en.ToString())).Count()>0;
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        buildingsGrid.Init(id);
        buildingsGrid.gameObject.SetActive(true);
        buildingsGrid.transform.position= new Vector3(button.transform.position.x,buildingsGrid.transform.position.y,buildingsGrid.transform.position.z);
    }
}