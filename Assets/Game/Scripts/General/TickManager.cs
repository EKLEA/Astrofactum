using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [SerializeField] float tickPerSecond;
    Action<float> Ontick;
    HashSet<IAmTickable> tickableOjects = new();
    public static TickManager Instance;
    public StateColorSettings stateColorSettings;
    Coroutine coroutine;
    public bool isPaused;
    
    private float _elapsedTime = 0f;
    public float ElapsedTime => _elapsedTime; 

    void Awake()
    {
        if(Instance != null && Instance != this)  
            DestroyImmediate(this); 
        else Instance = this;
    }

    public void StartTick()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(WorldTick());
    }

    public float StopTick()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        return _elapsedTime;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void UnPause()
    {
        isPaused = false;
    }

    public void Subscribe(IAmTickable building)
    {
        if(tickableOjects.Add(building)) Ontick += building.Tick;
    }

    public void Unsubscribe(IAmTickable building)
    {
        if(tickableOjects.Contains(building)) Ontick -= building.Tick;
    }

    public IEnumerator WorldTick()
    {
        while (true) 
        {
            if (!isPaused)
            {
                float deltaTime = 1f / tickPerSecond;
                _elapsedTime += deltaTime; 
                Ontick?.Invoke(deltaTime);
                LevelTaskController.Instance.Timer.text = "Время: " + _elapsedTime.ToString("0.0") + " секунд";
            }
            yield return new WaitForSeconds(1f / tickPerSecond);
        }
    }

    public void ResetTimer()
    {
        _elapsedTime = 0f;
    }
}