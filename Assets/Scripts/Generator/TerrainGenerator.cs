using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Terrain/Terrain")]
public class TerrainGenerator : ScriptableObject
{
	public float heightScale = 10f; // Множитель высоты для рельефа
	public float baseHeight = 4f; // Базовая высота (смещение)
	 // Множитель разрешения (качество)

	private TerrainData terrainData;

	public NoiseOctaveSettings[] Octaves;
	public NoiseOctaveSettings DomainWarp;

	[Serializable]
	public class NoiseOctaveSettings
	{
		public FastNoiseLite.NoiseType noiseType;
		public float frequency = 0.2f;
		public float amplitude = 1;
	}

	FastNoiseLite[] octaveNoises;
	FastNoiseLite warpNoise;
	int _quality;
	public void Init(int quality)
	{
		_quality=quality;
		octaveNoises = new FastNoiseLite[Octaves.Length];
		for (int i = 0; i < Octaves.Length; i++)
		{
			octaveNoises[i] = new FastNoiseLite();
			octaveNoises[i].SetNoiseType(Octaves[i].noiseType);
			octaveNoises[i].SetFrequency(Octaves[i].frequency);
		}

		warpNoise = new FastNoiseLite();
		warpNoise.SetNoiseType(DomainWarp.noiseType);
		warpNoise.SetFrequency(DomainWarp.frequency);
		warpNoise.SetDomainWarpAmp(DomainWarp.amplitude);
	}

	public TerrainData GenerateTerrain(int offsetX, int offsetZ)
	{
		terrainData = new TerrainData();
		int resolution = _quality * 32+1;

		// Задаем разрешение карты высот
		terrainData.heightmapResolution = resolution;

		// Задаем физические размеры террейна (по осям X, Y, Z)
		terrainData.size = new Vector3(32, 32, 32);

		// Создаем массив высот для карты высот
		float[,] heights = new float[resolution, resolution];

		// Генерация высот для каждой точки
		for (int x = 0; x < resolution; x++)
		{
			for (int z = 0; z < resolution; z++)
			{
				// Генерация координат в мировых единицах
				float worldX = x * (32f / (resolution - 1)) + offsetX;
				float worldZ = z * (32f / (resolution - 1)) + offsetZ;

				// Генерация высоты для каждой точки
				heights[z, x] = GetHeight(worldX, worldZ);
			}
		}

		// Применение высот к terrainData
		terrainData.SetHeights(0, 0, heights);

		return terrainData;
	}

	float GetHeight(float x, float y)
	{
		//warpNoise.DomainWarp(ref x, ref y);
		float result = baseHeight;
		for (int i = 0; i < Octaves.Length; i++)
		{
			float noise = octaveNoises[i].GetNoise(x, y);
			result += noise * Octaves[i].amplitude / 2;
		}
		return Mathf.Clamp01(result * heightScale / 32);
	}
}
