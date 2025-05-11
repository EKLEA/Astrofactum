using System;
using System.Collections.Generic;

public interface IWorkWithRecipe
{
    public string recipeID{get;}
    public RecipeTag recipeTag{get;}
    public float duration{get;}
    public void SetUpReciepe(string id){}
    
    public event Action OnUIUpdate;
}
   