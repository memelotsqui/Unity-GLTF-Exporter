using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ExportGLTFOptions 
    {
     
        // LOCATION
        public string exportLocation = "";                  // SET ONCE EXPORT TO GLTF IS CALLED

        // GENERIC EXTRAS
        // NODES - OPTIONS
        public bool exportGameObjectName;
        public bool exportMaterialName;
        public bool exportTexturesName;
        public bool exportCameras;
        public bool exportInactive;
        public bool exportGameObjectsTag;
        public bool exportGameObjectsLayer;
        public bool exportBatching;
        public bool exportNavMesh;
        public bool blockLeafNodesCreation;

        //ANIMATIONS
        public bool exportAnimations = true;

        //MESH OPTIONS
        public bool reduceGLTFChars;
        public bool exportNormals;
        public bool exportLightmapUVs;              // NOT WORKING YET, RIGHT NOW IT WILL ALWAYS EXPORT LIGHTMAP UVS
        public bool convertToGLB;                   // NOT WORKING YET.
        public bool createUVOffsetExtras;           // NOT WORKING YET: IF SET TO TRUE, FINAL SIZE WILL BE REDUCED, UVS WILL BE SAVED ONCE, AND OFFSET/SCALE WILL BE SAVED IN GLTF "EXTRAS" CODE FOR THREE JS GLTF IMPORTER TO USE
        public bool exportSubmeshesInExtra;         // SUBMESHES ARE EXPORTED SEPARATED IN CLASSIC GLTF, SHOULD BE EXPORTED COMBINED AND SAVE GROUPED POSITIONS

        //TEXTURES QUALITY
        public float colorTextureSaturation;
        public float metallicMultiplier;
        public float smoothnessMultiplier;
        public bool exportSeparatedAlphaMap;
        public TextureExportType exportTextureType;
        public bool fallbackGLTFTexture;

        public TextureDivideFactor allImagesDivideFactor= TextureDivideFactor.FULL;
        public TextureDivideFactor overrideLightmapDivideFactor= TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideNormalDivideFactor= TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideMetallicSmoothnessDivideFactor= TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideCubemapDivideFactor= TextureDivideFactor.NONE_SET;
        public TextureDivideFactor overrideDefaultDivideFactor = TextureDivideFactor.NONE_SET;
       // public int 

        public int allImagesQuality;
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
        public float maxLightmapClamp;
        public float saturationLightmap;
        public bool whiteImageForNonStaticLightmaps;
        public float lightmapIntensityMultiplier;
        public float lightmapContrastCheat;

        //QUANTIZATION
        public bool quantizeGLTF;
        public ComponentTypeSelected quantizeMainUVsTo;
        public ComponentTypeSelected quantizeLightUVsTo;
        public ComponentTypeSelected quantizeVerticesTo;
        public ComponentTypeSelected quantizeNormalsTo;

        // SCENE
        public bool exportSceneSkyboxEnvironment; 
        public bool exportSceneSkyboxBackground;
        public bool createCubeForSkybox = true; //
        public GameObject cubeSkybox = null;
        public int cubeSkyboxScale = 1; //1 = 1000
        public bool exportSceneSkyboxBackgroundEvenDefault;
        public bool exportSceneFog;
        public Cubemap computedEnvironmentCubemap;
        public Cubemap computedBackgroundCubemap;
       
        public int overrideEnvironmentQuality = 0;

        //SET MAIN CAMERA 
        public bool extraExportMainCameraInGLTF = false;
        public int extraCameraIndex = -1;

        //(AUTO CODE SECTION, NOT GLTF)
        // BASIC VARS NAMES
        public string varCamName = "_camera";
        public string varRendererName = "_renderer";
        public string varContainerName = "_container";
        public string varGLTFName = "_gltf";
        public string varSceneName = "_scene";

        public ExportGLTFOptions()
        {
            GetPrefsValues();
            SetupTextureOverrides();
        }
        public ExportGLTFOptions(SO_ExportGLTFOptions user_options)
        {
            GetUserValues(user_options);
            SetupTextureOverrides();
        }
        public bool SaveFallback(TextureType texture_type)
        {
            if (!fallbackGLTFTexture)
                return false;

            TextureExportType exportType = exportTextureType;

            switch (texture_type)
            {
                case TextureType.DEFAULT:
                    exportType = overrideDefaultExportTextureType == TextureExportType.NONE_SET ? exportType : overrideDefaultExportTextureType;
                    break;
                case TextureType.LIGHTMAP:
                    exportType = overrideLightmapExportTextureType == TextureExportType.NONE_SET ? exportType : overrideLightmapExportTextureType;
                    break;
                case TextureType.METALLIC_SMOOTHNESS:
                    exportType = overrideMetallicSmoothnessExportTextureType == TextureExportType.NONE_SET ? exportType : overrideMetallicSmoothnessExportTextureType;
                    break;
                case TextureType.NORMAL:
                    exportType = overrideNormalExportTextureType == TextureExportType.NONE_SET ? exportType : overrideNormalExportTextureType;
                    break;
                case TextureType.CUBEMAP:
                    exportType = overrideCubemapTextureType == TextureExportType.NONE_SET ? exportType : overrideCubemapTextureType;
                    break;
            }

            if (exportType != TextureExportType.DEFAULT)
                return true;
            return false;
        }
        public TextureExportType GetExportTextureType(TextureType texture_type)
        {
            switch (texture_type)
            {
                case TextureType.DEFAULT:
                    return overrideDefaultExportTextureType;
                case TextureType.LIGHTMAP:
                    return overrideLightmapExportTextureType;
                case TextureType.METALLIC_SMOOTHNESS:
                    return overrideMetallicSmoothnessExportTextureType;
                case TextureType.NORMAL:
                    return overrideNormalExportTextureType;
                case TextureType.CUBEMAP:
                    return overrideCubemapTextureType;
            }
            return TextureExportType.DEFAULT;
        }
        //public bool 
        public bool ExportSeparatedAlphaMap()
        {
            if (exportSeparatedAlphaMap)
            {
                if (exportTextureType == TextureExportType.DEFAULT || fallbackGLTFTexture) // MAKE SURE TO ALSE SAVE SEPARATED ALPHA IF WE HAVE FALLBACK TEXTURE
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// order in list: Lightmaps = 0, Normal = 1; MetallicSmoothness = 2, Default = 3;
        /// </summary>
        /// <returns></returns>
        public List <int> GetImagesQuality()
        {
            List<int> result = new List<int>();
            if (overrideLightmapQuality != 0) result.Add(overrideLightmapQuality); else result.Add(allImagesQuality);
            if (overrideNormalQuality != 0) result.Add(overrideNormalQuality); else result.Add(allImagesQuality);
            if (overrideMetallicSmoothnessQuality != 0) result.Add(overrideMetallicSmoothnessQuality); else result.Add(allImagesQuality);
            if (overrideCubemapQuality != 0) result.Add(overrideCubemapQuality); else result.Add(allImagesQuality);
            if (overrideDefaultQuality != 0) result.Add(overrideDefaultQuality); else result.Add(allImagesQuality);

            return result;
        }

        public int GetImagesDivideFactor(string imageType)
        {
            int _allImagesFactor = (int)allImagesDivideFactor >= 1 ? (int)allImagesDivideFactor : 1;
            switch (imageType)
            {
                case "lightmap":
                    return (int)overrideLightmapDivideFactor != -1 ? (int)overrideLightmapDivideFactor : _allImagesFactor;
                case "normal":
                    return (int)overrideNormalDivideFactor != -1 ? (int)overrideNormalDivideFactor : _allImagesFactor;
                case "metallicSmoothness":
                    return (int)overrideMetallicSmoothnessDivideFactor != -1 ? (int)overrideMetallicSmoothnessDivideFactor : _allImagesFactor;
                case "cubemap":
                    return (int)overrideCubemapDivideFactor != -1 ? (int)overrideCubemapDivideFactor : _allImagesFactor;
                case "default":
                    return (int)overrideDefaultDivideFactor != -1 ? (int)overrideDefaultDivideFactor : _allImagesFactor;
            }
            return _allImagesFactor;
        }
        public void SetupTextureOverrides()
        {
            if (exportTextureType == TextureExportType.DEFAULT)
            {
                overrideLightmapExportTextureType = exportTextureType;
                overrideNormalExportTextureType = exportTextureType;
                overrideMetallicSmoothnessExportTextureType = exportTextureType;
                overrideDefaultExportTextureType = exportTextureType;
                overrideCubemapTextureType = exportTextureType;
            }
            else
            {
                if (overrideLightmapExportTextureType == TextureExportType.NONE_SET) overrideLightmapExportTextureType = exportTextureType;
                if (overrideNormalExportTextureType == TextureExportType.NONE_SET) overrideNormalExportTextureType = exportTextureType;
                if (overrideMetallicSmoothnessExportTextureType == TextureExportType.NONE_SET) overrideMetallicSmoothnessExportTextureType = exportTextureType;
                if (overrideDefaultExportTextureType == TextureExportType.NONE_SET) overrideDefaultExportTextureType = exportTextureType;
                if (overrideCubemapTextureType == TextureExportType.NONE_SET) overrideCubemapTextureType = exportTextureType;
            }
    }
        public bool RequiresKTX2()
        {
            if (exportTextureType == TextureExportType.KTX2)
                return true;
            return false;
        }

        public ExportGLTFOptions(string cam_name, string renderer_name, string container_name, string gltf_name, string scene_name)
        {
            varCamName = cam_name;
            GetPrefsValues();
            if (exportTextureType == TextureExportType.DEFAULT)
                fallbackGLTFTexture = false;

        }

        public void GetPrefsValues()
        {
            //TEXTURE QUALITY
            colorTextureSaturation = PlayerPrefs.GetFloat("gltf_o_colorTextureSaturation", 1f);
            metallicMultiplier = PlayerPrefs.GetFloat("gltf_o_metallicMultiplier", 0.8f);
            smoothnessMultiplier = PlayerPrefs.GetFloat("gltf_o_smoothnessMultiplier", 0.8f);
            exportSeparatedAlphaMap = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSeparatedAlphaMap", 0));
            exportTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_exportTextureType", (int)TextureExportType.DEFAULT);
            fallbackGLTFTexture = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_fallbackGLTFTexture", 0));
            allImagesQuality = PlayerPrefs.GetInt("gltf_o_allImagesQuality", 90);
            overrideLightmapQuality = PlayerPrefs.GetInt("gltf_o_overrideLightmapQuality", 0);
            allImagesDivideFactor = (TextureDivideFactor)PlayerPrefs.GetInt("gltf_o_allImagesDivideFactor", 0);
            overrideNormalQuality = PlayerPrefs.GetInt("gltf_o_overrideNormalQuality", 0);
            overrideMetallicSmoothnessQuality = PlayerPrefs.GetInt("gltf_o_overrideMetallicSmoothnessQuality", 0);
            overrideCubemapQuality = PlayerPrefs.GetInt("gltf_o_overrideCubemapQuality", 0);
            overrideDefaultQuality = PlayerPrefs.GetInt("gltf_o_overrideDefaultQuality", 0);

            overrideDefaultExportTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_overrideDefaultExportTextureType", (int)TextureExportType.NONE_SET);
            overrideLightmapExportTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_overrideLightmapExportTextureType", (int)TextureExportType.NONE_SET);
            overrideMetallicSmoothnessExportTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_overrideMetallicSmoothnessExportTextureType", (int)TextureExportType.NONE_SET);
            overrideNormalExportTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_overrideNormalExportTextureType", (int)TextureExportType.NONE_SET);
            overrideCubemapTextureType = (TextureExportType)PlayerPrefs.GetInt("gltf_o_overrideCubemapTextureType", (int)TextureExportType.NONE_SET);

            //LIGHTMAP
            maxLightmapClamp = PlayerPrefs.GetFloat("gltf_o_quantizeMainUVsTo", 5f);
            saturationLightmap = PlayerPrefs.GetFloat("gltf_o_saturationLightmap", 1f);
            lightmapIntensityMultiplier = PlayerPrefs.GetFloat("gltf_o_lightmapIntensityMultiplier", 1f);
            lightmapContrastCheat = PlayerPrefs.GetFloat("gltf_o_lightmapContrastCheat", 1f);
            whiteImageForNonStaticLightmaps = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_whiteImageForNonStaticLightmaps", 0));

            //QUANTIZATION
            quantizeGLTF = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_quantizeGLTF", 0));
            quantizeMainUVsTo = (ComponentTypeSelected)PlayerPrefs.GetInt("gltf_o_quantizeMainUVsTo", (int)ComponentTypeSelected.FLOAT);
            quantizeLightUVsTo = (ComponentTypeSelected)PlayerPrefs.GetInt("gltf_o_quantizeLightUVsTo", (int)ComponentTypeSelected.FLOAT);
            quantizeVerticesTo = (ComponentTypeSelected)PlayerPrefs.GetInt("gltf_o_quantizeVerticesTo", (int)ComponentTypeSelected.FLOAT);
            quantizeNormalsTo = (ComponentTypeSelected)PlayerPrefs.GetInt("gltf_o_quantizeNormalsTo", (int)ComponentTypeSelected.FLOAT);

            // OTHER OPTIONS
            exportGameObjectName = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportGameObjectName", 1));
            exportMaterialName = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportMaterialName", 1));
            exportTexturesName = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportTexturesName", 1));
            exportCameras = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportCameras", 1));
            exportInactive = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportInactive", 0));
            exportGameObjectsTag = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportGameObjectsTag", 0));
            exportGameObjectsLayer = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportGameObjectsLayer", 0));
            exportBatching = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportBatching", 0));
            exportNavMesh = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportNavMesh", 0));
            exportSubmeshesInExtra = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSubmeshesInExtra", 0));
            reduceGLTFChars = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_reduceGLTFChars", 0));
            exportNormals = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportNormals", 1));
            exportLightmapUVs = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportLightmapUVs", 1));
            convertToGLB = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_convertToGLB", 0));
            createUVOffsetExtras = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_createUVOffsetExtras", 0));

            //SCENE EXTRAS
            exportSceneSkyboxEnvironment = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSceneSkyboxEnvironment", 0));
            exportSceneSkyboxBackground = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_createCubeForSkybox", 0));
            createCubeForSkybox = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSceneSkyboxBackground", 0));
            exportSceneSkyboxBackgroundEvenDefault = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSceneSkyboxBackgroundEvenDefault", 0));
            exportSceneFog = GetBoolFromInt(PlayerPrefs.GetInt("gltf_o_exportSceneFog", 0));

            
        }
        private void GetUserValues(SO_ExportGLTFOptions user_options)
        {
            //TEXTURE QUALITY
            colorTextureSaturation = user_options.colorTextureSaturation;
            metallicMultiplier = user_options.metallicMultiplier;
            smoothnessMultiplier = user_options.smoothnessMultiplier;
            exportSeparatedAlphaMap = user_options.exportSeparatedAlphaMap;
            exportTextureType = user_options.exportTextureType;
            fallbackGLTFTexture = user_options.fallbackGLTFTexture;
            allImagesQuality = user_options.allImagesQuality;

            overrideLightmapQuality = user_options.overrideLightmapQuality;
            overrideNormalQuality = user_options.overrideNormalQuality;
            overrideMetallicSmoothnessQuality = user_options.overrideMetallicSmoothnessQuality;
            overrideCubemapQuality = user_options.overrideCubemapQuality;
            overrideDefaultQuality = user_options.overrideDefaultQuality;

            allImagesDivideFactor = user_options.allImagesDivideFactor;
            overrideLightmapDivideFactor = user_options.overrideLightmapDivideFactor;
            overrideDefaultDivideFactor = user_options.overrideDefaultDivideFactor;
            overrideCubemapDivideFactor = user_options.overrideCubemapDivideFactor;
            overrideMetallicSmoothnessDivideFactor = user_options.overrideMetallicSmoothnessDivideFactor;
            overrideNormalDivideFactor = user_options.overrideNormalDivideFactor;

            overrideDefaultExportTextureType = user_options.overrideDefaultExportTextureType;
            overrideLightmapExportTextureType = user_options.overrideLightmapExportTextureType;
            overrideMetallicSmoothnessExportTextureType = user_options.overrideMetallicSmoothnessExportTextureType;
            overrideNormalExportTextureType = user_options.overrideNormalExportTextureType;
            overrideCubemapTextureType = user_options.overrideCubemapTextureType;

            //LIGHTMAP
            maxLightmapClamp = user_options.maxLightmapClamp;
            saturationLightmap = user_options.saturationLightmap;
            lightmapIntensityMultiplier = user_options.lightmapIntensityMultiplier;
            lightmapContrastCheat = user_options.lightmapContrastCheat;
            whiteImageForNonStaticLightmaps = user_options.whiteImageForNonStaticLightmaps;

            //QUANTIZATION
            quantizeGLTF = user_options.quantizeGLTF;
            quantizeMainUVsTo = user_options.quantizeMainUVsTo;
            quantizeLightUVsTo = user_options.quantizeLightUVsTo;
            quantizeVerticesTo = user_options.quantizeVerticesTo;
            quantizeNormalsTo = user_options.quantizeNormalsTo;

            // OTHER OPTIONS
            exportGameObjectName = user_options.exportGameObjectName;
            exportMaterialName = user_options.exportMaterialName;
            exportTexturesName = user_options.exportTexturesName;
            exportCameras = user_options.exportCameras;
            exportInactive = user_options.exportInactive;
            exportGameObjectsTag = user_options.exportGameObjectsTag;
            exportGameObjectsLayer = user_options.exportGameObjectsLayer;
            exportBatching = user_options.exportBatching;
            exportNavMesh = user_options.exportNavMesh;
            blockLeafNodesCreation = user_options.blockLeafNodesCreation;
            exportSubmeshesInExtra = user_options.exportSubmeshesInExtra;
            reduceGLTFChars = user_options.reduceGLTFChars;
            exportNormals = user_options.exportNormals;
            exportLightmapUVs = user_options.exportLightmapUVs;
            convertToGLB = user_options.convertToGLB;
            createUVOffsetExtras = user_options.createUVOffsetExtras;

            //SCENE EXTRAS
            exportSceneSkyboxEnvironment = user_options.exportSceneSkyboxEnvironment;
            exportSceneSkyboxBackground = user_options.exportSceneSkyboxBackground;
            createCubeForSkybox = user_options.createCubeForSkybox;
            exportSceneSkyboxBackgroundEvenDefault = user_options.exportSceneSkyboxBackgroundEvenDefault;
            exportSceneFog = user_options.exportSceneFog;
        }
        public void SetPrefsValues()
        {
            //TEXTURE QUALITY
            PlayerPrefs.SetFloat("gltf_o_colorTextureSaturation", colorTextureSaturation);
            PlayerPrefs.SetInt("gltf_o_exportSeparatedAlphaMap", GetIntFromBool(exportSeparatedAlphaMap));
            PlayerPrefs.SetInt("gltf_o_exportTextureType", (int)exportTextureType);
            PlayerPrefs.SetInt("gltf_o_fallbackGLTFTexture", GetIntFromBool(fallbackGLTFTexture));
            PlayerPrefs.SetInt("gltf_o_allImagesQuality", allImagesQuality);
            PlayerPrefs.SetInt("gltf_o_overrideLightmapQuality", overrideLightmapQuality);
            PlayerPrefs.SetInt("gltf_o_overrideNormalQuality",overrideNormalQuality);
            PlayerPrefs.SetInt("gltf_o_overrideMetallicSmoothnessQuality", overrideMetallicSmoothnessQuality);
            PlayerPrefs.SetInt("gltf_o_overrideCubemapQuality", overrideCubemapQuality);
            PlayerPrefs.SetInt("gltf_o_overrideDefaultQuality", overrideDefaultQuality);

            PlayerPrefs.SetInt("gltf_o_overrideDefaultExportTextureType", (int)overrideDefaultExportTextureType);
            PlayerPrefs.SetInt("gltf_o_overrideLightmapExportTextureType", (int)overrideLightmapExportTextureType);
            PlayerPrefs.SetInt("gltf_o_overrideMetallicSmoothnessExportTextureType", (int)overrideMetallicSmoothnessExportTextureType);
            PlayerPrefs.SetInt("gltf_o_overrideNormalExportTextureType", (int)overrideNormalExportTextureType);
            PlayerPrefs.SetInt("gltf_o_overrideCubemapTextureType", (int)overrideCubemapTextureType);

            //LIGHTMAP
            PlayerPrefs.SetFloat("gltf_o_quantizeMainUVsTo", maxLightmapClamp);
            PlayerPrefs.SetFloat("gltf_o_saturationLightmap", saturationLightmap);
            PlayerPrefs.SetFloat("gltf_o_lightmapIntensityMultiplier", lightmapIntensityMultiplier);
            PlayerPrefs.SetFloat("gltf_o_lightmapContrastCheat", lightmapContrastCheat);
            PlayerPrefs.SetInt("gltf_o_whiteImageForNonStaticLightmaps", GetIntFromBool(whiteImageForNonStaticLightmaps));

            //QUANTIZATION
            PlayerPrefs.SetInt("gltf_o_quantizeGLTF", GetIntFromBool(quantizeGLTF));
            PlayerPrefs.SetInt("gltf_o_quantizeMainUVsTo", (int)quantizeMainUVsTo);
            PlayerPrefs.SetInt("gltf_o_quantizeLightUVsTo", (int)quantizeLightUVsTo);
            PlayerPrefs.SetInt("gltf_o_quantizeVerticesTo", (int)quantizeVerticesTo);
            PlayerPrefs.SetInt("gltf_o_quantizeNormalsTo", (int)quantizeNormalsTo);

            // OTHER OPTIONS
            PlayerPrefs.SetInt("gltf_o_exportGameObjectName", GetIntFromBool(exportGameObjectName));
            PlayerPrefs.SetInt("gltf_o_exportMaterialName", GetIntFromBool(exportMaterialName));
            PlayerPrefs.SetInt("gltf_o_exportTexturesName", GetIntFromBool(exportTexturesName));
            PlayerPrefs.SetInt("gltf_o_exportCameras", GetIntFromBool(exportCameras));
            PlayerPrefs.SetInt("gltf_o_exportInactive", GetIntFromBool(exportInactive));
            PlayerPrefs.SetInt("gltf_o_exportGameObjectsTag", GetIntFromBool(exportGameObjectsTag));
            PlayerPrefs.SetInt("gltf_o_exportGameObjectsLayer", GetIntFromBool(exportGameObjectsLayer));
            PlayerPrefs.SetInt("gltf_o_exportBatching", GetIntFromBool(exportBatching));
            PlayerPrefs.SetInt("gltf_o_exportNavMesh", GetIntFromBool(exportNavMesh));
            PlayerPrefs.SetInt("gltf_o_exportSubmeshesInExtra", GetIntFromBool(exportSubmeshesInExtra));
            //export name here too
            PlayerPrefs.SetInt("gltf_o_reduceGLTFChars", GetIntFromBool(reduceGLTFChars));
            PlayerPrefs.SetInt("gltf_o_exportNormals", GetIntFromBool(exportNormals));
            PlayerPrefs.SetInt("gltf_o_exportLightmapUVs", GetIntFromBool(exportLightmapUVs));
            PlayerPrefs.SetInt("gltf_o_convertToGLB", GetIntFromBool(convertToGLB));
            PlayerPrefs.SetInt("gltf_o_createUVOffsetExtras", GetIntFromBool(createUVOffsetExtras));

            // ANIMATIONS - PENDING

            // SCENE EXTRAS
            PlayerPrefs.SetInt("gltf_o_exportSceneSkyboxEnvironment", GetIntFromBool(exportSceneSkyboxEnvironment));
            PlayerPrefs.SetInt("gltf_o_exportSceneSkyboxBackground", GetIntFromBool(exportSceneSkyboxBackground));
            PlayerPrefs.SetInt("gltf_o_createCubeForSkybox", GetIntFromBool(createCubeForSkybox));
            PlayerPrefs.SetInt("gltf_o_exportSceneSkyboxBackgroundEvenDefault", GetIntFromBool(exportSceneSkyboxBackgroundEvenDefault));
            PlayerPrefs.SetInt("gltf_o_exportSceneFog", GetIntFromBool(exportSceneFog));

            
            
        }
        public static ComponentType GetComponentType(ComponentTypeSelected compType, bool signed = false)
        {
            switch (compType)
            {
                case ComponentTypeSelected.BYTE:
                    if (signed)
                        return ComponentType.BYTE;
                    else
                        return ComponentType.UNSIGNED_BYTE;
                case ComponentTypeSelected.SHORT:
                    if (signed)
                        return ComponentType.SHORT;
                    else
                        return ComponentType.UNSIGNED_SHORT;
                default:
                    return ComponentType.FLOAT;
            }
        }

        private bool GetBoolFromInt(int val)
        {
            if (val == 0)
                return false;
            else
                return true;
        }
        private int GetIntFromBool(bool val)
        {
            if (val)
                return 1;
            else
                return 0;
        }

    }
}
