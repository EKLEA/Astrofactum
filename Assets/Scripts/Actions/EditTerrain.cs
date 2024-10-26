using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class EditTerrain : ActionWithWorld
{
	public Terrain terrain;
	public float radius = 5f; 
	private float targetHeight; 
	private bool isModifying = false; 
	public GameWorld gameWorld;
	List<Vector3> points=new();
	float step;

	// Фиксируем начальную высоту под курсором мыши
	void AddPoint()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Проверяем, попала ли мышь в Terrain
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject.GetComponent<Terrain>()!=null)
			{
				terrain=hit.collider.gameObject.GetComponent<Terrain>();
				TerrainData terrainData = terrain.terrainData;

				// Получаем координаты по x и z на Terrain
				Vector3 terrainPos = GetTerrainRelativePosition(hit.point);

				// Получаем текущую высоту в этой точке
				float[,] heightMap = terrainData.GetHeights(Mathf.FloorToInt(terrainPos.x), Mathf.FloorToInt(terrainPos.z), 1, 1);

				if (isModifying == false)
				{
					targetHeight = heightMap[0, 0];
					gameWorld=terrain.GetComponentInParent<GameWorld>();
					step= 1.0f / gameWorld.quality;
				}
				points.Add(hit.point);
			}
		}
	}

	HashSet<Vector2> uniquePoints = new HashSet<Vector2>();
	
	void ModifyTerrain()
	{
		if(points.Count<=1) return;
		
		for (int i = 0; i < points.Count-1; i++)
			AddPoints(points[i],points[i+1]);
			
			
		gameWorld.EditTerrain(uniquePoints.ToArray(),targetHeight);
		onActionEnded();
	}
	
	void AddPoints(Vector3 point1,Vector3 point2)
	{
		Vector3 direction = (point2 - point1).normalized;
		float t =0;
		Vector3 p=point1;
		while (t<=Vector3.Distance(point1,point2))
		{
			
			
			for (float x = p.x- radius-2; x <=p.x + radius+2; x += step)
			{
				for (float z = p.z - radius-2; z <= p.z + radius+2; z += step)
				{
					Vector2 pos = new Vector2(x, z);
					if (Vector2.Distance(new Vector2(p.x, p.z), pos) <= radius)
					{
						uniquePoints.Add(pos);
					}
				}
			}	
			p+= direction * radius;	
			t+=radius;
		}
		
	   
	}
	


	
	Vector3 GetTerrainRelativePosition(Vector3 worldPosition)
	{
		Vector3 terrainPosition = terrain.transform.position;
		TerrainData terrainData = terrain.terrainData;

		float relativeX = (worldPosition.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution;
		float relativeZ = (worldPosition.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution;

		return new Vector3(relativeX, 0, relativeZ);
	}

	public override void LeftClick()
	{
		AddPoint();
		if(!Input.GetButton("hold")&&points.Count>=2)
		{
			ModifyTerrain();
		}
	}

	public override void RightClick()
	{
		ModifyTerrain();
	}
}

