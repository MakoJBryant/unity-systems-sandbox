using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    public Light sceneSunLight;

    [Header("Subsystem References")]
    public ShapeGenerator shapeGenerator;

    [Header("Settings Assets")]
    public BiomeSettings biomeSettings;
    public OceanSettings oceanSettings;
    public AtmosphereSettings atmosphereSettings;

    [Header("Manual Ocean Control")]
    [Tooltip("If > 0, overrides automatic ocean radius (world units).")]
    public float manualOceanRadius = 0f;

    private MeshRenderer meshRenderer;

    private GameObject oceanGameObject;
    private MeshFilter oceanMeshFilter;
    private MeshRenderer oceanMeshRenderer;
    private Mesh oceanMesh;

    private GameObject atmosphereGameObject;
    private MeshFilter atmosphereMeshFilter;
    private MeshRenderer atmosphereMeshRenderer;
    private Mesh atmosphereMesh;

    private Texture2D biomeTexture;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (shapeGenerator == null)
            shapeGenerator = GetComponent<ShapeGenerator>();

        GeneratePlanet();
    }

    void OnValidate()
    {
        if (shapeGenerator && biomeSettings && oceanSettings && atmosphereSettings)
            GeneratePlanet();
    }

    [ContextMenu("Generate Planet Now")]
    public void GeneratePlanet()
    {
        if (!shapeGenerator || !biomeSettings || !oceanSettings || !atmosphereSettings)
            return;

        if (shapeGenerator.shapeSettings == null)
            return;

        // 1. Generate shape + terrain
        shapeGenerator.GenerateShape();

        float minElevation = shapeGenerator.MinElevation;
        float maxElevation = shapeGenerator.MaxElevation;
        float radius = shapeGenerator.radius;

        // 2. Biome texture
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
        GenerateOcean(minElevation, maxElevation);
        GenerateAtmosphere(radius, maxElevation);
    }

    private void AlignChildObjects()
    {
        if (oceanGameObject != null)
            oceanGameObject.transform.SetParent(transform, false);

        if (atmosphereGameObject != null)
            atmosphereGameObject.transform.SetParent(transform, false);
    }

    private void GenerateOcean(float minElevation, float maxElevation)
    {
        oceanGameObject = OceanGenerator.GenerateOcean(
            transform,
            shapeGenerator.resolution,
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

    private void GenerateAtmosphere(float radius, float maxElevation)
    {
        atmosphereGameObject = AtmosphereGenerator.GenerateAtmosphere(
            transform,
            shapeGenerator.resolution,
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
        if (Application.isPlaying &&
            atmosphereMeshRenderer != null &&
            atmosphereSettings != null &&
            sceneSunLight != null)
        {
            atmosphereMeshRenderer.sharedMaterial.SetVector(
                "_SunDirection",
                sceneSunLight.transform.forward
            );
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
