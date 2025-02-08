using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PhantomObject : MonoBehaviour,IAmBuilding
{
	public Material previewMaterialTrue; 
	public Material previewMaterialFalse; 
	BuildingInfo buildingInfo;
	private GameObject _phantomParentObject;
	public string id {get{return _id;}}

	public GameObject[] inPorts => throw new NotImplementedException();

	public GameObject[] OutPorts => throw new NotImplementedException();
	private string _id;

	public void Init(BuildingInfo info)
	{
		buildingInfo = info;
		_id = buildingInfo.id;
		_phantomParentObject = this.gameObject; 
		SetUpMeshes();
	}


   
	public void ChangeColor(bool canAction)
	{
		var gm =_phantomParentObject;
		if(gm.transform.childCount>0)
		{
			foreach (Transform child in gm.transform)
			{
				var meshFilter = child.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					var meshRenderer=child.GetComponent<MeshRenderer>();	
					meshRenderer.material = canAction?previewMaterialTrue:previewMaterialFalse;
					
				}
			}
		}
		else
		{
			var meshFilter = gm.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				var meshRenderer= gm.GetComponent<MeshRenderer>();	
				meshRenderer.material = canAction?previewMaterialTrue:previewMaterialFalse;
			}
			
		}
	}
	void SetUpMeshes()
	{
		Debug.Log("Вщту");
		var gm =buildingInfo.prefab;
		var s = _phantomParentObject.AddComponent<BoxCollider>();
		s.size=gm.GetComponent<BoxCollider>().size;
		s.center=gm.GetComponent<BoxCollider>().center;
		
		if(gm.transform.childCount>0)
		{
			foreach (Transform child in gm.transform)
			{
				var meshFilter = child.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					var mesh = meshFilter.sharedMesh;
					GameObject phantomObject = new GameObject("PhantomObject");
					
					phantomObject.transform.parent=_phantomParentObject.transform;
					phantomObject.transform.rotation=child.rotation;
					phantomObject.transform.localPosition=Vector3.zero;
					phantomObject.transform.localScale= child.localScale;

					
					MeshFilter newMeshFilter = phantomObject.AddComponent<MeshFilter>();
					MeshRenderer newMeshRenderer = phantomObject.AddComponent<MeshRenderer>();

				
					newMeshFilter.sharedMesh = mesh;
					
				}
			}
		}
		else
		{
			var meshFilter = gm.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				var mesh = meshFilter.sharedMesh;
				GameObject phantomObject = new GameObject("PhantomObject");
				phantomObject.transform.parent=_phantomParentObject.transform;
				
				phantomObject.transform.localPosition=Vector3.zero;
				phantomObject.transform.localScale= gm.transform.localScale;

				
				MeshFilter newMeshFilter = phantomObject.AddComponent<MeshFilter>();
				MeshRenderer newMeshRenderer = phantomObject.AddComponent<MeshRenderer>();

			
				newMeshFilter.sharedMesh = mesh;
			}
			
		}
			
		
	}
	public void UnPhantom()
	{
		var s= Instantiate(buildingInfo.prefab);
		s.transform.position=transform.position;
		s.transform.rotation=transform.rotation;
		s.GetComponent<BuildingLogicBase>().Init(buildingInfo.id);
		DestroyImmediate(this);
	}
}
