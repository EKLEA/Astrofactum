using UnityEngine;

[System.Serializable]
public class RidgeNoiseSettings
{
    public int numLayers = 4;
    public float lacunarity = 2;
    public float persistence = 0.5f;
    public float scale = 1;
    public float elevation = 1;
    public float verticalShift = 0;
    public Vector3 offset;
    public float power = 2;
    public float gain = 1;
    public float peakSmoothing = 0;
    public FastNoiseLite noiseGenerator = new FastNoiseLite();

    public Vector3 GetOffset(PRNG prng)
    {
        return offset + new Vector3(prng.Value(), prng.Value(), prng.Value()) * 10000;
    }

    public float GenerateRidged(Vector3 position)
    {
        float noiseValue = 0;
        float frequency = scale;
        float amplitude = elevation;

        for (int i = 0; i < numLayers; i++)
        {
            float v = noiseGenerator.GetNoise(position.x * frequency, position.y * frequency, position.z * frequency);
            v = 1 - Mathf.Abs(v * 2 - 1); // Ridge effect
            v = Mathf.Pow(v, power);
            v *= gain;
            noiseValue += v * amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - peakSmoothing); // Сглаживание пиков
        return noiseValue + verticalShift;
    }
}
