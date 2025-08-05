using UnityEngine;

public static class PlanetVisualUpdater
{
    public static void ApplyMaterialProperties(Material mat, float radius, float minHeight, float maxHeight, Vector3 center, Texture2D biomeTex)
    {
        if (mat == null) return;

        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_MinHeight", minHeight);
        mat.SetFloat("_MaxHeight", maxHeight);
        mat.SetVector("_PlanetCenter", center);
        if (biomeTex != null)
            mat.SetTexture("_BiomeTexture", biomeTex);
    }
}
