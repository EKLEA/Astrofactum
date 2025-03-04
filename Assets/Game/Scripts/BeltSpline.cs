using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class BeltSpline : SplineParent
{
    LimetedList<SplineItem> splineItems;
    int countOfSegments;
    bool canMove;
    
    public event Action OnItemOnEnd;
    public override void Init(string tid)
    {
        countOfSegments=(int)spline.GetLength()/50;;
        splineItems = new LimetedList<SplineItem>(countOfSegments);
        splineItems.OnItemInListOnEnd+=InvokeEvent;
        base.Init(tid);
    }
    public override void SetUpToVisual(bool b)
    {
        base.SetUpToVisual(b);
        if (b) splineItems.OnItemMoved+=UpdateVisual; else splineItems.OnItemMoved-=UpdateVisual;
    }
    public override bool AddToSpline(SplineItem item)
    {
        var b= splineItems.Add(item);
        _isWork=b;
        if(!_isWork)
        {
            StartCoroutine(ProccessingCoroutine());
            canMove=true;
            _isWork=true;
        }
        return b;
    }
    public override void Proccessing()
    {
        canMove=splineItems.Move();
    }
    public override IEnumerator ProccessingCoroutine()
    {
        while(canMove)
        {
            Proccessing();
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator MoveObjectsAlongSpline()
    {
        while (canMove) 
        {
            foreach (var item in splineItems.Values)
            {
                int index = splineItems.IndexOf(item);
                float t = index / (float)countOfSegments;
                Vector3 targetPoint = spline.EvaluatePosition(t);

                item.transform.position = Vector3.Lerp(item.transform.position, targetPoint, Time.deltaTime * speed);
            }

            yield return null; 
        }
    }
    void InvokeEvent()
    {
        OnItemOnEnd?.Invoke();
    }
    void UpdateVisual()
    {
       StartCoroutine(MoveObjectsAlongSpline());
    }
}