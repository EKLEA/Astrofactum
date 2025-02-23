using UnityEngine;

[System.Serializable]
public class SimpleNoiseSettings
{
    public int numLayers = 4;
    public float lacunarity = 2;
    public float persistence = 0.5f;
    public float scale = 1;
    public float elevation = 1;
    public float verticalShift = 0;
    public Vector3 offset;
    public FastNoiseLite noiseGenerator = new FastNoiseLite();

    public Vector3 GetOffset(PRNG prng)
    {
        return offset + new Vector3(prng.Value(), prng.Value(), prng.Value()) * 10000;
    }

    public float Generate(Vector3 position)
    {
        float noiseValue = 0;
        float frequency = scale;
        float amplitude = elevation;

        for (int i = 0; i < numLayers; i++)
        {
            float v = noiseGenerator.GetNoise(position.x * frequency, position.y * frequency, position.z * frequency);
            noiseValue += v * amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return noiseValue + verticalShift;
    }
}