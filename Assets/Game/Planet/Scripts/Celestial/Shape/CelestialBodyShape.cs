using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CelestialBodyShape : ScriptableObject
{
    public bool randomize;
    public int seed;

    public bool perturbVertices;
    [Range(0, 1)]
    public float perturbStrength = 0.7f;

    public event System.Action OnSettingChanged;

    public virtual float[] CalculateHeights(Vector3[] vertices)
    {
        SetShapeData();
        float[] heights = new float[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            heights[i] = (float)CalculateHeight(vertices[i]);
        }

        return heights;
    }

    protected abstract double CalculateHeight(Vector3 position);

    protected virtual void SetShapeData() { }

    protected virtual void OnValidate()
    {
        if (OnSettingChanged != null)
        {
            OnSettingChanged();
        }
    }
}
