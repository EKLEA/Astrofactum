using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteryPoint : MonoBehaviour
{
	public TextAsset recipeJson;
	public UIManager uIManager;
	public TextAsset enumImageJson;
	public WorldController worldController;
	
	
	public void Awake()
	{
		
		RecipeManager.Init(recipeJson);
		InfoDataBase.InitBases();
		BuildingsImagesManager.LoadImages(enumImageJson);
		worldController.Init();
		uIManager.Init();
		
	}
}
