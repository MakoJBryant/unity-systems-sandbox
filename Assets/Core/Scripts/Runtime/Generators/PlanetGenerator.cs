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
    public BiomeSettings biomeSettings;
    public OceanSettings oceanSettings;
    public AtmosphereSettings atmosphereSettings;

    [Range(0.5f, 1.5f)] public float atmosphereExpansionFactor = 1.02f;

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

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        mesh = new Mesh { name = "Generated Planet Mesh" };
        meshFilter.sharedMesh = mesh;

        GeneratePlanet();
    }

    void OnValidate()
    {
        if (terrainSettings && biomeSettings && oceanSettings && atmosphereSettings)
            GeneratePlanet();
    }

    [ContextMenu("Generate Planet Now")]
    public void GeneratePlanet()
    {
        if (!terrainSettings || !biomeSettings || !oceanSettings || !atmosphereSettings)
        {
            Debug.LogWarning("Missing required settings assets—aborting planet generation.");
            return;
        }

        if (terrainSettings.noiseLayers == null || terrainSettings.noiseLayers.Length == 0)
        {
            Debug.LogWarning("No noise layers found in TerrainSettings—aborting planet generation.");
            return;
        }

        mesh = PlanetMeshBuilder.Build(resolution, radius, terrainSettings, out minElevation, out maxElevation);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        biomeTexture = BiomeGenerator.GenerateBiomeTexture(biomeSettings);

        PlanetVisualUpdater.ApplyMaterialProperties(meshRenderer.sharedMaterial, radius, minElevation, maxElevation, transform.position, biomeTexture);

        AlignChildObjects();
        GenerateOceanPlane();
        GenerateAtmospherePlane();
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
            oceanSettings.seaLevel,
            oceanSettings,
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
            atmosphereSettings,
            sceneSunLight,
            ref atmosphereGameObject,
            ref atmosphereMeshFilter,
            ref atmosphereMeshRenderer,
            ref atmosphereMesh,
            ref atmosphereController
        );

        Debug.Log($"Atmosphere Transform - Position: {atmosphereGameObject.transform.position}, Rotation: {atmosphereGameObject.transform.rotation.eulerAngles}, Scale: {atmosphereGameObject.transform.localScale}");
    }

    void OnDestroy()
    {
        if (biomeTexture != null) DestroyImmediate(biomeTexture);
        if (oceanGameObject != null) DestroyImmediate(oceanGameObject);
        if (atmosphereGameObject != null) DestroyImmediate(atmosphereGameObject);
    }
}
