using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BuildingController : UIController
{
    public ActionsGrid actionsGrid;
    [Header("Building")]
    public TextMeshProUGUI BuildingName;
    public Button DestroyBT;
    public Button ClearBT;
    [Header("Recipe")]
    public RecipePopUp ChooseRecipeController;
    public Transform recipeTranform;
    public ActionButton recipeBT;
    public Transform InputTransform;
    public Transform OutputTransform;
    public UIItem Example;
    [Header("WordBuilding")]
    public Image WorkIndicator;
    
    
    
    public event Action OnUIClose;
    (Slot,UIItem)[] inputUI=new  (Slot,UIItem)[4];
    (Slot,UIItem)[] outputUI=new (Slot,UIItem)[4];
    IWorkWithRecipe workWithRecipe;
    IWorkWithItems workWithItems;
    IAmTickable amTickable;
    Building _building;
    Recipe _recipe;
    
    void InitBT()
    {
        
        for(int i = 0; i < inputUI.Length;i++)
        {
            inputUI[i]=(null,Instantiate(Example,InputTransform));
            inputUI[i].Item2.gameObject.SetActive(false);
        }
        for(int i = 0; i < outputUI.Length;i++)
        {
            outputUI[i]=(null,Instantiate(Example,OutputTransform));
            outputUI[i].Item2.gameObject.SetActive(false);
        }
    }
    public void Init(Building building)
    {
        Disable();
        Enable();
        _building=building;
        workWithRecipe=_building.GetComponent<IWorkWithRecipe>();
        workWithItems=_building.GetComponent<IWorkWithItems>();
        amTickable=_building.GetComponent<IAmTickable>();
        BuildingName.text=_building.title;
        if(amTickable!=null)
        {
            WorkIndicator.transform.parent.gameObject.SetActive(true);
            amTickable.onStateChanged+=ChangeColor;
        }
        if(workWithRecipe!=null)
        {
            
            for(int i = 0; i < inputUI.Length;i++)
            {
                inputUI[i].Item2.gameObject.SetActive(false);
            }
            for(int i = 0; i < outputUI.Length;i++)
            {
                outputUI[i].Item2.gameObject.SetActive(false);
            }
            workWithRecipe.OnUIUpdate+=UpdateUI;
            if(workWithRecipe.recipeID!=null) 
            {
                var recipe=InfoDataBase.recipeBase[workWithRecipe.recipeID];
                SetUpRecipeTranform(recipe);
                
            }
            else
            {
                recipeBT.image.enabled=false;
                recipeBT.textMeshPro.text="";
            }
            
            recipeTranform.gameObject.SetActive(true);
            ChooseRecipeController.OnChoosedRecipe+=SetUpRecipe;
        }
        if(workWithItems!=null)ClearBT.gameObject.SetActive(true);
    }
    public void OpenChooseRecipe()
    {
        ChooseRecipeController.Init(workWithRecipe.recipeTag);
    }
    void UpdateUI()
    {
        var inp=inputUI.Where(f=>f.Item1!=null).ToArray();
        if(inp.Count()>0)
        {    
            for(int i = 0; i < inp.Count();i++)
            {
                inp[i].Item2.Amount.text=inp[i].Item1.Count.ToString();      
            } 
        }
        var o=outputUI.Where(f=>f.Item1!=null).ToArray();
        if(o.Count()>0)
        {
            for(int i = 0; i < o.Count();i++)
            {
                o[i].Item2.Amount.text=o[i].Item1.Count.ToString();
            }
        }
    }
    void SetUpRecipeTranform(Recipe recipe)
    {
    
        
        if (recipeBT.image != null)
        {
            recipeBT.image.sprite = recipe.Icon;
            recipeBT.image.enabled=true;
        }
        else
            Debug.LogError("Image component is missing on ActionButton!");

        if (recipeBT.textMeshPro != null)
            recipeBT.textMeshPro.text = recipe.Title;
        else
            Debug.LogError("TextMeshProUGUI is missing on ActionButton!");
            
        for(int i=0;i<recipe.Inputs.Count;i++)
        {
            var inputItem=recipe.Inputs[i];
            inputUI[i].Item1=workWithItems.inSlots.Where(x=>x.Id==inputItem.id).FirstOrDefault();
            inputUI[i].Item2.Amount.text=inputUI[i].Item1.Count.ToString();
            inputUI[i].Item2.text.text=InfoDataBase.itemInfoBase.GetInfo(inputItem.id).title;
            inputUI[i].Item2.image.sprite=InfoDataBase.itemInfoBase.GetInfo(inputItem.id).icon;
            inputUI[i].Item2.gameObject.SetActive(true);
            
        }
        for(int i=0;i<recipe.Outputs.Count;i++)
        {
            var inputItem=recipe.Outputs[i];
            outputUI[i].Item1=workWithItems.outSlots.Where(x=>x.Id==inputItem.id).FirstOrDefault();
            outputUI[i].Item2.Amount.text=outputUI[i].Item1.Count.ToString();
            outputUI[i].Item2.text.text=InfoDataBase.itemInfoBase.GetInfo(inputItem.id).title;
            outputUI[i].Item2.image.sprite=InfoDataBase.itemInfoBase.GetInfo(inputItem.id).icon;
            outputUI[i].Item2.gameObject.SetActive(true);
            
        }
        recipeBT.interactable = workWithRecipe.canChange;
    }
    void SetUpRecipe(string recipeID)
    {
        var recipe=InfoDataBase.recipeBase[recipeID];
        workWithRecipe.SetUpReciepe(recipeID);
        SetUpRecipeTranform(recipe);
        
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
        actionsGrid.Disable();
        foreach(var gm in GetComponentsInChildren<Transform>(includeInactive:true))
        {
            gm.gameObject.SetActive(true);
        }
        
        ClearBT.gameObject.SetActive(false);
        WorkIndicator.transform.parent.gameObject.SetActive(false);
        recipeTranform.gameObject.SetActive(false);
        ChooseRecipeController.Disable();
        
        
        base.Enable();
    }
    public override void Disable()
    {
        actionsGrid.Enable();
        if(amTickable!=null)  amTickable.onStateChanged-=ChangeColor;
        if(workWithRecipe!=null)
        {
             ChooseRecipeController.OnChoosedRecipe-=SetUpRecipe;
             workWithRecipe.OnUIUpdate+=UpdateUI;
        }
        
        if(inputUI.Count(f=>f.Item2==null)>0||outputUI.Count(f=>f.Item2==null)>0) InitBT();
        for(int i = 0; i < inputUI.Length;i++)
        {
            inputUI[i].Item2.gameObject.SetActive(false);
        }
        for(int i = 0; i < outputUI.Length;i++)
        {
            outputUI[i].Item2.gameObject.SetActive(false);
        }
        foreach(var gm in GetComponentsInChildren<Transform>(includeInactive:true))
        {
            gm.gameObject.SetActive(false);
        }
        base.Disable();
    }
}