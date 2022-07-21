using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WEBGL_EXPORTER.GLTF;
public class Mono_ExportToGLTF : MonoBehaviour
{
    public SO_ExportGLTFOptions gltfCustomOptions;
    public string gltfName = "gltf";

    //public bool exportForNFT = false;

    public string modelId;
    public string exportLocation;
    public int curBuild = 0;

    public Transform targetParent;
    public Transform startPosition;

    public ReflectionProbe optionalEnvironmentReflections;
    public virtual void ExportGLTF(string location, bool test_build = false) {
        if (targetParent == null)
            targetParent = transform;

        string finalModelID = test_build ? modelId + "_" + curBuild.ToString() : modelId;

        SetupExtrasComputedData();

        ExportGLTFOptions export_options;
        if (gltfCustomOptions != null)
        {
            export_options = new ExportGLTFOptions(gltfCustomOptions);
        }
        else
        {
            export_options = new ExportGLTFOptions();
        }

        if (optionalEnvironmentReflections != null)
            export_options.computedEnvironmentCubemap = optionalEnvironmentReflections.bakedTexture as Cubemap;
        ExportToGLTF.CallExportGLTF(location, gltfName, targetParent, export_options, true, finalModelID, true);

        if (test_build) curBuild++;

        ClearExtrasComputedData();

    }
    private void SetupExtrasComputedData()
    {
        ObjectMasterComputedExtrasMono _masterExtras = targetParent.gameObject.AddComponent<ObjectMasterComputedExtrasMono>();
        if (startPosition != null)
            _masterExtras.AddProperty(new ObjectProperty("startPosition",startPosition.gameObject));
    }
    private void ClearExtrasComputedData()
    {
        ObjectMasterComputedExtrasMono [] computedMaster = targetParent.GetComponentsInChildren<ObjectMasterComputedExtrasMono>();
        for (int i =0; i < computedMaster.Length; i++)
            DestroyImmediate(computedMaster[i]);

        ObjectNodeComputedExtrasMono[] computedNodes = targetParent.GetComponentsInChildren <ObjectNodeComputedExtrasMono> ();
        for (int i = 0; i < computedNodes.Length; i++)
            DestroyImmediate(computedNodes[i]);
    }
    //private ExportGLTFOptions getNFTGLTFOptions()
    //{
    //    ExportGLTFOptions options = new ExportGLTFOptions();
    //    options.exportLocation = exportLocation;

    //    // NODES - OPTIONS
    //    options.exportGameObjectName = true;
    //    options.exportMaterialName = true;
    //    options.exportTexturesName = false;
    //    options.exportCameras = false;
    //    options.exportInactive = true;
    //    options.exportGameObjectsTag = true;
    //    options.exportGameObjectsLayer = true;
    //    options.exportBatching = true;
    //    options.exportNavMesh = true;

    //    //MESH OPTIONS
    //    options.reduceGLTFChars = false;
    //    options.exportNormals = true;
    //    options.exportLightmapUVs = true;
    //    options.convertToGLB = true;    // pendig
    //    options.createUVOffsetExtras = false; //pending
    //    options.exportSubmeshesInExtra = true;

    //    //TEXTURES QUALITY
    //    options.colorTextureSaturation = 0.75f;
    //    options.exportSeparatedAlphaMap = true;
    //    options.exportTextureType = TextureExportType.WEBP;
    //    options.fallbackGLTFTexture = false;
    //    options.allImagesQuality = 75;
    //    options.overrideLightmapQuality = 90;
    //    options.overrideNormalQuality = 0;
    //    options.overrideMetallicSmoothnessQuality = 0;
    //    options.overrideDefaultQuality = 0;
    //    options.overrideCubemapQuality = 0;
    //    options.overrideEnvironmentQuality = 0;

    //    options.overrideLightmapExportTextureType = TextureExportType.DEFAULT;
    //    options.overrideMetallicSmoothnessExportTextureType = TextureExportType.WEBP;
    //    options.overrideNormalExportTextureType = TextureExportType.WEBP;
    //    options.overrideDefaultExportTextureType = TextureExportType.WEBP;
    //    options.overrideCubemapTextureType = TextureExportType.WEBP;

    //    //LIGHTMAP OPTIONS
    //    options.maxLightmapClamp = 5f;
    //    options.saturationLightmap = 1f;
    //    options.lightmapIntensityMultiplier = 1f;
    //    options.lightmapContrastCheat = 1f;
    //    options.whiteImageForNonStaticLightmaps = true;

    //    //QUANTIZATION
    //    options.quantizeGLTF = true;
    //    options.quantizeMainUVsTo = ComponentTypeSelected.SHORT;
    //    options.quantizeLightUVsTo = ComponentTypeSelected.SHORT;
    //    options.quantizeVerticesTo = ComponentTypeSelected.FLOAT;
    //    options.quantizeNormalsTo = ComponentTypeSelected.BYTE;

    //    // SCENE
    //    options.exportSceneSkyboxBackground = true;
    //    options.exportSceneSkyboxEnvironment = true;
    //    options.createCubeForSkybox = true;
    //    options.exportSceneSkyboxBackgroundEvenDefault = false;
    //    options.exportSceneFog = true;

        

    //    options.extraExportMainCameraInGLTF = true;
    //    return options;
    //}
}
