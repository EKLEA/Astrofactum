using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemStack
{
    public string id;
    public int amount;

    public ItemStack(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}

public class Recipe
{
    public string Id { get; set; }
    public string Title { get; set; }
    public float Duration { get; set; }
    
    public Sprite Icon { get; set; }  // Добавлено поле для спрайта
    public RecipeTag Tag { get; set; }
    public List<ItemStack> Inputs { get; set; }
    public List<ItemStack> Outputs { get; set; }
}
public enum RecipeTag
{
    Construction,
    Smelting,
    Assembling,
    Generator
}
