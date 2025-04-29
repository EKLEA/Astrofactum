using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
public class BuildingInfoController : MonoBehaviour
{
    public event Action OnUIClose;
    public event Action<string> OnReciepeChoosed;
    public UIController ChooseRecipeController;
    public Image WorkIndicator;
    public TextMeshProUGUI Name;
    public Transform recipeTranform;
    public Image recipeIcon;
    IWorkWithRecipe workWithRecipe;
    IWorkWithItems workWithItems;
    public void Init(Building building)
    {
        workWithRecipe=building.GetComponent<IWorkWithRecipe>();
        if(workWithRecipe!=null)
        {
            workWithItems=building.GetComponent<IWorkWithItems>();
        }
    }
    public void OnUIClosedInvoke()
    {
        OnUIClose?.Invoke();
    }
    public void ResetEvents()
    {
        OnUIClose=null;
    }
}