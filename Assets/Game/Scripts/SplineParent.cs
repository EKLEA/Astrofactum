using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent ( typeof (SplineInstantiate),typeof (SplineContainer))]
public class SplineParent : MonoBehaviour, IAmBuilding
{
    public string id{get{return _id;}}
    public float speed;
    public event Action OnItemOnEnd;
    public bool IsWork{get{return _isWork;} private set{_isWork = value;}}
    List<SplineItem> splineItems;
    int maxCount;
    int curCount;
    string _id;
    Spline spline;
    private bool _isWork;

    SplineInstantiate splineInstantiate=>GetComponent<SplineInstantiate>();
    SplineContainer container=>GetComponent<SplineContainer>();

    

    public void Init(string tid)
    {
        _id=tid;
        spline=new Spline();
        container.AddSpline(spline);
        var prefab = InfoDataBase.buildingBase.GetInfo(_id).prefab;
        var newItem = new SplineInstantiate.InstantiableItem { Prefab = prefab };
        splineInstantiate.itemsToInstantiate = new SplineInstantiate.InstantiableItem[] { newItem };
        //count через длину сплайна
    }
    public void DrawSpline(SplineType type)
    {
        
    }
    public bool AddToSpline(SplineItem item)
    {
        if(curCount!=maxCount)
        {
           
            splineItems.Add(item);
            curCount++;
            if(!_isWork)
            {
                StartCoroutine(ProccessingCoroutine());
                _isWork=true;
            }
            return true;
        }
        else return false;
    }
    public void Proccessing()
    {
        while(splineItems.Contains(null))
        {
        //перебор индексов и передвижение
        
            OnItemOnEnd?.Invoke();
        }
        _isWork=false;
        StopAllCoroutines();
    }
    public IEnumerator ProccessingCoroutine()
    {
        while(true)
        {
            Proccessing();
        }
    }
    
}
public enum SplineType
{
    StaightAngle,
    Directly,
}