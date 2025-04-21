using System.Collections.Generic;
using UnityEngine;

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
    public float Duration { get; set; }
    public List<ItemStack> Inputs { get; set; } = new List<ItemStack>();
    public List<ItemStack> Outputs { get; set; } = new List<ItemStack>();
    public RecipeTag Tag { get; set; }
}
public enum RecipeTag
{
    Construction,
    Smelting,
    Assembling,
    Generator
}
