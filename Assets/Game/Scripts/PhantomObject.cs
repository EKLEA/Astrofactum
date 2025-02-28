using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PhantomObject : PhantomObjParent
{	
	BuildingInfo buildingInfo;
	GameObject prefab;
	public override void Init(string id)
	{
		base.Init(id);
		gmForMesh.transform.parent=this.transform;
		buildingInfo = InfoDataBase.buildingBase.GetInfo(id);
		prefab =buildingInfo.prefab;
		gameObject.gameObject.layer=LayerMask.NameToLayer("Phantom");
		SetUpPhantom();
		
	}
	public override void ChangeColor(bool canAction)
	{
		
		Material newMaterial = canAction ? previewMaterialTrue : previewMaterialFalse;
		
		foreach (MeshRenderer meshRenderer in gmForMesh.GetComponentsInChildren<MeshRenderer>())
			meshRenderer.material = newMaterial;
	}
	protected override void SetUpPhantom()
	{
		var c = gameObject.AddComponent<BoxCollider>();
		
		c.size=prefab.GetComponent<BoxCollider>().size;
		c.center=prefab.GetComponent<BoxCollider>().center;
		
		
		if(prefab.transform.childCount==0)
			AddPartMesh(prefab.transform);
		else
			for(int i=0;i<prefab.transform.childCount;i++)
				AddPartMesh(prefab.transform.GetChild(i));	
		gmForMesh.transform.localScale=prefab.transform.localScale;
	}
	
	protected override void AddPartMesh(Transform part)
	{
		GameObject newPart = new GameObject(part.name);
		newPart.transform.SetParent(gmForMesh.transform);
		newPart.transform.localPosition = part.localPosition;
		newPart.transform.localRotation = part.localRotation;
		newPart.transform.localScale = part.localScale;

		if (part.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
		{
			var newMeshFilter = newPart.AddComponent<MeshFilter>();
			newMeshFilter.sharedMesh = meshFilter.sharedMesh;
		}

		if (part.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
		{
			var newMeshRenderer = newPart.AddComponent<MeshRenderer>();
			newMeshRenderer.material = previewMaterialTrue; // Используем фантомный материал
		}
	}
	public override void UnPhantom()
	{
		var s= Instantiate(buildingInfo.prefab);
		s.transform.position=transform.position;
		s.transform.rotation=transform.rotation;
		s.GetComponent<BuildingLogicBase>().Init(buildingInfo.id);
		DestroyImmediate(this.gameObject);
	}
}
