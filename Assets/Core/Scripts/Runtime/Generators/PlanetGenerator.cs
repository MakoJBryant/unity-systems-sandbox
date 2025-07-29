using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    [Range(2, 256)] public int resolution = 64;
    public float radius = 1f;
    [Range(0f, 1f)] public float seaLevel = 0.5f;
    public Light sceneSunLight;

    [Header("Settings Assets")]
    public TerrainSettings terrainSettings;
    public ColorSettings colorSettings;

    [Range(0.5f, 1.5f)] public float atmosphereExpansionFactor = 1.02f;

    public Transform sunTransform;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;

    private GameObject oceanGameObject;
    private MeshFilter oceanMeshFilter;
    private MeshRenderer oceanMeshRenderer;
    private Mesh oceanMesh;

    private GameObject atmosphereGameObject;
    private MeshFilter atmosphereMeshFilter;
    private MeshRenderer atmosphereMeshRenderer;
    private Mesh atmosphereMesh;
    private AtmosphereController atmosphereController;

    private float minElevation;
    private float maxElevation;
    private Texture2D biomeTexture;

    // REMOVE THIS LINE:
    // private TerrainGenerator terrainGenerator;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        mesh = new Mesh { name = "Generated Planet Mesh" };
        meshFilter.sharedMesh = mesh;

        // REMOVE THIS LINE:
        // terrainGenerator = new TerrainGenerator(terrainSettings);

        GeneratePlanet();
    }

    void OnValidate()
    {
        if (terrainSettings != null && colorSettings != null)
            GeneratePlanet();
    }

    [ContextMenu("Generate Planet Now")]
    public void GeneratePlanet()
    {
        if (terrainSettings == null || colorSettings == null)
        {
            Debug.LogWarning("TerrainSettings or ColorSettings missing—aborting planet generation.");
            return;
        }

        if (terrainSettings.noiseLayers == null || terrainSettings.noiseLayers.Length == 0)
        {
            Debug.LogWarning("No noise layers found in TerrainSettings—aborting planet generation.");
            return;
        }

        mesh.Clear();
        SphereCreator.CreateSphereMesh(resolution, radius, out Vector3[] vertices, out int[] triangles, out Vector2[] uvs);

        // DIAGNOSTIC: Calculate centroid before terrain deformation
        Vector3 centroidBefore = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
            centroidBefore += vertices[i];
        centroidBefore /= vertices.Length;
        Debug.Log($"[PlanetGenerator] Mesh centroid before displacement: {centroidBefore}");

        // CALL STATIC METHOD DIRECTLY - NO INSTANCE:
        // Assuming the signature:
        // TerrainGenerator.ApplyToMesh(ref Vector3[] vertices, float baseRadius, out float minElevation, out float maxElevation, TerrainSettings settings)
        // or adjust parameters as needed.

        TerrainGenerator.ApplyTerrainDeformation(vertices, terrainSettings, out Vector3[] displacedVertices, out minElevation, out maxElevation);
        vertices = displacedVertices;

        // DIAGNOSTIC: Calculate centroid after terrain deformation
        Vector3 centroidAfter = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
            centroidAfter += vertices[i];
        centroidAfter /= vertices.Length;
        Debug.Log($"[PlanetGenerator] Mesh centroid after displacement: {centroidAfter}");

        // Calculate normals from triangles
        var normals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
            Vector3 faceNormal = Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]).normalized;
            normals[a] += faceNormal;
            normals[b] += faceNormal;
            normals[c] += faceNormal;
        }
        for (int i = 0; i < normals.Length; i++) normals[i].Normalize();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        // Update material properties
        meshRenderer.sharedMaterial = colorSettings.planetMaterial;
        meshRenderer.sharedMaterial.SetFloat("_Radius", radius);
        meshRenderer.sharedMaterial.SetFloat("_MinHeight", minElevation);
        meshRenderer.sharedMaterial.SetFloat("_MaxHeight", maxElevation);
        meshRenderer.sharedMaterial.SetVector("_PlanetCenter", transform.position);

        biomeTexture = UpdateBiomeTexture();
        if (biomeTexture != null)
            meshRenderer.sharedMaterial.SetTexture("_BiomeTexture", biomeTexture);

        meshCollider.sharedMesh = mesh;

        AlignChildObjects();
        GenerateOceanPlane();
        GenerateAtmospherePlane();
    }

    void LateUpdate()
    {
        AlignChildObjects();

        if (sunTransform != null)
        {
            float orbitSpeed = 10f;
            transform.RotateAround(sunTransform.position, Vector3.up, orbitSpeed * Time.deltaTime);

            meshRenderer.sharedMaterial.SetVector("_PlanetCenter", transform.position);

            if (oceanMeshRenderer != null)
                oceanMeshRenderer.sharedMaterial.SetVector("_PlanetCenter", transform.position);

            if (atmosphereMeshRenderer != null)
                atmosphereMeshRenderer.sharedMaterial.SetVector("_PlanetCenter", transform.position);
        }
    }

    private void AlignChildObjects()
    {
        if (oceanGameObject != null)
            oceanGameObject.transform.SetParent(transform, false);

        if (atmosphereGameObject != null)
            atmosphereGameObject.transform.SetParent(transform, false);
    }

    void GenerateOceanPlane()
    {
        oceanGameObject = OceanGenerator.GenerateOcean(
            transform,
            resolution,
            minElevation,
            maxElevation,
            seaLevel,
            colorSettings,
            ref oceanGameObject,
            ref oceanMeshFilter,
            ref oceanMeshRenderer,
            ref oceanMesh
        );
    }

    void GenerateAtmospherePlane()
    {
        atmosphereGameObject = AtmosphereGenerator.GenerateAtmosphere(
            transform,
            resolution,
            maxElevation,
            atmosphereExpansionFactor,
            colorSettings,
            sceneSunLight,
            ref atmosphereGameObject,
            ref atmosphereMeshFilter,
            ref atmosphereMeshRenderer,
            ref atmosphereMesh,
            ref atmosphereController
        );

        Debug.Log($"Atmosphere Transform - Position: {atmosphereGameObject.transform.position}, Rotation: {atmosphereGameObject.transform.rotation.eulerAngles}, Scale: {atmosphereGameObject.transform.localScale}");
    }

    Texture2D UpdateBiomeTexture()
    {
        var biomeSettings = colorSettings.biomeSettings;
        if (biomeSettings == null || biomeSettings.biomes == null || biomeSettings.biomes.Length == 0)
            return null;

        var biomes = biomeSettings.biomes;

        int texRes = 256;
        Texture2D texture = new Texture2D(texRes, 1, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[texRes];

        System.Array.Sort(biomes, (a, b) => a.startHeight.CompareTo(b.startHeight));

        for (int i = 0; i < texRes; i++)
        {
            float h = i / (float)(texRes - 1);
            Color col = biomes[0].color;

            foreach (var biome in biomes)
            {
                float blend = Mathf.Clamp01((h - biome.startHeight) / biome.blendAmount);
                col = Color.Lerp(col, biome.color, blend);
            }

            pixels[i] = col;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    void OnDestroy()
    {
        if (biomeTexture != null) DestroyImmediate(biomeTexture);
        if (oceanGameObject != null) DestroyImmediate(oceanGameObject);
        if (atmosphereGameObject != null) DestroyImmediate(atmosphereGameObject);
    }
}
