using System;
using System.Collections;
using System.Collections.Generic;
using SplineMeshTools.Colliders;
using SplineMeshTools.Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
[RequireComponent (typeof (SplineMeshResolution), typeof(SplineBoxColliderGenerator))]
public class SplineParent : Building,IHavePorts
{
   
    protected Spline spline;
    [SerializeField] protected SplineInstantiate splineInstantiate;
    [SerializeField] protected SplineContainer splineContainer;
    public SplineMeshResolution resolution;
    [SerializeField] protected SplineBoxColliderGenerator splineBoxColliderGenerator;
    public float maxLenght;
    [SerializeField] protected Port[] _inPortsGM;
    public Port[] OutPorts => new Port[] {LogicOutPort};

    public Port[] InPorts => new Port[] {LogicInPort};

    [SerializeField] protected Port[]_outPortsGM;
    public Port LogicInPort;
    public Port LogicOutPort;
    public override void Init(string tid)
    {
        base.Init(tid);
        spline=splineContainer[0];
        SetUpToVisual(true );
        splineBoxColliderGenerator.GenerateAndAssignMesh();
        
        
        SetUpInPort(null);
        SetUpOutPort(null);
        
    }
    public float GetLenght()
    {
        return spline.GetLength();
    }
    public void SetFirstPointSpline(Vector3 pos, Quaternion rot)
    {   

        Vector3 localPos = splineContainer.transform.InverseTransformPoint(pos);
        Quaternion localRot = Quaternion.Inverse(splineContainer.transform.rotation) * rot;
        
        spline[0] = new BezierKnot(localPos, Vector3.zero, Vector3.zero, localRot);
    }

    public void SetSecondPointSpline(Vector3 pos, Quaternion rot)
    {
    
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(pos);
        Quaternion localRot = Quaternion.Inverse(splineContainer.transform.rotation) * rot;
        
        spline[1] = new BezierKnot(localPos, Vector3.zero, Vector3.zero, localRot);
    }
    public virtual void SetUpInPort(Port inPort)
    {
        if(inPort==null)
        {
            LogicInPort=_inPortsGM[0];
            LogicInPort.gameObject.SetActive(true);
        }
        else
        {
            LogicInPort = inPort;
            
            _inPortsGM[0].gameObject.SetActive(false);
        }
    }
    public virtual void SetUpOutPort(Port outPort)
    {
        if(outPort==null) 
        {
            LogicOutPort=_outPortsGM[0];
            LogicOutPort.gameObject.SetActive(true);
        }
        else 
        {
            LogicOutPort = outPort;
            _outPortsGM[0].gameObject.SetActive(false);
        }
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
        _inPortsGM[0].transform.localPosition = new Vector3(spline.EvaluatePosition(0f).x, _inPortsGM[0].transform.localPosition.y, spline.EvaluatePosition(0f).z);
        
        resolution.meshResolution[0] = state.GetResolution(spline);
        
        _inPortsGM[0].transform.rotation =splineContainer.transform.rotation*spline[0].Rotation;

    // Поворачиваем выходной порт согласно rotation последней точки Безье
        _outPortsGM[0].transform.rotation =splineContainer.transform.rotation*spline[spline.Count - 1].Rotation;
        resolution.GenerateMeshAlongSpline();
        splineBoxColliderGenerator.GenerateAndAssignMesh();
    }
    public void Reset()
    {
        
        
        LogicInPort=_inPortsGM[0];
        LogicOutPort=_outPortsGM[0];
        
        _inPortsGM[0].gameObject.SetActive(true);
        _outPortsGM[0].gameObject.SetActive(true);
        
        spline[0] = new BezierKnot(Vector3.zero,Vector3.zero,Vector3.zero);
        spline[1] = new BezierKnot(Vector3.forward,Vector3.zero,Vector3.zero);
        DrawSpline(SplineType.StraightAngle,SplineState.Active);
    }
    public List<GameObject>GetAllCollisionsAlongSpline(float checkStep = 1.0f, float checkRadius = 0.2f, float verticalOffset = 0.75f)
    {
        List<GameObject> result = new();
        
        if (spline == null || splineContainer == null || spline.GetLength() <= 0)
            return result;

        int steps = Mathf.Max(1, Mathf.CeilToInt(spline.GetLength() / checkStep));
        
        for (int i = 0; i <= steps; i++)
        {
            Vector3 point = splineContainer.EvaluatePosition(i / (float)steps);
            
            Vector3 checkPoint = point + Vector3.up * verticalOffset;
            
            Collider[] colliders = Physics.OverlapSphere(checkPoint, checkRadius);
            
            foreach (var collider in colliders)
            {
                if (collider.transform != this.transform && 
                    !collider.transform.IsChildOf(this.transform) &&
                    !result.Contains(collider.gameObject))
                {
                    result.Add(collider.gameObject);
                }
            }
        }
        
        return result;
    }
    public virtual bool ValidateSpline()
    {
       return false;
    }
    public bool CheckSelfIntersections(float checkStep = 0.05f)
    {
        List<Vector3> points = new List<Vector3>();
        
        for (float t = 0; t <= 1f; t += checkStep)
        {
            points.Add(splineContainer.EvaluatePosition(t));
        }

        for (int i = 0; i < points.Count - 3; i++)
        {
            for (int j = i + 2; j < points.Count - 1; j++)
            {
                if (LineSegmentsIntersect(
                    points[i], points[i+1],
                    points[j], points[j+1]))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool CheckSplineCurvature(float maxAngle,int segments)
    {
        float step = 1f / segments;

        for (int i = 0; i < segments - 1; i++)
        {
            float t1 = i * step;
            float t2 = (i + 1) * step;

            Vector3 dir1 = splineContainer.EvaluateTangent(t1);
            dir1.Normalize(); 
            
            Vector3 dir2 = splineContainer.EvaluateTangent(t2);
            dir2.Normalize();

            float angle = Vector3.Angle(dir1, dir2);
            
            if (angle > maxAngle)
                return false;
        }
        return true;
    }
    protected bool LineSegmentsIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 a = p2 - p1;
        Vector3 b = p3 - p4;
        Vector3 c = p1 - p3;

        float alphaNumerator = Vector3.Dot(b, c);
        float denominator = Vector3.Dot(a, b);

        if (denominator == 0) 
            return false;

        float alpha = alphaNumerator / denominator;
        float beta = Vector3.Dot(a, c) / denominator;

        return (alpha > 0 && alpha < 1) && (beta > 0 && beta < 1);
    }
    public virtual void SetUpToVisual(bool b)
    {
        splineInstantiate.enabled = b;
    }
}
public static class SplineExtensons
{
    public static int GetResolution(this SplineState type,Spline spline)
    {
        return type switch
        {
            SplineState.Active=>2,
            SplineState.Passive=>math.max((int)(spline.GetLength() / 5),1),
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