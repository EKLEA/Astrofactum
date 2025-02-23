using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{

    Editor shapeEditor;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Planet planet = (Planet)target;

        if (GUILayout.Button("Generate Entire Planet"))
        {
            if(RequireQuadTemplate()) {
                planet.distanceToPlayer = planet.Size;
                planet.distanceToPlayerPow2 = planet.distanceToPlayer * planet.distanceToPlayer;
                planet.position = planet.transform.position;
                planet.Initialize();
                planet.GenerateMesh();
                //planet.GenerateTexture();
                //planet.UpdateShaders();
                planet.CachedPlanet = planet.CachePlanet();
                planet.Update();
            }
        }

        /// <summary>
        /// Returns true if quad templates have been generated and false if they have not, along with logging a warning.
        /// </summary>
        bool RequireQuadTemplate() {
            if(!Presets.Generated) {
                foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if(obj.GetComponent<Presets>() != null) { 
                        Debug.LogWarning("QUAD TEMPLATE MISSING. Generate one by pressing the button on the Presets component.");
                        EditorGUIUtility.PingObject(obj);
                        break;
                    };
                }
            }

            return Presets.Generated;
        }

        DrawSettingsEditor(planet.shapeConfig, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }
}
