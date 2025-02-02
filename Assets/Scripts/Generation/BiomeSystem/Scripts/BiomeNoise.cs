using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    public Color color;
    
    public Vector2 temperatureRange;
    public Vector2 precipitationRange;
    public Vector2 elevationRange;
    
    // Параметры для высотного рельефа:
    [Tooltip("Средняя высота биома (сдвиг по вертикали)")]
    public float depth = 0f;
    [Tooltip("Величина колебаний высоты (масштаб деталей рельефа)")]
    public float scale = 1f;
}


[ExecuteInEditMode]
public class BiomeNoise : MonoBehaviour
{
    [Header("Global World Settings")]
    [Tooltip("Глобальный множитель масштаба мира, применяется к scale каждого шума.")]
    public float worldScale = 1f;
    [Tooltip("Глобальное смещение мира, прибавляется к offset каждого шума.")]
    public Vector2 worldOffset = Vector2.zero;
    [Tooltip("Глобальный сид для генерации случайного глобального смещения.")]
    public int seed = 42;

    [Header("Noise Settings")]
    public NoiseProperties temperatureNoise;
    public NoiseProperties precipitationNoise;
    public NoiseProperties elevationNoise;

    [Header("Biome Settings")]
    public List<Biome> biomes = new List<Biome>();

    // Сгенерированное глобальное смещение на основе сид
    private Vector2 globalSeedOffset;

    void Awake()
    {
        globalSeedOffset = GenerateSeedOffset(seed);
    }
    // Генерирует смещение на основе сид
    private Vector2 GenerateSeedOffset(int seed)
    {
        System.Random random = new System.Random(seed);
        float offsetX = (float)random.NextDouble() * 10000f;
        float offsetY = (float)random.NextDouble() * 10000f;
        return new Vector2(offsetX, offsetY);
    }

    // Вычисляет эффективное значение шума с учётом глобальных настроек
    private float GetEffectiveNoiseValue(NoiseProperties noise, float x, float y, float effectiveScale, Vector2 effectiveOffset)
    {
        // Сохраняем оригинальные настройки, чтобы не изменять ScriptableObject
        float originalScale = noise.scale;
        Vector2 originalOffset = noise.offset;

        noise.scale = effectiveScale;
        noise.offset = effectiveOffset;
        float value = noise.GetValue(x, y);
        
        // Восстанавливаем настройки
        noise.scale = originalScale;
        noise.offset = originalOffset;
        return value;
    }

    // Метод, который возвращает биом для заданных координат
    public Biome GetBiomeAt(float x, float y)
    {
        // Вычисляем эффективный масштаб и смещение для каждого типа шума.
        // К локальному offset добавляем глобальное смещение мира (worldOffset)
        // и глобальное смещение, сгенерированное на основе сида (globalSeedOffset).
        float effectiveTempScale = temperatureNoise.scale * worldScale;
        Vector2 effectiveTempOffset = temperatureNoise.offset + worldOffset;// + globalSeedOffset;
        
        float effectivePrecScale = precipitationNoise.scale * worldScale;
        Vector2 effectivePrecOffset = precipitationNoise.offset + worldOffset;// + globalSeedOffset;
        
        float effectiveElevScale = elevationNoise.scale * worldScale;
        Vector2 effectiveElevOffset = elevationNoise.offset + worldOffset;// + globalSeedOffset;

        float temperature = GetEffectiveNoiseValue(temperatureNoise, x, y, effectiveTempScale, effectiveTempOffset);
        float precipitation = GetEffectiveNoiseValue(precipitationNoise, x, y, effectivePrecScale, effectivePrecOffset);
        float elevation = GetEffectiveNoiseValue(elevationNoise, x, y, effectiveElevScale, effectiveElevOffset);

        foreach (var biome in biomes)
        {
            if (temperature >= biome.temperatureRange.x && temperature <= biome.temperatureRange.y &&
                precipitation >= biome.precipitationRange.x && precipitation <= biome.precipitationRange.y &&
                elevation >= biome.elevationRange.x && elevation <= biome.elevationRange.y)
            {
                return biome;
            }
        }

        return null; // Если не найден подходящий биом
    }

    // Вызывается, когда значения изменяются в инспекторе или скрипт загружается
    void OnValidate()
    {     
        FindObjectOfType<PreviewMap>()?.MarkForUpdate();
    }
}
