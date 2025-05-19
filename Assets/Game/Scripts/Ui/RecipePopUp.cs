using System;
using System.Linq;
using UnityEngine;

public class RecipePopUp : UIController
{
    public GameObject grid;
    ActionButton[] buttonsPool=new ActionButton[16];
    public event Action<string> OnChoosedRecipe;
    UIManager uiManager;
    void InitBT()
    {
        for(int i=0; i<buttonsPool.Length; i++)
        {
            buttonsPool[i]=Instantiate(uiManager.actionButtonExample,grid.transform);
            buttonsPool[i].gameObject.SetActive(false);
            
        }
    }
    public void Init(RecipeTag recipeTag)
    {
        uiManager=UIManager.Instance;
        Disable();
        Enable();
        var info =InfoDataBase.recipeBase.Where(f=>f.Value.Tag==recipeTag).ToArray();
        for(int i=0; i<info.Count();i++)
        {
            buttonsPool[i].SetUpButton(info[i].Key,info[i].Value.Title,info[i].Value.Icon,this);
            buttonsPool[i].gameObject.SetActive(true);
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    {
        OnChoosedRecipe?.Invoke(id);
        Disable();
    }
    public override void Enable()
    {
        if(buttonsPool.Count(f=>f==null)>0) InitBT();
        for(int i=0; i<buttonsPool.Length; i++)
        {
            buttonsPool[i].gameObject.SetActive(false);
            buttonsPool[i].onClick.RemoveAllListeners();
        }
        base.Enable();
    }
    public override void Disable()
    {
        base.Disable();
    }
}