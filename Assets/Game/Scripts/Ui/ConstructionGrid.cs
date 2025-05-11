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
        foreach (BuildingsTypes buildingType in Enum.GetValues(typeof(BuildingsTypes)))
        {
            string localizedName = buildingType.GetStringOfBuildingsTypes();
            
            Sprite icon = BuildingsImagesManager.GetBuildingImage(buildingType);
            ActionButton button = Instantiate(UIManager.Instance.actionButtonExample, buttons);
            
            button.SetUpButton(
                buildingType.ToString(),  
                localizedName,          
                icon,                   
                this
            );

            bool hasBuildings = InfoDataBase.buildingBase.GetBase()
                .Any(f => f.Value.buildingType == buildingType);
                
            button.interactable = hasBuildings;
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        buildingsGrid.Init(id);
        buildingsGrid.gameObject.SetActive(true);
        buildingsGrid.transform.position= new Vector3(button.transform.position.x,buildingsGrid.transform.position.y,buildingsGrid.transform.position.z);
    }
}