using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor (typeof(SO_ExportGLTFOptions))]
    public class SO_ExportGLTFOptions_Editor : Editor
    {
        private enum SEL_MENU { textures, lightmap, options, quantization, scene }

        private SEL_MENU curMenu = SEL_MENU.options;
        SO_ExportGLTFOptions myScript;

        //bool edit = false;

        public void OnEnable()
        {
            myScript = (SO_ExportGLTFOptions)target;
            SetDefaults();
            //edit = false;
        }

        public override void OnInspectorGUI()
        {
            //GUILayout.Label("",GUILayout.Height(10f));
            //EditorGUILayout.BeginHorizontal();
            //{
            //    GUILayout.FlexibleSpace();
            //    {
            //        if (GUILayout.Button("Edit Properties", GUILayout.Width(160f), GUILayout.Height(40f)))
            //            edit = true;
            //    }
            //    GUILayout.FlexibleSpace();
            //}
            //EditorGUILayout.EndHorizontal();
            //GUILayout.Label("", GUILayout.Height(10f));
           
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Options", GUILayout.Height(30f), GUILayout.Height(30f)))
                curMenu = SEL_MENU.options;
            if (GUILayout.Button("Textures", GUILayout.Height(30f), GUILayout.Height(30f)))
                curMenu = SEL_MENU.textures;
            if (GUILayout.Button("Lightmaps", GUILayout.Height(30f), GUILayout.Height(30f)))
                curMenu = SEL_MENU.lightmap;
            if (GUILayout.Button("Quantization", GUILayout.Height(30f), GUILayout.Height(30f)))
                curMenu = SEL_MENU.quantization;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Scene", GUILayout.Height(30f), GUILayout.Height(30f)))
                curMenu = SEL_MENU.scene;
            EditorGUILayout.EndHorizontal();
            //if (!edit)
            //    GUI.enabled = false;
            switch (curMenu)
            {
                //==GENERAL OPTIONS==//
                case SEL_MENU.options:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    myScript.exportGameObjectName = EditorGUILayout.Toggle("Export GameObject Names", myScript.exportGameObjectName);
                    myScript.exportMaterialName = EditorGUILayout.Toggle("Export Material Names", myScript.exportMaterialName);
                    myScript.exportTexturesName = EditorGUILayout.Toggle("Export Texture Names", myScript.exportTexturesName);
                    myScript.exportCameras = EditorGUILayout.Toggle("Export Cameras", myScript.exportCameras);
                    myScript.exportInactive = EditorGUILayout.Toggle("Export Inactive GameObjects", myScript.exportInactive);
                    myScript.exportGameObjectsTag = EditorGUILayout.Toggle("Export Tag", myScript.exportGameObjectsTag);
                    myScript.exportGameObjectsLayer = EditorGUILayout.Toggle("Export Layer", myScript.exportGameObjectsLayer);
                    myScript.exportBatching = EditorGUILayout.Toggle("Export 'batching' flag", myScript.exportBatching);
                    myScript.exportNavMesh = EditorGUILayout.Toggle("Export navMesh", myScript.exportNavMesh);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    myScript.blockLeafNodesCreation = EditorGUILayout.Toggle("Block Leaf Nodes", myScript.blockLeafNodesCreation);
                    myScript.exportSubmeshesInExtra = EditorGUILayout.Toggle("Export Submeshes in extra", myScript.exportSubmeshesInExtra);
                    myScript.reduceGLTFChars = EditorGUILayout.Toggle("Reduce GLTF Code", myScript.reduceGLTFChars);
                    myScript.exportNormals = EditorGUILayout.Toggle("Export Mesh Normals", myScript.exportNormals);

                    GUI.enabled = false;
                    myScript.exportLightmapUVs = EditorGUILayout.Toggle("Export Lightmap UVs", myScript.exportLightmapUVs);
                    myScript.convertToGLB = EditorGUILayout.Toggle("Convert to GLB", myScript.convertToGLB);
                    myScript.createUVOffsetExtras = EditorGUILayout.Toggle("Save Offset Scale in Extras", myScript.createUVOffsetExtras);
                    //myScript.exportLightmapUVs = true;
                    //myScript.convertToGLB = false;
                    //myScript.createUVOffsetExtras = false;
                    //if (edit)
                    GUI.enabled = true;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    break;
                    //==TEXTURES QUALITY==//
                case SEL_MENU.textures:
                    
                    //myScript.colorTextureSaturation = EditorGUILayout.Slider("Color Texture Saturation", myScript.colorTextureSaturation, 0f, 2f);
                    myScript.colorTextureSaturation = EditorGUILayout.FloatField("Color Texture Saturation", myScript.colorTextureSaturation);
                    myScript.metallicMultiplier = EditorGUILayout.Slider("Metallic Max Multiplier", myScript.metallicMultiplier, 0f, 1f);
                    myScript.smoothnessMultiplier = EditorGUILayout.Slider("Smoothness Max Multiplier", myScript.smoothnessMultiplier, 0f, 1f);
                    myScript.exportSeparatedAlphaMap = EditorGUILayout.Toggle("Export alfa in extra map", myScript.exportSeparatedAlphaMap);

                    EditorGUILayout.LabelField("Quality");
                    myScript.allImagesQuality = EditorGUILayout.IntSlider("All Images Quality", myScript.allImagesQuality, 1, 100);
                    EditorGUILayout.LabelField("Quality Overrides");
                    myScript.overrideLightmapQuality = EditorGUILayout.IntSlider("Lightmap Override", myScript.overrideLightmapQuality, 0, 100);
                    myScript.overrideNormalQuality = EditorGUILayout.IntSlider("Normal Override", myScript.overrideNormalQuality, 0, 100);
                    myScript.overrideMetallicSmoothnessQuality = EditorGUILayout.IntSlider("Metallic Smoothness Override", myScript.overrideMetallicSmoothnessQuality, 0, 100);
                    myScript.overrideCubemapQuality = EditorGUILayout.IntSlider("Cubemap Override", myScript.overrideCubemapQuality, 0, 100);
                    myScript.overrideDefaultQuality = EditorGUILayout.IntSlider("Default Override", myScript.overrideDefaultQuality, 0, 100);

                    EditorGUILayout.LabelField("Divide Factor");
                    myScript.allImagesDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Divide Texture Size: ", myScript.allImagesDivideFactor);
                    if (myScript.allImagesDivideFactor == TextureDivideFactor.NONE_SET)
                        myScript.allImagesDivideFactor = TextureDivideFactor.FULL;
                    EditorGUILayout.LabelField("Divide Factor Overrides");
                    myScript.overrideLightmapDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Lightmap Override: ", myScript.overrideLightmapDivideFactor);
                    myScript.overrideNormalDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Normal Override: ", myScript.overrideNormalDivideFactor);
                    myScript.overrideMetallicSmoothnessDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Metallic Smoothness Override: ", myScript.overrideMetallicSmoothnessDivideFactor);
                    myScript.overrideCubemapDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Cubemap Override: ", myScript.overrideCubemapDivideFactor);
                    myScript.overrideDefaultDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Default Override ", myScript.overrideDefaultDivideFactor);

                    EditorGUILayout.LabelField("Export type of texture");
                    myScript.exportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Export Texture Type: ", myScript.exportTextureType);
                    if (myScript.exportTextureType == TextureExportType.NONE_SET)
                        myScript.exportTextureType = TextureExportType.DEFAULT;

                    if (myScript.exportTextureType == TextureExportType.DEFAULT)
                        GUI.enabled = false;
                    myScript.fallbackGLTFTexture = EditorGUILayout.Toggle("Fallback to jpeg", myScript.fallbackGLTFTexture);
                    EditorGUILayout.LabelField("Export Overrides: (suggested DEFAULT in lightmap)");
                    myScript.overrideNormalExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Normal Type: ", myScript.overrideNormalExportTextureType);
                    myScript.overrideDefaultExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Default Type: ", myScript.overrideDefaultExportTextureType);
                    myScript.overrideMetallicSmoothnessExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Smnoothness Type: ", myScript.overrideMetallicSmoothnessExportTextureType);
                    myScript.overrideLightmapExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Lightmap Type: ", myScript.overrideLightmapExportTextureType);
                    myScript.overrideCubemapTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Cubemap Type: ", myScript.overrideCubemapTextureType);
                    //if (edit)
                    GUI.enabled = true;
                    break;
                //==LIGHTMAPS==//
                case SEL_MENU.lightmap:
                    myScript.maxLightmapClamp = EditorGUILayout.FloatField("Max Clamp", myScript.maxLightmapClamp);
                    myScript.saturationLightmap = EditorGUILayout.FloatField("Saturation Compensation", myScript.saturationLightmap);
                    myScript.lightmapIntensityMultiplier = EditorGUILayout.FloatField("Intensity Multiplier*", myScript.lightmapIntensityMultiplier);
                    myScript.lightmapContrastCheat = EditorGUILayout.FloatField("lightmap Contrast Cheat*", myScript.lightmapContrastCheat);
                    myScript.whiteImageForNonStaticLightmaps = EditorGUILayout.Toggle("White Image on non static", myScript.whiteImageForNonStaticLightmaps);
                    break;


                //==QUANTIZATION==//
                case SEL_MENU.quantization:
                    if (myScript.blockLeafNodesCreation == true) { 
                        GUI.enabled = false;
                        GUILayout.Label("\"Block Leaf Nodes Creation\" is set to true");
                        GUILayout.Label("Quantization will always be false");
                    }

                    myScript.quantizeGLTF = EditorGUILayout.Toggle("Quantize values", myScript.quantizeGLTF);
                    if (!myScript.quantizeGLTF)
                        GUI.enabled = false;
                    myScript.quantizeMainUVsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Main UVs: ", myScript.quantizeMainUVsTo);
                    myScript.quantizeLightUVsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Lightmap UVs: ", myScript.quantizeLightUVsTo);
                    myScript.quantizeVerticesTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Vertices: ", myScript.quantizeVerticesTo);
                    myScript.quantizeNormalsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Normals: ", myScript.quantizeNormalsTo);
                    //if (edit)
                    GUI.enabled = true;
                    break;
                case SEL_MENU.scene:
                    myScript.exportSceneSkyboxEnvironment = EditorGUILayout.Toggle("Export Skybox Environment", myScript.exportSceneSkyboxEnvironment);
                    myScript.exportSceneSkyboxBackground = EditorGUILayout.Toggle("Export Skybox Background", myScript.exportSceneSkyboxBackground);
                    myScript.exportSceneSkyboxBackgroundEvenDefault = EditorGUILayout.Toggle("Consider Default Skybox", myScript.exportSceneSkyboxBackgroundEvenDefault);
                    myScript.createCubeForSkybox = EditorGUILayout.Toggle("Create Cubemap Mesh", myScript.createCubeForSkybox);
                    myScript.exportSceneFog = EditorGUILayout.Toggle("Export Fog if active", myScript.exportSceneFog);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(myScript);
                Debug.Log("changed");
            }

            ////ANIMATIONS
            //public bool exportAnimations = true;

            ////MESH OPTIONS
            //public bool reduceGLTFChars;
            //public bool exportNormals;
            //public bool exportLightmapUVs;              // NOT WORKING YET, RIGHT NOW IT WILL ALWAYS EXPORT LIGHTMAP UVS
            //public bool convertToGLB;                   // NOT WORKING YET.
            //public bool createUVOffsetExtras;           // NOT WORKING YET: IF SET TO TRUE, FINAL SIZE WILL BE REDUCED, UVS WILL BE SAVED ONCE, AND OFFSET/SCALE WILL BE SAVED IN GLTF "EXTRAS" CODE FOR THREE JS GLTF IMPORTER TO USE
            //public bool exportSubmeshesInExtra;         // SUBMESHES ARE EXPORTED SEPARATED IN CLASSIC GLTF, SHOULD BE EXPORTED COMBINED AND SAVE GROUPED POSITIONS

            ////TEXTURES QUALITY
            //public float colorTextureSaturation;
            //public bool exportSeparatedAlphaMap;
            //public TextureExportType exportTextureType;
            //public bool fallbackGLTFTexture;            // NEW 7/22/2021
            //public int allImagesQuality;
            //public int overrideLightmapQuality;         // SET 0 TO NOT OVERRIDE
            //public int overrideNormalQuality;
            //public int overrideMetallicSmoothnessQuality;
            //public int overrideCubemapQuality;
            //public int overrideDefaultQuality;
            //public TextureExportType overrideLightmapExportTextureType = TextureExportType.DEFAULT;
            //public TextureExportType overrideNormalExportTextureType = TextureExportType.NONE_SET;
            //public TextureExportType overrideMetallicSmoothnessExportTextureType = TextureExportType.NONE_SET;
            //public TextureExportType overrideDefaultExportTextureType = TextureExportType.NONE_SET;
            //public TextureExportType overrideCubemapTextureType = TextureExportType.NONE_SET;

            ////LIGHTMAP OPTIONS
            //public float maxLightmapClamp;
            //public float saturationLightmap;
            //public bool whiteImageForNonStaticLightmaps;
            //public float lightmapIntensityMultiplier;
            //public float lightmapContrastCheat;

            ////QUANTIZATION
            //public bool quantizeGLTF;
            //public ComponentTypeSelected quantizeMainUVsTo;
            //public ComponentTypeSelected quantizeLightUVsTo;
            //public ComponentTypeSelected quantizeVerticesTo;
            //public ComponentTypeSelected quantizeNormalsTo;

            //// SCENE
            //public bool exportSceneSkyboxEnvironment;
            //public bool exportSceneSkyboxBackground;
            //public bool createCubeForSkybox = true; //
            //public GameObject cubeSkybox = null;
            //public int cubeSkyboxScale = 1; //1 = 1000
            //public bool exportSceneSkyboxBackgroundEvenDefault;
            //public bool exportSceneFog;
            //public Cubemap computedEnvironmentCubemap;
            //public Cubemap computedBackgroundCubemap;

            //public int overrideEnvironmentQuality = 0;

            ////SET MAIN CAMERA 
            //public bool extraExportMainCameraInGLTF = false;
            //public int extraCameraIndex = -1;
            EditorUtility.SetDirty(myScript);

        }

        private void SetDefaults()
        {
            Debug.Log(myScript.overrideLightmapDivideFactor);
            //myScript.overrideLightmapDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Lightmap Override: ", myScript.overrideLightmapDivideFactor);
            //myScript.overrideNormalDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Normal Override: ", myScript.overrideNormalDivideFactor);
            //myScript.overrideMetallicSmoothnessDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Metallic Smoothness Override: ", myScript.overrideMetallicSmoothnessDivideFactor);
            //myScript.overrideCubemapDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Cubemap Override: ", myScript.overrideCubemapDivideFactor);
            //myScript.overrideDefaultDivideFactor = (TextureDivideFactor)EditorGUILayout.EnumPopup("Default Override ", myScript.overrideDefaultDivideFactor);
        }
    }
}
