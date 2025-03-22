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
    public override void Init(string tid)
    {
        base.Init(tid);
        splineItems=new LimetedList<SplineItem>((int)spline.GetLength()/50);
        splineItems.OnItemInListOnEnd+=InvokeMethod;
        
    }
    public override void SetUpToVisual(bool b)
    {
        base.SetUpToVisual(b);
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
    void InvokeMethod(SplineItem splineItem)
    {
        InvokeCanRemoved(splineItem.slot);
        InvokeCanAdded(splineItem.slot);
    }
    void UpdateVisual()
    {
       StartCoroutine(MoveObjectsAlongSpline());
    }
}