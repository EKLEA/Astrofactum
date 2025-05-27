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
        levelTaskController.OnTaskDone += EndGame;
        levelTaskController.SetUpTask();
        StartLevel();
    }
    public TickManager tickManager;
    public LevelTaskController levelTaskController;
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
    void EndGame(int score)
    {
        
        StopLevel();
        //Запрос
        //чет делать с очками
        Debug.Log("Конец");
    }
}
