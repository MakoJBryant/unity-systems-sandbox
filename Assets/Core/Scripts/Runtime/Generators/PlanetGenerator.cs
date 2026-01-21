using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    [Range(2, 256)] public int resolution = 64;
    public float radius = 1000f;
    public Light sceneSunLight;

    [Header("Settings Assets")]
    public TerrainSettings terrainSettings;
    public BiomeSettings biomeSettings;
    public OceanSettings oceanSettings;
    public AtmosphereSettings atmosphereSettings;

    [Header("Manual Ocean Control")]
    [Tooltip("If > 0, overrides automatic ocean radius (world units).")]
    public float manualOceanRadius = 0f;

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

    private float minElevation;
    private float maxElevation;
    private Texture2D biomeTexture;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (mesh == null)
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
        if (!terrainSettings || !biomeSettings || !oceanSettings || !atmosphereSettings) return;

        if (terrainSettings.noiseLayers == null || terrainSettings.noiseLayers.Length == 0) return;

        // Build planet mesh
        mesh = PlanetMeshBuilder.Build(resolution, radius, terrainSettings, out minElevation, out maxElevation);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Biome texture
        biomeTexture = BiomeGenerator.GenerateBiomeTexture(biomeSettings);

        PlanetVisualUpdater.ApplyMaterialProperties(
            meshRenderer.sharedMaterial,
            radius,
            minElevation,
            maxElevation,
            transform.position,
            biomeTexture
        );

        AlignChildObjects();
        GenerateOceanPlane();
        GenerateAtmosphere();
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
            ref oceanMesh,
            manualOceanRadius
        );
    }

    void GenerateAtmosphere()
    {
        atmosphereGameObject = AtmosphereGenerator.GenerateAtmosphere(
            transform,
            resolution,
            radius,
            maxElevation,
            atmosphereSettings,
            sceneSunLight,
            ref atmosphereGameObject,
            ref atmosphereMeshFilter,
            ref atmosphereMeshRenderer,
            ref atmosphereMesh
        );
    }

    void Update()
    {
        if (Application.isPlaying && atmosphereMeshRenderer != null && atmosphereSettings != null && sceneSunLight != null)
        {
            atmosphereMeshRenderer.sharedMaterial.SetVector("_SunDirection", sceneSunLight.transform.forward);
        }
    }

    void OnDestroy()
    {
        if (biomeTexture != null)
            DestroyImmediate(biomeTexture);

        if (oceanGameObject != null)
            DestroyImmediate(oceanGameObject);

        if (atmosphereGameObject != null)
            DestroyImmediate(atmosphereGameObject);
    }
}
