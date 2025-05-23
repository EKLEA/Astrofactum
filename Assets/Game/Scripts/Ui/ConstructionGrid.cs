using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class ConstructionGrid : UIController
{
    public UIController buildingsGrid;
    public RectTransform BuildingTransf;
    public Transform buttons;
    UIManager uiManager=>UIManager.Instance;
    WorldController worldController=WorldController.Instance;
    public override void Init()
    {
        foreach (BuildingsTypes buildingType in Enum.GetValues(typeof(BuildingsTypes)))
        {
            string localizedName = buildingType.GetStringOfBuildingsTypes();
            
            Sprite icon = BuildingsImagesManager.GetBuildingImage(buildingType);
            ActionButton button = Instantiate(uiManager.actionButtonExample, buttons);
            
            button.SetUpButton(
                buildingType.ToString(),  
                localizedName,          
                icon,                   
                this
            );
            List<BuildingInfo> buildingId = InfoDataBase.buildingBase.GetBase().Values.ToList();
            buildingId.RemoveAll(f=>worldController.NotAllowedBuilding.Contains(f));
            bool hasBuildings = buildingId.Any(f => f.buildingType == buildingType);
                
            button.interactable = hasBuildings;
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        buildingsGrid.Init(id);
        buildingsGrid.gameObject.SetActive(true);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, button.rectTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BuildingTransf.parent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );
        BuildingTransf.anchoredPosition = new Vector2(localPos.x, 510f);
    }
}