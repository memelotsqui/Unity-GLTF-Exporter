using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    [CreateAssetMenu(fileName = "gltf options", menuName = "ScriptableObjects/gltf options", order = 1)]
    public class SO_ExportGLTFOptions : ScriptableObject
    {
        // GENERIC EXTRAS
        // NODES - OPTIONS
        public bool exportGameObjectName = true;
        public bool exportMaterialName = true;
        public bool exportTexturesName =true;
        public bool exportCameras = true;
        public bool exportInactive = true;
        public bool exportGameObjectsTag =true;
        public bool exportGameObjectsLayer = true;
        public bool exportBatching = true;
        public bool exportNavMesh = true;
        public bool blockLeafNodesCreation = false;

        //ANIMATIONS
        public bool exportAnimations = true;

        //MESH OPTIONS
        public bool reduceGLTFChars = false;
        public bool exportNormals =true;
        public bool exportLightmapUVs = true;               // NOT WORKING YET, RIGHT NOW IT WILL ALWAYS EXPORT LIGHTMAP UVS
        public bool convertToGLB = false;                   // NOT WORKING YET.
        public bool createUVOffsetExtras = false;           // NOT WORKING YET: IF SET TO TRUE, FINAL SIZE WILL BE REDUCED, UVS WILL BE SAVED ONCE, AND OFFSET/SCALE WILL BE SAVED IN GLTF "EXTRAS" CODE FOR THREE JS GLTF IMPORTER TO USE
        public bool exportSubmeshesInExtra = true;          // SUBMESHES ARE EXPORTED SEPARATED IN CLASSIC GLTF, SHOULD BE EXPORTED COMBINED AND SAVE GROUPED POSITIONS

        //TEXTURES QUALITY
        public float colorTextureSaturation = 0.8f;
        public float metallicMultiplier = 0.8f;
        public float smoothnessMultiplier = 0.8f;
        public bool exportSeparatedAlphaMap = true;
        public TextureExportType exportTextureType = TextureExportType.DEFAULT;
        public bool fallbackGLTFTexture = false;            // NEW 7/22/2021

        public TextureDivideFactor allImagesDivideFactor = TextureDivideFactor.FULL;
        public TextureDivideFactor overrideLightmapDivideFactor = TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideDefaultDivideFactor = TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideCubemapDivideFactor = TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideMetallicSmoothnessDivideFactor = TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideNormalDivideFactor = TextureDivideFactor.NONE_SET;

        public int allImagesQuality = 100;
        public int overrideLightmapQuality;         // SET 0 TO NOT OVERRIDE
        public int overrideNormalQuality;
        public int overrideMetallicSmoothnessQuality;
        public int overrideCubemapQuality;
        public int overrideDefaultQuality;
        public TextureExportType overrideLightmapExportTextureType = TextureExportType.DEFAULT;
        public TextureExportType overrideNormalExportTextureType = TextureExportType.NONE_SET;
        public TextureExportType overrideMetallicSmoothnessExportTextureType = TextureExportType.NONE_SET;
        public TextureExportType overrideDefaultExportTextureType = TextureExportType.NONE_SET;
        public TextureExportType overrideCubemapTextureType = TextureExportType.NONE_SET;

        //LIGHTMAP OPTIONS
        public float maxLightmapClamp = 5f;
        public float saturationLightmap = 1f;
        public bool whiteImageForNonStaticLightmaps = true;
        public float lightmapIntensityMultiplier = 1f;
        public float lightmapContrastCheat = 1f;

        //QUANTIZATION
        public bool quantizeGLTF = false;
        public ComponentTypeSelected quantizeMainUVsTo;
        public ComponentTypeSelected quantizeLightUVsTo;
        public ComponentTypeSelected quantizeVerticesTo;
        public ComponentTypeSelected quantizeNormalsTo;

        // SCENE
        public bool exportSceneSkyboxEnvironment = true;
        public bool exportSceneSkyboxBackground = true;
        public bool createCubeForSkybox = true; //will get deleted
        //public GameObject cubeSkybox = null;
        public int cubeSkyboxScale = 1; //1 = 1000
        public bool exportSceneSkyboxBackgroundEvenDefault = false;
        public bool exportSceneFog = true;
        //public Cubemap computedEnvironmentCubemap;
        //public Cubemap computedBackgroundCubemap;

        public int overrideEnvironmentQuality = 0;

        //SET MAIN CAMERA 
        public bool extraExportMainCameraInGLTF = false;
        //public int extraCameraIndex = -1;

    }
}