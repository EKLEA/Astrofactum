using UnityEngine;
using UnityEngine.Splines;

public class PhantomSpline : PhantomParent
{
    SplineInstantiate spInst=>GetComponent<SplineInstantiate>();
    MeshRenderer mesh;
    public override void Init(string id)
	{
		_id = id;
		logic = GetComponent<BuildingLogicBase>();
		logic.enabled = false;
		prefab=spInst.itemsToInstantiate[0].Prefab;
		mesh=spInst.itemsToInstantiate[0].Prefab.GetComponent<MeshRenderer>();
	}
	public override void ChangeColor(bool canAction)
	{
		Material newMaterial = canAction ? previewMaterialTrue : previewMaterialFalse;
		mesh.material=newMaterial;
	}
	public override void UnPhantom()
	{
		logic.enabled=true;
		spInst.itemsToInstantiate[0].Prefab=prefab;
		DestroyImmediate(this);
	}
}