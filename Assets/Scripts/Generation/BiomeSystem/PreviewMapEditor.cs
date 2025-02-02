using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PreviewMap))]
public class PreviewMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        PreviewMap previewMap = (PreviewMap)target;
        
        if (GUILayout.Button("Generate Biome Map"))
        {
            previewMap.GenerateAndDisplayMap();
        }
    }
}