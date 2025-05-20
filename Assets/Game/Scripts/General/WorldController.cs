using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance;
    
    public BuildingInfo[] NotAllowedBuilding;
    [Header("SetUpRecipe")]
    
    public bool IsPaused => tickManager.isPaused;
    public void Init()
    {
        Instance = this;
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
