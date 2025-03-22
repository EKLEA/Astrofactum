using System;
using System.Collections;
using SplineMeshTools.Colliders;
using SplineMeshTools.Core;
using SplineMeshTools;
using UnityEngine;
using UnityEngine.Splines;
using SplineMeshTools.Editor;
using Unity.Mathematics;
using UnityEngine.UIElements;

[RequireComponent (typeof (SplineMeshResolution), typeof(SplineBoxColliderGenerator))]
public class SplineParent : BuildingWithPorts
{
    public float speed;
    [SerializeField] Port inPort;
    [SerializeField] Port outPort;
    protected Spline spline;
    protected SplineInstantiate splineInstantiate=>GetComponent<SplineInstantiate>();
    protected SplineContainer splineContainer=>GetComponent<SplineContainer>();
    public SplineMeshResolution resolution=>GetComponent<SplineMeshResolution>();
    protected SplineBoxColliderGenerator splineBoxColliderGenerator=>GetComponent<SplineBoxColliderGenerator>();
    
    public override void Init(string tid)
    {
        spline=splineContainer[0];
        _id=tid;
        SetUpToVisual(true );
        splineBoxColliderGenerator.GenerateBoxColliderMesh();
    }
    public virtual void FirstPointSpline((Port,Vector3,Quaternion) p1)
    {
        if(p1.Item1!=null)
        {
            inPort.gameObject.SetActive(false);
            p1.Item1.PortUpdateTo(this);
        }
        Vector3 localPos1 = splineContainer.transform.InverseTransformPoint(p1.Item2);
        spline[0]=new BezierKnot(localPos1,Vector3.zero,Vector3.zero,p1.Item3);
        inPort.transform.localPosition=new Vector3(spline.EvaluatePosition(0f).x,inPort.transform.localPosition.y,spline.EvaluatePosition(0f).z);
    }
    public virtual void SecondPointSpline((Port,Vector3,Quaternion) p2,SplineType type)
    {
       Debug.Log(p2.Item3.eulerAngles);
        
        if(p2.Item1!=null)
        {
            outPort.gameObject.SetActive(false);
            p2.Item1.PortUpdateFrom(this);
        }
        
        Vector3 localPos1 =  spline[0].Position;
        Vector3 localPos2 = splineContainer.transform.InverseTransformPoint(p2.Item2);
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
        
        spline[0]=new BezierKnot( spline[0].Position ,Vector3.zero,tOut);
        spline[1]=new BezierKnot(localPos2,tIn,Vector3.zero,p2.Item3);
        outPort.transform.localPosition=new Vector3(spline.EvaluatePosition(0f).x,outPort.transform.localPosition.y,spline.EvaluatePosition(0f).z);
        splineBoxColliderGenerator.resolution=(int)(spline.GetLength()/5);
        splineBoxColliderGenerator.GenerateBoxColliderMesh();
        resolution.GenerateMeshAlongSpline();
    }
    public virtual void SetUpToVisual(bool b)
    {
        splineInstantiate.enabled = b;
    }
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