using SplineMeshTools.Core;
using UnityEngine;
using UnityEngine.Splines;

public class PhantomSpline : PhantomParent
{
    MeshRenderer meshRenderer=>GetComponent<MeshRenderer>();
    Material[] originMat;
    Material[] newMat;
    string namee;
    public override void Init()
	{
		namee=gameObject.name;
		originMat=meshRenderer.materials;
		newMat=new Material[originMat.Length];
		ChangeColor(true);
		logic = GetComponent<BuildingLogicBase>();
		gameObject.name="Phantom";
		(logic as SplineParent).resolution.GenerateMeshAlongSpline();
		//logic.enabled = false;
	}
	public override void ChangeColor(bool canAction)
	{
		Material newMaterial = canAction ? previewMaterialTrue : previewMaterialFalse;
		
		for(int i=0;i<meshRenderer.materials.Length;i++)
			newMat[i]=newMaterial;
		meshRenderer.materials=newMat;
	}
	public override void UnPhantom()
	{
		gameObject.name=namee;
		meshRenderer.materials=originMat;
		gameObject.transform.parent=transform.parent.parent;
		DestroyImmediate(this);
	}
}