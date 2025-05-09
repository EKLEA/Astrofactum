using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [SerializeField] float tickPerSecond;
    Action<float> Ontick;
    HashSet<IAmTickable> tickableOjects=new();
    public static TickManager Instance;
    public StateColorSettings stateColorSettings;
    Coroutine coroutine;
    void Awake()
    {
        if(Instance != null&& Instance!=this)  
            DestroyImmediate(this); 
        else Instance=this;
    }
    public void StartTick()
    {
        if(coroutine!=null) return;
        else coroutine=StartCoroutine(WorldTick());
    }
    public void StopTick()
    {
        if(coroutine==null) return;
        else{
            StopCoroutine(coroutine);
            coroutine=null;}
    }
    public void Subscribe(IAmTickable building)
    {
        if(tickableOjects.Add(building)) Ontick+=building.Tick;
    }
    public void Unsubscribe(IAmTickable building)
    {
        if(tickableOjects.Contains(building)) Ontick-=building.Tick;
    }
    
    public IEnumerator WorldTick()
    {
        while (true)
        {
            Ontick?.Invoke(1/tickPerSecond);
            yield return new WaitForSeconds(1/tickPerSecond);
        }
    }
}