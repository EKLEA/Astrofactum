using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class EditTerrain : MonoBehaviour
{
	public Terrain terrain; // Ссылка на Terrain
	public float radius = 5f; // Радиус воздействия
	private float targetHeight; // Целевая высота
	private bool isModifying = false; // Флаг, что сейчас идет изменение
	public GameWorld gameWorld;
	int resulution=>gameWorld.quality;

	void Update()
	{
		// Проверяем нажатие ЛКМ
		if (Input.GetMouseButtonDown(0))
		{
			isModifying = true;
			SetTargetHeight();
		}

		// Проверяем отпускание ЛКМ
		if (Input.GetMouseButtonUp(0))
		{
			isModifying = false;
		}

		// Если идет изменение, обновляем высоты
		if (isModifying)
		{
			ModifyTerrain();
		}
	}

	// Фиксируем начальную высоту под курсором мыши
	void SetTargetHeight()
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

				targetHeight = heightMap[0, 0]; // Сохраняем начальную высоту
			}
		}
	}

	// Изменение высоты Terrain
	void ModifyTerrain()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			Vector3 point = hit.point;
			float step = 1.0f / resulution; // Предполагаем, что resulution - это количество точек на единицу

			// Используем HashSet для хранения уникальных точек
			HashSet<Vector2> uniquePoints = new HashSet<Vector2>();

			// Генерация точек в радиусе
			for (float x = point.x - radius-2; x <= point.x + radius+2; x += step)
			{
				for (float z = point.z - radius-2; z <= point.z + radius+2; z += step)
				{
					Vector2 pos = new Vector2(x, z);
					if (Vector2.Distance(new Vector2(point.x, point.z), pos) <= radius)
					{
						uniquePoints.Add(pos);
					}
				}
			}
			// Передаем уникальные точки в метод изменения террейна
			gameWorld.EditTerrain(uniquePoints.ToArray(),targetHeight);
		}
	}



	// Преобразование позиции в мировых координатах в относительные координаты Terrain
	Vector3 GetTerrainRelativePosition(Vector3 worldPosition)
	{
		Vector3 terrainPosition = terrain.transform.position;
		TerrainData terrainData = terrain.terrainData;

		float relativeX = (worldPosition.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution;
		float relativeZ = (worldPosition.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution;

		return new Vector3(relativeX, 0, relativeZ);
	}
}

