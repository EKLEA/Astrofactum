using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
	public ChunkRenderer chunkPrefab;
	public Dictionary<Vector2Int,ChunkData> ChunksDatas = new Dictionary<Vector2Int, ChunkData>();
	
	public TerrainGenerator terrainGenerator;
	public int quality;
	public int loadRadius=10;
	Dictionary<ChunkData,Dictionary<Vector2,float>> chunksChanged=new();
	
	void Start()
	{
		terrainGenerator.Init(quality);
		StartCoroutine(Generate(false));
	}
	IEnumerator Generate(bool wait)
	{
		
		for (int x =-loadRadius; x <=  loadRadius; x++)
		{
			for (int y =  -loadRadius; y <= loadRadius; y++)
			{
				Vector2Int chunkPosition = new Vector2Int(x, y);
				
				if (ChunksDatas.ContainsKey(chunkPosition)) continue;
				
				var xPos= chunkPosition.x*32*terrainGenerator.scale;
				var zPos= chunkPosition.y*32*terrainGenerator.scale;
						
				ChunkData chunkData=new ChunkData();
				chunkData.chunkPosition= chunkPosition;
				
				
				ChunksDatas.Add(chunkPosition, chunkData);
				
				chunkData.terrainData=terrainGenerator.GenerateTerrain(xPos,zPos);
				var chunk = Instantiate(chunkPrefab,new Vector3(xPos,0,zPos),Quaternion.identity,transform);
				chunk.SetUpData(chunkData);
				
				if (wait) yield return null;
			}
		}
		foreach (var chunk in ChunksDatas.Values)
		{
			AssignNeighbors(chunk);
		}
		
	}
	void AssignNeighbors(ChunkData chunkData)
	{
		Vector2Int chunkPosition = chunkData.chunkPosition;

		// Проверяем и назначаем соседей по всем четырем направлениям
		if (ChunksDatas.TryGetValue(chunkPosition + Vector2Int.left, out var leftChunk))
		{
			chunkData.leftChunk = leftChunk;
			leftChunk.rightChunk = chunkData; // Также обновляем ссылку на текущий чанк у соседа
		}

		if (ChunksDatas.TryGetValue(chunkPosition + Vector2Int.right, out var rightChunk))
		{
			chunkData.rightChunk = rightChunk;
			rightChunk.leftChunk = chunkData;
		}

		if (ChunksDatas.TryGetValue(chunkPosition + Vector2Int.down, out var backChunk))
		{
			chunkData.backChunk = backChunk;
			backChunk.forwardChunk = chunkData;
		}

		if (ChunksDatas.TryGetValue(chunkPosition + Vector2Int.up, out var forwardChunk))
		{
			chunkData.forwardChunk = forwardChunk;
			forwardChunk.backChunk = chunkData;
		}
	}
	
	[ContextMenu("Regenerate terrain")]
	public void Regenerate()
	{
		terrainGenerator.Init(quality);
		foreach( var chunckData in ChunksDatas)
		{
			Destroy(chunckData.Value.terrainData);
			
		}
		ChunksDatas.Clear();
		StartCoroutine(Generate(false));
	}
	public void EditTerrain(Vector2[] points,float targetHeihth)
	{
		foreach(var pos in points)
		{
			var chunkPos =GetChunkPos(pos);
			if(ChunksDatas.TryGetValue(chunkPos, out var chunkData))
			{
				var posInChunk=pos-chunkPos*(32*terrainGenerator.scale);
				if(!chunksChanged.ContainsKey(chunkData)) chunksChanged.Add(chunkData,new Dictionary<Vector2,float>());
				if(chunksChanged[chunkData].ContainsKey(posInChunk))continue;
				chunksChanged[chunkData].Add(posInChunk,targetHeihth);
			}		
		}
		foreach(var chunk in chunksChanged.Keys)
		{
			chunk.chunkRenderer.EditHeights(chunksChanged[chunk]);
		}
	}
	
	public Vector2Int GetChunkPos(Vector2 pointPos)
	{
		Vector2Int chunkPosition = Vector2Int.FloorToInt(new Vector2(pointPos.x/(32*terrainGenerator.scale) , pointPos.y/(32*terrainGenerator.scale)));
		
		return  chunkPosition;
	}
	public ChunkData GetChunkData(Vector2Int pos)
	{
		return ChunksDatas[pos];
	}
}
