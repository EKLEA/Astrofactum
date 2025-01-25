using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System;
using System.Drawing;

public class TerrainGeneretion : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Transform viewer;
	public Material mapMaterial;
	public static TerrainGeneretion Instance;
	Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	float meshWorldSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}
	void Start() {
		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		UpdateVisibleChunks ();
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh ();
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
	}
		
	void UpdateVisibleChunks() {
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
		for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) {
			alreadyUpdatedChunkCoords.Add (visibleTerrainChunks [i].coord);
			visibleTerrainChunks [i].UpdateTerrainChunk ();
		}
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains (viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
						terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					} else {
						TerrainChunk newChunk = new TerrainChunk (viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
						terrainChunkDictionary.Add (viewedChunkCoord, newChunk);
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load();

					}
				}
			}
		}
	}

	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible){
		if(isVisible){
			visibleTerrainChunks.Add(chunk);
		} else {
			visibleTerrainChunks.Remove(chunk);
		}
	}
	public float GetHeight(Vector2 position)
	{
		Vector2Int chunkOrigin=GetChunkPosOrigin(position);
		return terrainChunkDictionary[chunkOrigin].GetHeightInPos(GetPosInChunk(position,chunkOrigin));
	}
	public void EditTerrain(Vector2[] points, float targetHeight)
	{
		// Используем HashSet для уникальности чанков
		HashSet<Vector2Int> updatedChunks = new HashSet<Vector2Int>();

		// Проходим по всем точкам
		foreach (var point in points)
		{
			Vector2Int chunkOrigin = GetChunkPosOrigin(point);  // Находим чанк
			Vector2Int localPos = GetPosInChunk(point, chunkOrigin);  // Находим локальную позицию в чанке

			// Проверяем, существует ли чанк в словаре
			if (terrainChunkDictionary.ContainsKey(chunkOrigin))
			{
				// Устанавливаем новую высоту и помечаем чанк как обновленный
				terrainChunkDictionary[chunkOrigin].SetHeightInPos(localPos, targetHeight);
				updatedChunks.Add(chunkOrigin);  // Добавляем чанк в список обновленных
			}
			else
			{
				Debug.LogWarning($"Chunk at {chunkOrigin} not found in the dictionary.");
			}
		}

		// Обновляем только затронутые чанки
		foreach (var chunkOrigin in updatedChunks)
		{
			// Обновляем каждый затронутый чанк
			terrainChunkDictionary[chunkOrigin].UpdateTerrainChunk();
		}
	}


	Vector2Int GetPosInChunk(Vector2 position,Vector2Int chunkOrigin)
	{
		Vector2 chunkPosInWorld =new Vector2(chunkOrigin.x*meshWorldSize,chunkOrigin.y*meshWorldSize);
		int x =Mathf.FloorToInt(position.x-(chunkPosInWorld.x-(int)meshWorldSize/2));
		int y =Mathf.FloorToInt(position.y-(chunkPosInWorld.y-(int)meshWorldSize/2));
		return new Vector2Int(x,y);
	}
	Vector2Int GetChunkPosOrigin(Vector2 position)
	{
		int x = Mathf.RoundToInt(position.x/meshWorldSize);
		int y = Mathf.RoundToInt(position.y/meshWorldSize);
		return new Vector2Int(x,y);
	}
}

[System.Serializable]
public struct LODInfo
{
	[Range(0, MeshSettings.numSupportedLODs - 1)]
	public int lod;
	public float visibleDstThreshold;


	public float sqrVisibleDstThreshold
	{
		get
		{
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}