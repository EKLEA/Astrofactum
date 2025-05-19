using System;
using System.Collections.Generic;

public interface IWorkWithRecipe
{
    public string recipeID{get;}
    public RecipeTag recipeTag{get;}
    public float duration{get;}
    public void SetUpReciepe(string id){}
    public bool canChange{get;set;}
    
    public event Action OnUIUpdate;
}
   