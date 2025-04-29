using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineItem : MonoBehaviour
{
    public float value;
    public float target;
    public Slot slot;
    Spline spline;
    
    public void SetUpSplineItem(Slot slot,Spline spline)
    {
        value=0;
        this.slot = slot;
        this.spline = spline;
       Instantiate(InfoDataBase.itemInfoBase.GetInfo(slot.Id).prefab,transform);
    }
    public void UpdateTarget(float _target)
    {
        target=_target;
    }
    public void UpdateValue(float _value)
    {
        if(value+_value>=target)
            value=target;
        else
        {
            value+=_value;
        }
    }
    public void ChangePos()
    {
        spline.Evaluate(value, out float3 position, out float3 tangent, out float3 upVector);
        
        transform.rotation = Quaternion.LookRotation(tangent, upVector);
        Vector3 pos = position;
        transform.localPosition=new Vector3(pos.x, transform.localPosition.y, pos.z);
    }
} 