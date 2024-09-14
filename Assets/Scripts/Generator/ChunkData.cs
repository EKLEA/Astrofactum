using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Vector2Int chunkPosition;
	public TerrainData terrainData;

	// Соседние чанки для хранения ссылок на них
	public ChunkData leftChunk;
	public ChunkData rightChunk;
	public ChunkData backChunk;
	public ChunkData forwardChunk;
	public ChunkRenderer chunkRenderer;
}
