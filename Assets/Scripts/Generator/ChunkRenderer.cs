using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
	public Material material;
	Terrain terrain => GetComponent<Terrain>();
	TerrainCollider terrCollider => GetComponent<TerrainCollider>();

	public void SetUpData(ChunkData chunkData)
	{
		terrain.terrainData = chunkData.terrainData;
		terrCollider.terrainData = chunkData.terrainData;
		terrain.materialTemplate = material;
		chunkData.chunkRenderer=this;
	}
	public void EditHeights(Dictionary<Vector2,float> newHeights)
	{
		var updateHeights = terrain.terrainData.GetHeights(0,0,terrain.terrainData.heightmapResolution,terrain.terrainData.heightmapResolution);
		foreach(var pos in newHeights.Keys)
		{
			int x = Convert.ToInt32(pos.x*2);
			int y = Convert.ToInt32(pos.y*2);
			updateHeights[y,x]=newHeights[pos];
			
		}
		terrain.terrainData.SetHeights(0,0,updateHeights);
	}

}

