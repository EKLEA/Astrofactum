using UnityEngine;

[ExecuteInEditMode]
public class PreviewMap : MonoBehaviour
{
    public BiomeNoise biomeNoise;
    public int textureResolution = 512;
    public Renderer targetRenderer;

    public bool autoUpdate = true; // Автоматическое обновление (переключатель)

    private Texture2D _biomeMapTexture;
    private bool _needsUpdate = true; // Флаг обновления

    void Update()
    {
        if (autoUpdate && _needsUpdate)
        {
            GenerateAndDisplayMap();
        }
    }

    public void MarkForUpdate()
    {
        _needsUpdate = true;
        if (autoUpdate)
        {
            GenerateAndDisplayMap();
        }
    }

    // Метод для ручного обновления карты
    [ContextMenu("Update Map")]
    public void ForceUpdateMap()
    {
        GenerateAndDisplayMap();
    }

    public void GenerateAndDisplayMap()
    {
        if (biomeNoise == null)
        {
            Debug.LogError("BiomeNoise is not assigned!");
            return;
        }

        if (targetRenderer == null)
        {
            Debug.LogError("TargetRenderer is not assigned!");
            return;
        }

        if (_biomeMapTexture == null || _biomeMapTexture.width != textureResolution)
        {
            _biomeMapTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGB24, false);
        }

        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                float worldX = (float)x / textureResolution * 10f; // Масштаб мира (настраивается)
                float worldY = (float)y / textureResolution * 10f;

                Biome biome = biomeNoise.GetBiomeAt(worldX, worldY);
                Color pixelColor = biome != null ? biome.color : Color.black; // Проверка на null
                _biomeMapTexture.SetPixel(x, y, pixelColor);
            }
        }

        _biomeMapTexture.Apply();
        targetRenderer.sharedMaterial.mainTexture = _biomeMapTexture;

        _needsUpdate = false; // Сбрасываем флаг обновления
    }
}
