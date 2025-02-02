using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeSettings", menuName = "Biome System/Biome Settings")]
public class BiomeSettings : ScriptableObject
{
    public Biome[] biomes;
}
