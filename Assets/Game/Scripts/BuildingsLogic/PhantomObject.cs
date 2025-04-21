using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PhantomObject : PhantomParent
{	
	List<MeshRenderer> meshRenderers=new();
	List<Material[]> Materials=new();
	public override void Init()
	{
		meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>(true));
		
		foreach (MeshRenderer mr in meshRenderers)
		{
			Material[] mats = mr.materials;
			Material[] matsCopy = new Material[mats.Length];

			for (int i = 0; i < mats.Length; i++)
			{
				matsCopy[i] = Instantiate(mats[i]);
				mats[i] = previewMaterialTrue;
			}
			mr.materials = mats;
			Materials.Add(matsCopy);
		}
		base.Init();
	}
	public override void ChangeColor(bool canAction)
	{
		Material newMaterial = canAction ? previewMaterialTrue : previewMaterialFalse;
		foreach (MeshRenderer meshRenderer in meshRenderers)
		{
			Material[] mats = meshRenderer.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i] = newMaterial;
			
			meshRenderer.materials = mats;
		}
	}

	public override void UnPhantom()
	{
		for (int i = 0; i < meshRenderers.Count; i++)
		{
			meshRenderers[i].materials=Materials[i];
		}
		this.gameObject.transform.parent=transform.parent.parent;
		base.UnPhantom();
	}
}
