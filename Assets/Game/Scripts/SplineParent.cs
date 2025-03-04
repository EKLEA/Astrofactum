using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent ( typeof (SplineInstantiate),typeof (SplineContainer))]
public class SplineParent : BuildingLogicBase, IAmBuilding
{
    public float speed;
    protected string _id;
    protected Spline spline;
    protected bool _isWork;    
    protected SplineInstantiate splineInstantiate=>GetComponent<SplineInstantiate>();
    protected SplineContainer splineContainer=>GetComponent<SplineContainer>();
    
    public override void Init(string tid)
    {
        _id=tid;
        splineInstantiate.itemsToInstantiate = new SplineInstantiate.InstantiableItem[] 
        { new SplineInstantiate.InstantiableItem { Prefab = InfoDataBase.buildingBase.GetInfo(_id).prefab }};
        
        SetUpToVisual(true );
    }
    public virtual void CreateSpline(Port p1, Port p2)
    {
        spline = new Spline();
        p1.spline=this;
        p2.spline=this;
        spline.Add(new BezierKnot(p1.transform.position));
        spline.Add(new BezierKnot(p2.transform.position));
        splineContainer.AddSpline(spline);
    }
    public virtual void SetUpToVisual(bool b)
    {
        splineInstantiate.enabled = b;
    }
    public virtual bool AddToSpline(SplineItem item){return true;}
    public override void Proccessing()
    {
    }
    public override IEnumerator ProccessingCoroutine()
    {
        while(true)
        {
            Proccessing();
        }
    }
    
}
public enum SplineType
{
    StraightAngle,
    Directly,
}