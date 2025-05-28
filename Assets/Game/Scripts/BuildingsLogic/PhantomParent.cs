using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class PhantomParent : MonoBehaviour
{
    protected Material previewMaterialTrue{get{return InfoDataBase.materialBase.GetInfo("True");}} 
    protected Material previewMaterialFalse{get{return InfoDataBase.materialBase.GetInfo("False");}}
    protected IAmTickable logic;
	
	public virtual void UnPhantom()
	{
	    foreach (var s in gameObject.transform.GetComponentsInChildren<Transform>(includeInactive:true))
		{
		    s.gameObject.layer=LayerMask.NameToLayer("Building");
		}
		logic?.SetUpLogic();
		DestroyImmediate(this);
	}
    public virtual void Init()
    {	
		logic=GetComponent<IAmTickable>();
        foreach (var s in gameObject.transform.GetComponentsInChildren<Transform>(includeInactive:true))
		{
		    s.gameObject.layer=LayerMask.NameToLayer("Phantom");
		}
    }
    public virtual void ChangeColor(bool canAction){}
}	