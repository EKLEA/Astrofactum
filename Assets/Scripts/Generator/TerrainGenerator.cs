using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Terrain/Terrain")]
public class TerrainGenerator : ScriptableObject
{
	public float heightScale = 10f; // Множитель высоты для рельефа

	public float baseHeight = 4f; // Базовая высота (смещение)
	public int scale = 1; // Базовая высота (смещение)
	 // Множитель разрешения (качество)

	

	public NoiseOctaveSettings[] Octaves;
	public NoiseOctaveSettings DomainWarp;
	public NoiseOctaveSettings ContinentNoise;
	public NoiseOctaveSettings ErosionNoise;

	private FastNoiseLite[] octaveNoises;
	private FastNoiseLite warpNoise;
	private FastNoiseLite continentNoise;
	private FastNoiseLite erosionNoise;
	private TerrainData terrainData;
	private int _quality;
	public int resolution;


	[Serializable]
	public class NoiseOctaveSettings
	{
		public FastNoiseLite.NoiseType noiseType;
		public float frequency = 0.2f;
		public float amplitude = 2f;
		public AnimationCurve splineCurve = AnimationCurve.Linear(-1, 0, 1, 1); // ВЕЛИКИЙ СПЛАЙН, ОПРЕДЕЛЯЕТ СУДЬБУ МИРА
		
	}


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

		continentNoise = new FastNoiseLite();
		continentNoise.SetNoiseType(ContinentNoise.noiseType);
		continentNoise.SetFrequency(ContinentNoise.frequency);

		erosionNoise = new FastNoiseLite();
		erosionNoise.SetNoiseType(ErosionNoise.noiseType);
		erosionNoise.SetFrequency(ErosionNoise.frequency);
	}

	public TerrainData GenerateTerrain(int offsetX, int offsetZ)
	{
		terrainData = new TerrainData();
		resolution = _quality * 32+1;

		// Задаем разрешение карты высот
		terrainData.heightmapResolution = resolution;

		// Задаем физические размеры террейна (по осям X, Y, Z)
		terrainData.size = new Vector3(32, 96, 32);


		// Создаем массив высот для карты высот
		float[,] heights = new float[resolution, resolution];

		// Генерация высот для каждой точки
		for (int x = 0; x < resolution; x++)
		{
			for (int z = 0; z < resolution; z++)
			{
				// Генерация координат в мировых единицах
				float worldX = x * ((32f*scale) / (resolution - 1)) + offsetX;
				float worldZ = z * ((32f*scale) / (resolution - 1)) + offsetZ;

				// Генерация высоты для каждой точки
				heights[z, x] = GetHeight(worldX, worldZ);
			}
		}

		// Применение высот к terrainData
		terrainData.SetHeights(0, 0, heights);

		return terrainData;
	}


	 float GetHeight(float worldX, float worldZ)
	{
		warpNoise.DomainWarp(ref worldX, ref worldZ);

		float result = baseHeight;
		for (int i = 0; i < Octaves.Length; i++)
		{
			float noise = octaveNoises[i].GetNoise(worldX, worldZ);
			result += noise * Octaves[i].amplitude;
		}

		float continentalNoiseValue = continentNoise.GetNoise(worldX, worldZ);
		float erosionNoiseValue = erosionNoise.GetNoise(worldX, worldZ);

		result *= ContinentNoise.splineCurve.Evaluate(continentalNoiseValue);
		result *= ErosionNoise.splineCurve.Evaluate(erosionNoiseValue);

		return Mathf.Clamp(result * heightScale / 64, 0f, 1f); // Ограничиваем значение от 0 до 1
	}
}
