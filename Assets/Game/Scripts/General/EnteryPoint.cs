using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnteryPoint : MonoInstaller
{
	public TextAsset recipeJson;
	public UIManager uIManager;
	public override void InstallBindings()
	{
		RecipeManager.Init(recipeJson);
		InfoDataBase.InitBases();
	}
}
