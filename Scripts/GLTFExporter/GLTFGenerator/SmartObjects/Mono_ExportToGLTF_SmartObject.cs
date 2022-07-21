using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF.SMART { 

    [ExecuteAlways]
    public class Mono_ExportToGLTF_SmartObject : MonoBehaviour
    {
        public SO_ExportGLTFOptions gltfCustomOptions;
        public string nameIdentifier;
        [HideInInspector]
        public SmartType smartType = SmartType.basic;
        [HideInInspector]
        public bool displayClassName = true;
        [HideInInspector]
        public string smartObjectClassName;
        [HideInInspector]
        public string modelId;
        [HideInInspector]
        public string exportLocation;
        [HideInInspector]
        public int curBuild;

        //scene extras
        [HideInInspector]
        public ReflectionProbe optionalReflectionProbe;
        [HideInInspector]
        public bool exportFog;

        [HideInInspector]
        public GameObject startPosition;
        [HideInInspector]
        public List<GameObject> startPositionList;

        private List<ObjectProperty> _smartObject;
        public virtual void ExportGLTF(string location, bool test_build)
        {

            string finalModelID = test_build ? modelId + "_" + curBuild.ToString() : modelId;

            SetupExtrasData();

            ExportGLTFOptions export_options = GetExportOptions();

            ExportToGLTF.CallExportGLTF(location, "gltf", transform, export_options, true, finalModelID, true);

            if (test_build) curBuild++;

            ClearExtrasComputedData();
        }
        public void SetupExtrasData()
        {
            ObjectMasterComputedExtrasMono _masterExtras = transform.gameObject.AddComponent<ObjectMasterComputedExtrasMono>();

            //if (smartObjectClassName != "") {
            _smartObject = new List<ObjectProperty>();
            _smartObject.Add(new ObjectProperty("name", nameIdentifier));
            if (smartObjectClassName != "") _smartObject.Add(new ObjectProperty("class", smartObjectClassName));
            _smartObject.Add(new ObjectProperty("smartType", smartType.ToString()));
            _smartObject.AddRange(SetupCustomExtrasData());
            _masterExtras.AddProperty(new ObjectProperty("smartObject", _smartObject));
            if (smartType == SmartType.space)
            {
                if (startPositionList != null)
                {
                    if (startPositionList.Count > 0)
                    {
                        Debug.Log("additnob");
                        startPositionList = GameObjectUtilities.RemoveNullObjects(startPositionList);
                        _smartObject.Add(new ObjectProperty("startPosition", startPositionList));
                    }
                }
            }
        }
        public virtual List<ObjectProperty> SetupCustomExtrasData()
        {
            return new List<ObjectProperty>();
        }
        public virtual void ClearExtrasComputedData()
        {
            ObjectNodeComputedExtrasMono[] computed_nodes = transform.GetComponentsInChildren<ObjectNodeComputedExtrasMono>(true);
            ObjectMasterComputedExtrasMono[] computed_master = transform.GetComponentsInChildren<ObjectMasterComputedExtrasMono>(true);
            for (int i = 0; i < computed_nodes.Length; i++)
                GameObject.DestroyImmediate(computed_nodes[i]);
            for (int i = 0; i < computed_master.Length; i++)
                GameObject.DestroyImmediate(computed_master[i]);
        }
        public virtual ExportGLTFOptions GetExportOptions()
        {
            ExportGLTFOptions options;
            if (gltfCustomOptions != null)
                options = new ExportGLTFOptions(gltfCustomOptions);
            else
            {
                options = new ExportGLTFOptions();

                // NODES - OPTIONS
                options.exportGameObjectName = true;
                options.exportMaterialName = false;
                options.exportTexturesName = false;
                options.exportInactive = true;
                options.exportGameObjectsTag = true;
                options.exportGameObjectsLayer = true;
                options.exportBatching = true;
                options.exportNavMesh = true;

                //MESH OPTIONS
                options.reduceGLTFChars = false;
                options.exportNormals = true;
                options.exportLightmapUVs = true;
                options.convertToGLB = true;    // pendig
                options.createUVOffsetExtras = false; //pending
                options.exportSubmeshesInExtra = false;

                //TEXTURES QUALITY
                options.colorTextureSaturation = 0.8f;
                options.exportSeparatedAlphaMap = true;
                options.exportTextureType = TextureExportType.WEBP;
                options.fallbackGLTFTexture = false;
                options.allImagesQuality = 75;
                options.overrideLightmapQuality = 90;
                options.overrideNormalQuality = 0;
                options.overrideMetallicSmoothnessQuality = 0;
                options.overrideDefaultQuality = 0;
                options.overrideCubemapQuality = 0;

                options.overrideLightmapExportTextureType = TextureExportType.DEFAULT;
                options.overrideMetallicSmoothnessExportTextureType = TextureExportType.WEBP;
                options.overrideNormalExportTextureType = TextureExportType.WEBP;
                options.overrideDefaultExportTextureType = TextureExportType.WEBP;
                options.overrideCubemapTextureType = TextureExportType.WEBP;

                //LIGHTMAP OPTIONS
                options.maxLightmapClamp = 5f;
                options.saturationLightmap = 1f;
                options.lightmapIntensityMultiplier = 2.5f;
                options.lightmapContrastCheat = 1f;
                options.whiteImageForNonStaticLightmaps = true;

                //QUANTIZATION
                options.quantizeGLTF = true;
                options.quantizeMainUVsTo = ComponentTypeSelected.SHORT;
                options.quantizeLightUVsTo = ComponentTypeSelected.SHORT;
                options.quantizeVerticesTo = ComponentTypeSelected.FLOAT;
                options.quantizeNormalsTo = ComponentTypeSelected.BYTE;

                

                options.overrideEnvironmentQuality = 0;
                options.extraExportMainCameraInGLTF = false;
            }

            options.exportCameras = false;
            // MUST BE IN BOTH CASES TO OVERRIDE VALUES FROM USER. SMART OBJECT SPACES REQUIRE ENVIRONEMNT DEFINITION
            if (smartType == SmartType.space)
            {
                options.exportSceneSkyboxBackground = true;
                options.exportSceneSkyboxEnvironment = true;
                //options.createCubeForSkybox = true;
                if (exportFog)
                    options.exportSceneFog = true;
                if (optionalReflectionProbe != null)
                    options.computedEnvironmentCubemap = optionalReflectionProbe.bakedTexture as Cubemap;
            }
            else
            {
                options.exportSceneSkyboxBackground = false;
                options.exportSceneSkyboxEnvironment = false;
                //options.createCubeForSkybox = false;
                options.exportSceneSkyboxBackgroundEvenDefault = false;
                options.exportSceneFog = false;
            }
            return options;
        }

        public void AddStartPositionToArray(GameObject obj)
        {
            if (startPositionList == null)
                startPositionList = new List<GameObject>();

            startPositionList.Insert(0,obj);
        }
    }
    
}

