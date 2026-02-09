using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    PlanetGenerator planet;
    Editor shapeEditor;

    void OnEnable()
    {
        planet = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        if (planet.shapeGenerator == null)
        {
            EditorGUILayout.HelpBox(
                "ShapeGenerator is not assigned. PlanetGenerator needs a ShapeGenerator component.",
                MessageType.Warning
            );
        }
        else if (planet.shapeGenerator.shapeSettings == null)
        {
            EditorGUILayout.HelpBox(
                "ShapeSettings asset is not assigned on the ShapeGenerator.",
                MessageType.Warning
            );
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel);

            CreateCachedEditor(
                planet.shapeGenerator.shapeSettings,
                null,
                ref shapeEditor
            );

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                shapeEditor.OnInspectorGUI();

                if (check.changed)
                {
                    EditorUtility.SetDirty(planet.shapeGenerator.shapeSettings);
                    planet.GeneratePlanet();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
