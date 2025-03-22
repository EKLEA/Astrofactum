using UnityEngine;
using UnityEngine.Splines;

public class PhantomParent : MonoBehaviour
{
    protected Material previewMaterialTrue{get{return InfoDataBase.materialBase.GetInfo("True");}} 
    protected Material previewMaterialFalse{get{return InfoDataBase.materialBase.GetInfo("False");}}
    protected BuildingLogicBase logic;
	
	public virtual void UnPhantom(){}
    public virtual void Init(){}
    public virtual void ChangeColor(bool canAction){}
}