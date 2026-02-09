using UnityEditor;
using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

namespace MakoJBryant.SolarSystem.Editor
{
    // Custom inspector for ShapeSettings
    [CustomEditor(typeof(ShapeSettings))]
    public class ShapeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
