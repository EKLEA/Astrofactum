using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
        { new SplineInstantiate.InstantiableItem { Prefab = InfoDataBase.buildingBase.GetInfo(_id).prefab,Probability =100 }};
        
        SetUpToVisual(true );
    }
    public virtual void CreateSpline(Port p1, Port p2,SplineType type)
    {
        spline = new Spline();
        Vector3 localPos1 = splineContainer.transform.InverseTransformPoint(p1.transform.position);
        Vector3 localPos2 = splineContainer.transform.InverseTransformPoint(p2.transform.position);
        Vector3 tIn,tOut;
        switch(type)
        {
            case SplineType.StraightAngle:
                tIn= localPos1.x<localPos2.x? new Vector3(-(Math.Abs(localPos1.x)+Math.Abs(localPos2.x)),0,0):
                new Vector3((Math.Abs(localPos1.x)+Math.Abs(localPos2.x)),0,0);
                
                tOut= localPos1.z<localPos2.z? new Vector3(0,0,Math.Abs(localPos1.z)+Math.Abs(localPos2.z)):
                new Vector3(0,0,-(Math.Abs(localPos1.z)+Math.Abs(localPos2.z)));
                break;
            default:
                tIn= localPos1.x<localPos2.x? new Vector3(-(Math.Abs(localPos1.x)+Math.Abs(localPos2.x))/2,0,0):
                new Vector3((Math.Abs(localPos1.x)+Math.Abs(localPos2.x))/2,0,0);
                
                tOut= localPos1.z<localPos2.z? new Vector3(0,0,(Math.Abs(localPos1.z)+Math.Abs(localPos2.z))/2):
                new Vector3(0,0,-(Math.Abs(localPos1.z)+Math.Abs(localPos2.z))/2);
                break;

        }
        spline.Add(new BezierKnot(localPos1,Vector3.zero,tOut));
        spline.Add(new BezierKnot(localPos2,tIn,Vector3.zero));
        splineContainer.AddSpline(spline);
        splineInstantiate.enabled=false;
        splineInstantiate.enabled=true;
    }
    public virtual void ChangeLastKnot(Vector3 pos,Quaternion rot)
    {
        int lastIndex = spline.Count - 1;
        BezierKnot knot = spline[lastIndex];
        knot.Position = pos;
        knot.Rotation=rot;
        spline.SetKnot(lastIndex, knot);
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