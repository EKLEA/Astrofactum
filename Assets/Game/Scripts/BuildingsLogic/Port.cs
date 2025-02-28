using System;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour
{
	public PortDir portDir{get {return _portDir;}}
	public PortType portType{get {return _portType;}}
	public SplineParent spline;
	[SerializeField] PortDir _portDir;
	[SerializeField] PortType _portType;
    public Item GetFromSpline()
    {
        return null;
    }
    public void AddToSpline(Item item)
    {
		spline.AddToSpline(new SplineItem(item));
    }
}
public enum PortType
{
	solid,
	liquid
}
public enum PortDir
{
	In,
	Out,
	UnSelected
}