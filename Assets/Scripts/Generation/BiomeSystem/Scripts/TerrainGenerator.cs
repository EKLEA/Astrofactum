using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Глобальные настройки высоты")]
    public float baseHeight = 0f;
    
    [Header("Шумы для высоты")]
    public NoiseProperties depthNoise;   // Низкочастотный шум для формирования крупных особенностей
    public NoiseProperties surfaceNoise; // Высокочастотный шум для мелких деталей

    [Header("Система биомов")]
    public BiomeNoise biomeNoise;

    [Tooltip("Множитель для глубинного шума")]
    public float depthMultiplier = 10f;
    [Tooltip("Множитель для поверхностного шума")]
    public float surfaceMultiplier = 2f;

    [Header("Глобальные модификаторы")]
    [Tooltip("Глобальная кривая для модуляции шумовых значений (можно использовать для усиления/ослабления эффектов)")]
    public AnimationCurve globalHeightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    /// <summary>
    /// Вычисляет итоговую высоту для заданных мировых координат с учётом биомных параметров, шумов и глобальной кривой.
    /// </summary>
    public float GetTerrainHeight(float x, float y)
    {
        // Определяем биом для данной точки
        Biome biome = biomeNoise.GetBiomeAt(x, y);
        if (biome == null)
        {
            // Если биом не найден, используем значения по умолчанию
            biome = new Biome() { depth = 0f, scale = 1f };
        }

        // Получаем значения шумов для глубины и поверхности
        float rawDepth = depthNoise.GetValue(x, y);
        float rawSurface = surfaceNoise.GetValue(x, y);

        // Применяем глобальную кривую для модуляции шумовых значений
        float modDepth = globalHeightCurve.Evaluate(rawDepth);
        float modSurface = globalHeightCurve.Evaluate(rawSurface);

        // Итоговый расчёт высоты:
        // baseHeight - базовый уровень (например, уровень моря)
        // biome.depth - смещение, характерное для данного биома
        // modDepth и modSurface - модулированные значения шумов
        // depthMultiplier и surfaceMultiplier - множители для усиления влияния каждого шума
        // biome.scale - масштабный параметр биома, позволяющий увеличить или уменьшить амплитуду шума
        float height = baseHeight 
                     + biome.depth 
                     + (modDepth * depthMultiplier * biome.scale) 
                     + (modSurface * surfaceMultiplier * biome.scale);
        return height;
    }
}
