using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Celestial Body/Earth-Like/Earth Shape")]
public class EarthShape : ScriptableObject
{
    [Header("Continent settings")]
    public float oceanDepthMultiplier = 5;
    public float oceanFloorDepth = 1.5f;
    public float oceanFloorSmoothing = 0.5f;
    public float mountainBlend = 1.2f;

    [Header("Noise settings")]
    public SimpleNoiseSettings continentNoise;
    public SimpleNoiseSettings maskNoise;
    public RidgeNoiseSettings ridgeNoise;

    public Vector4 testParams;

    public float CalculateHeight(Vector3 position, int seed)
    {
        var prng = new PRNG(seed);
        Vector3 continentOffset = continentNoise.GetOffset(prng);
        Vector3 ridgeOffset = ridgeNoise.GetOffset(prng);
        Vector3 maskOffset = maskNoise.GetOffset(prng);

        float continentShape = continentNoise.Generate(position * 1000f + continentOffset); //KURWA НЕ
        continentShape = SmoothMax(continentShape, -oceanFloorDepth, oceanFloorSmoothing);

        if (continentShape < 0)
        {
            continentShape *= 1 + oceanDepthMultiplier;
        }

        float ridgeValue = ridgeNoise.GenerateRidged(position * 1000f + ridgeOffset);
        float maskValue = maskNoise.Generate(position * 1000f + maskOffset);
        float maskedRidgeValue = ridgeValue * Mathf.Lerp(0, mountainBlend, maskValue);

        float finalHeight = 1 + continentShape * 0.01f + maskedRidgeValue * 0.01f;

        return finalHeight;
    }

    private float SmoothMax(float a, float b, float k)
    {
        float h = Mathf.Clamp01((a - b + k) / (2 * k));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }
}
