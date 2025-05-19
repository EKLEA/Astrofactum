using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance;
    
    public BuildingInfo[] NotAllowedBuilding;
    [Header("SetUpRecipe")]
    public ResourceGeneratorPair[] resourceGenerators;
    public bool IsPaused => tickManager.isPaused;
    public void Init()
    {
        Instance = this;
        for(int i = 0; i < resourceGenerators.Length; i++)
        {
            resourceGenerators[i].generator.Init("drill");
            resourceGenerators[i].generator.SetUpReciepe(resourceGenerators[i].recipeID);
            resourceGenerators[i].generator.canChange = false;
            
        }
    }
    public TickManager tickManager;
    public void Pause()
    {
        tickManager.Pause();
    }
    public void UnPause()
    {
        tickManager.UnPause();
    }
    public void StartLevel()
    {
        tickManager.StartTick();
    }
    public void StopLevel()
    {
        tickManager.StopTick();
    }
}
[System.Serializable]
public class ResourceGeneratorPair
{
    public ResourceGenerator generator;
    public string recipeID;
}
