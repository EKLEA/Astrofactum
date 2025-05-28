using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Zenject.SpaceFighter;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance;
    public GameObject WinPanel;

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

    private AttemptSender attemptSender;

    void EndGame(int score)
    {
        StopLevel();
        Debug.Log("Конец");

        string currentCode = GameSessionData.ActiveCode;
        double elapsedTime = Math.Round(tickManager.ElapsedTime, 1);

        if (currentCode == "123")
        {
            Debug.Log($"[Псевдвокод: {currentCode}] - Очки {score}; Время {elapsedTime}");
        }
        else
        {
            attemptSender = FindAnyObjectByType<AttemptSender>();
            if (attemptSender != null)
            {
                Debug.Log($"[Нормискод: {currentCode}] - Очки {score}; Время {elapsedTime}");
                attemptSender.SendTestResults(score, elapsedTime);
            }
        }

        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("WinPanel не найден!");
        }
    }

    public void ReturnToMainMenu()
    {
        SceneController.Instance.ReturnToMainMenu();
    }
}
