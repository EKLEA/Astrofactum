using System;
using Unity.VisualScripting;
using UnityEngine;

public class ConstructionGrid : UIController
{
    public UIController buildingsGrid;
    public Transform buttons;
    public override void Init()
    {
        foreach (var en in Enum.GetValues(typeof(BuildingsTypes)))
        {
            ActionButton s =Instantiate(UIManager.Instance.actionButtonExample,buttons);
            s.SetUpButton(en.ToString(),this);
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        buildingsGrid.gameObject.SetActive(true);
        buildingsGrid.Init(id);
        buildingsGrid.transform.position= new Vector3(button.transform.position.x,buildingsGrid.transform.position.y,buildingsGrid.transform.position.z);
    }
}