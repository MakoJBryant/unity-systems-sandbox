using UnityEngine;
using UnityEditor;
using MakoJBryant.SolarSystem.Generation;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    PlanetGenerator planetGenerator;
    Editor terrainSettingsEditor;
    Editor colorSettingsEditor;

    void OnEnable()
    {
        planetGenerator = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (planetGenerator.terrainSettings == null)
        {
            EditorGUILayout.HelpBox("Terrain Settings asset is not assigned!", MessageType.Warning);
        }

        if (planetGenerator.colorSettings == null)
        {
            EditorGUILayout.HelpBox("Color Settings asset is not assigned!", MessageType.Warning);
        }

        if (GUILayout.Button("Generate Planet Now"))
        {
            planetGenerator.GeneratePlanet();
        }

        if (planetGenerator.terrainSettings != null)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Terrain Settings", EditorStyles.boldLabel);

                CreateCachedEditor(planetGenerator.terrainSettings, null, ref terrainSettingsEditor);
                terrainSettingsEditor.OnInspectorGUI();

                if (check.changed)
                {
                    EditorUtility.SetDirty(planetGenerator.terrainSettings);
                    planetGenerator.GeneratePlanet();
                }
            }
        }

        if (planetGenerator.colorSettings != null)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);

                CreateCachedEditor(planetGenerator.colorSettings, typeof(ColorSettingsEditor), ref colorSettingsEditor);
                colorSettingsEditor.OnInspectorGUI();

                if (check.changed)
                {
                    EditorUtility.SetDirty(planetGenerator.colorSettings);
                    planetGenerator.GeneratePlanet();
                }
            }
        }
    }
}
