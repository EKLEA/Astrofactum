using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, BiomeNoise biomeNoise){
        float[,] noiseValues = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);
        
        // Для потокобезопасности копируем кривую
        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        // Проходим по всем точкам карты
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Базовое значение высоты из шума
                float baseHeightValue = noiseValues[x, y];
                
                // Определяем мировые координаты текущей точки
                // Если sampleCentre задаёт начало выборки, то можно взять:
                Vector2 worldPos = sampleCentre + new Vector2(x, y);
                
                // Получаем биом для данной точки
                Biome biome = biomeNoise.GetBiomeAt(worldPos.x, worldPos.y);
                
                // Если биом найден, модифицируем высоту согласно его параметрам.
                // Если биом не найден, используем значения по умолчанию (0 смещения, scale = 1)
                float biomeDepth = 0f;
                float biomeScale = 1f;
                if (biome != null)
                {
                    biomeDepth = biome.depth;
                    biomeScale = biome.scale;
                }
                
                // Применяем глобальную кривую для модуляции базового значения
                float modHeight = heightCurve_threadsafe.Evaluate(baseHeightValue) * settings.heightMultiplier;
                
                // Итоговая высота = базовая высота модифицированная кривой, плюс смещение биома, умноженное на его масштаб
                float finalHeight = modHeight + biomeDepth;
                finalHeight *= biomeScale;
                
                noiseValues[x, y] = finalHeight;
                if (finalHeight > maxValue) maxValue = finalHeight;
                if (finalHeight < minValue) minValue = finalHeight;
            }
        }
        
        return new HeightMap(noiseValues, minValue, maxValue);
    }
}

public struct HeightMap {
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;
    
    public HeightMap (float[,] values, float minValue, float maxValue){
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}