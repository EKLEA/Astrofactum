using System;
using System.Collections;
using SplineMeshTools.Colliders;
using SplineMeshTools.Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
[RequireComponent (typeof (SplineMeshResolution), typeof(SplineBoxColliderGenerator))]
public class SplineParent : Building,IHavePorts
{
   
    protected Spline spline;
    [SerializeField] SplineInstantiate splineInstantiate;
    [SerializeField] SplineContainer splineContainer;
    public SplineMeshResolution resolution;
    [SerializeField] SplineBoxColliderGenerator splineBoxColliderGenerator;
    
    [SerializeField] protected Port[] _inPortsGM;
    public Port[] OutPorts => LogicOutPort;

    public Port[] InPorts => LogicInPort;

    [SerializeField] protected Port[]_outPortsGM;
    protected Port[] LogicInPort;
    protected Port[] LogicOutPort;

    public override void Init(string tid)
    {
        base.Init(tid);
        spline=splineContainer[0];
        SetUpToVisual(true );
        splineBoxColliderGenerator.GenerateAndAssignMesh();
        LogicInPort=_inPortsGM;
        LogicOutPort=_outPortsGM;
        
    }
    public void SetFirstPointSpline((Port port, Vector3 pos, quaternion rot) args)
    {
        Vector3 localPos;
        Quaternion rot=Quaternion.Euler(Vector3.zero);
        if (args.port != null)
        {
            _inPortsGM[0].gameObject.SetActive(false);
            LogicInPort=new Port[]{args.port};
            localPos = splineContainer.transform.InverseTransformPoint(args.port.point.position);
        }
        else
        {
            localPos = splineContainer.transform.InverseTransformPoint(args.pos);
            rot = args.rot;
            LogicInPort=_inPortsGM;
            _inPortsGM[0].transform.localPosition = new Vector3(spline.EvaluatePosition(0f).x, 
                                                    _inPortsGM[0].transform.localPosition.y, 
                                                    spline.EvaluatePosition(0f).z);
        }
        
        UpdateInPort(LogicInPort[0]);
        spline[0] = new BezierKnot(localPos,Vector3.zero,Vector3.zero,rot);
       
    }
   public void SetSecondPointSpline((Port port, Vector3 pos, Quaternion rot) args)
    {
        Vector3 localPos;
        Quaternion rot=Quaternion.Euler(Vector3.zero);
        if (args.port != null)
        {
            _outPortsGM[0].gameObject.SetActive(false);
            LogicOutPort=new Port[]{args.port};
            UpdateOutPort(args.port);
            localPos = splineContainer.transform.InverseTransformPoint(args.port.point.position);
        }
        else
        {
            localPos = splineContainer.transform.InverseTransformPoint(args.pos);
            rot = args.rot;
            LogicInPort=_outPortsGM;
        }
        
        UpdateOutPort(LogicOutPort[0]);
        spline[1] = new BezierKnot(localPos,Vector3.zero,Vector3.zero,rot);
        
    }
    public void UpdateTangents(SplineType type)
    {
        Vector3 localPos1 = spline[0].Position;
        Vector3 localPos2 = spline[1].Position;
        Vector3 tIn, tOut;
        float distX = localPos1.x-localPos2.x;
        float distZ = localPos1.z-localPos2.z;

        switch (type)
        {
            case SplineType.StraightAngle:
                tOut= localPos1.z>localPos2.z?new Vector3(0,0,distZ):new Vector3(0,0,-distZ);
                
                tIn= localPos1.x>localPos2.x?new Vector3(0,0,-distX):new Vector3(0,0,distX);
                break;

            case SplineType.Smooth:
                tIn = Vector3.zero;
                tOut = Vector3.zero;
                break;

            case SplineType.Directly:
                tIn = Vector3.zero;
                tOut = Vector3.zero;
                break;

            default:
                 tIn = Vector3.zero;
                tOut = Vector3.zero;
                /*tIn= localPos1.x>localPos2.x? new Vector3(-(Math.Abs(localPos1.x)+Math.Abs(localPos2.x))/2,0,0):
                new Vector3((Math.Abs(localPos1.x)+Math.Abs(localPos2.x))/2,0,0);
                tOut= localPos1.z<localPos2.z? new Vector3(0,0,(Math.Abs(localPos1.z)+Math.Abs(localPos2.z))/2):
                new Vector3(0,0,-(Math.Abs(localPos1.z)+Math.Abs(localPos2.z))/2);*/
                break;
        }
       
        BezierKnot knot0 = spline[0];

        BezierKnot knot1 = spline[1];
        
        spline[0] = new BezierKnot(localPos1,Vector3.zero,tOut,knot0.Rotation);
        spline[1] = new BezierKnot(localPos2,tIn,Vector3.zero,knot1.Rotation);
        
    }

    public virtual void DrawSpline(SplineType type, SplineState state)
    {
        UpdateTangents(type);

        _outPortsGM[0].transform.localPosition = new Vector3(spline.EvaluatePosition(1f).x, _outPortsGM[0].transform.localPosition.y, spline.EvaluatePosition(1f).z);
        
        resolution.meshResolution[0] = state.GetResolution(spline);
        _inPortsGM[0].transform.rotation=spline[0].Rotation;
        
        _inPortsGM[0].transform.rotation = splineContainer.transform.rotation * spline[0].Rotation;
    
    // Поворачиваем выходной порт согласно rotation последней точки Безье
        _outPortsGM[0].transform.rotation = splineContainer.transform.rotation * spline[spline.Count - 1].Rotation;
        
        resolution.GenerateMeshAlongSpline();
        splineBoxColliderGenerator.GenerateAndAssignMesh();
    }
    public void Reset()
    {
        _inPortsGM[0].gameObject.SetActive(true);
        _outPortsGM[0].gameObject.SetActive(true);
        spline[0] = new BezierKnot(Vector3.zero,Vector3.zero,Vector3.zero);
        spline[1] = new BezierKnot(Vector3.forward,Vector3.zero,Vector3.zero);
        
        resolution.GenerateMeshAlongSpline();
        splineBoxColliderGenerator.GenerateAndAssignMesh();
    }
    public virtual void SetUpToVisual(bool b)
    {
        splineInstantiate.enabled = b;
    }
    public virtual void UpdateOutPort(Port port)
    {
       //
    }
    public virtual void UpdateInPort(Port port)
    {
       //
    }
}
public static class SplineExtensons
{
    public static int GetResolution(this SplineState type,Spline spline)
    {
        return type switch
        {
            SplineState.Active=>2,
            SplineState.Passive=>(int)(spline.GetLength() / 5),
            _=>2
        };
    }
}
public enum SplineType
{
    StraightAngle,
    Directly,
    Smooth,
}
public enum SplineState
{
    Active,
    Passive
}