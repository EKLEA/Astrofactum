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
		gameObject.name="Phantom";
		(logic as SplineParent)?.resolution.GenerateMeshAlongSpline();
		base.Init();
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
		var p = gameObject.transform.parent;
		this.gameObject.transform.parent=transform.parent.parent;
		if (p.transform.childCount == 0) DestroyImmediate(p.gameObject);
		
		base.UnPhantom();
		
	}
}