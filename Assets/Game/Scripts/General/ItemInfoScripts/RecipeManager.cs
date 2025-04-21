using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using System;

public class RecipeManager : MonoBehaviour
{
    static TextAsset _recipeJson;
    public static void Init(TextAsset recipeJson )
    {
        _recipeJson=recipeJson;
    }
    public static IReadOnlyDictionary<string, Recipe> LoadRecipesFromJson()
    {
        var recipes = new Dictionary<string, Recipe>();
        var json = JSON.Parse(_recipeJson.text);
        
        foreach (KeyValuePair<string, JSONNode> entry in json.AsArray)
        {
            Recipe recipe = new Recipe();
            recipe.Id = entry.Value["id"].Value;
            recipe.Duration = entry.Value["duration"].AsFloat;

            recipe.Inputs = new List<ItemStack>();
            foreach (JSONNode input in entry.Value["inputs"].AsArray)
            {
                recipe.Inputs.Add(new ItemStack(input["itemID"].Value,input["amount"].AsInt));
            }
            recipe.Outputs = new List<ItemStack>();
            foreach (JSONNode output in entry.Value["outputs"].AsArray)
            {
                recipe.Outputs.Add(new ItemStack(output["itemID"].Value, output["amount"].AsInt));
            }

            // Загрузка тега и станций
            recipe.Tag = (RecipeTag)Enum.Parse(typeof(RecipeTag), entry.Value["tag"].Value);
            recipes.Add(recipe.Id, recipe);
        }
        
        return recipes;
    }
}
