using UnityEngine;

[CreateAssetMenu(menuName = "Biome System/Noise Properties")]
public class NoiseProperties : ScriptableObject
{
    [Header("Основные параметры шума")]
    public float scale = 50f;         // Масштаб шума
    public float amplitude = 1f;      // Итоговая амплитуда шума (можно использовать для усиления/ослабления итогового значения)
    public Vector2 offset;            // Смещение шума
    public float persistence = 0.5f;  // Влияние октав (затухание амплитуды)
    public float lacunarity = 2f;     // Увеличение частоты на каждой октаве
    public int octaves = 3;           // Количество октав

    [Header("Нормализация")]
    public bool normalize = true; // Если true – использовать теоретическую нормализацию

    public float GetValue(float x, float y)
    {
        float totalValue = 0f;
        float frequency = 1f;
        float amplitudeMultiplier = 1f;

        // Вычисляем максимальный возможный вклад для данных настроек
        float maxPossibleHeight = 0f;
        float amplitudeAccumulator = 1f;
        for (int i = 0; i < octaves; i++)
        {
            maxPossibleHeight += amplitudeAccumulator;
            amplitudeAccumulator *= persistence;
        }

        // Суммируем октавы
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offset.x) / scale * frequency;
            float sampleY = (y + offset.y) / scale * frequency;
            // Mathf.PerlinNoise возвращает значение от 0 до 1, умножая на 2 и вычитая 1 получаем диапазон [-1, 1]
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
            totalValue += perlinValue * amplitudeMultiplier;

            frequency *= lacunarity;
            amplitudeMultiplier *= persistence;
        }

        float normalizedValue = totalValue;
        if (normalize)
        {
            // Приводим totalValue из диапазона [-maxPossibleHeight, +maxPossibleHeight] к [0, 1]
            normalizedValue = (totalValue + maxPossibleHeight) / (2f * maxPossibleHeight);
        }
        return Mathf.Clamp01(normalizedValue * amplitude);
    }
}
