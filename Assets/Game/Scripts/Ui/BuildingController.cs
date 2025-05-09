using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class BuildingController : UIController
{
    public event Action OnUIClose;
    public event Action<string> OnReciepeChoosed;
    public UIController ChooseRecipeController;
    public Image WorkIndicator;
    public TextMeshProUGUI Name;
    public Transform recipeTranform;
    public Image recipeIcon;
    public Button DestroyBT;
    public Button ClearBT;
    IWorkWithRecipe workWithRecipe;
    IWorkWithItems workWithItems;
    IAmTickable amTickable;
    Building _building;
    public void Init(Building building)
    {
        Disable();
        Enable();
        Debug.Log("opeeeeeen");
        _building=building;
        workWithRecipe=_building.GetComponent<IWorkWithRecipe>();
        workWithItems=_building.GetComponent<IWorkWithItems>();
        amTickable=_building.GetComponent<IAmTickable>();
        Name.text=_building.id;
        if(amTickable!=null)
        {
            WorkIndicator.gameObject.SetActive(true);
            amTickable.onStateChanged+=ChangeColor;
        }
        if(workWithRecipe!=null)
        {
            recipeTranform.gameObject.SetActive(true);
        }
        if(workWithItems!=null)ClearBT.gameObject.SetActive(true);
    }
    public void OnUIClosedInvoke()
    {
        OnUIClose?.Invoke();
        Disable();
    }
    public void ResetEvents()
    {
        OnUIClose=null;
    }
    public void DestroyBuilding()
    {
        _building?.Destroy();
        OnUIClosedInvoke();
    }
    public void ClearBuilding()
    {
        workWithItems?.Clear();
    }
    void ChangeColor(ProcessionState state)
    {
        WorkIndicator.color=state.GetColorOfState();
    }
    public override void Enable()
    {   
        foreach(var gm in GetComponentsInChildren<Transform>(includeInactive:true))
        {
            gm.gameObject.SetActive(true);
        }
        
        ClearBT.gameObject.SetActive(false);
        WorkIndicator.gameObject.SetActive(false);
        recipeTranform.gameObject.SetActive(false);
        
        
        base.Enable();
    }
    public override void Disable()
    {
        if(amTickable!=null)  amTickable.onStateChanged-=ChangeColor;
        foreach(var gm in GetComponentsInChildren<Transform>(includeInactive:true))
        {
            gm.gameObject.SetActive(false);
        }
        base.Disable();
    }
}