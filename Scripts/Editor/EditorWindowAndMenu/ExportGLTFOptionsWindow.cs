using UnityEditor;
using UnityEngine;
namespace WEBGL_EXPORTER.GLTF
{
    public class ExportGLTFOptionsWindow : EditorWindow
    {
        private enum SEL_MENU { textures, lightmap,options,quantization,scene }
        private enum SEL_COMP_TYPE { BYTE = 5121, SHORT = 5123, FLOAT = 5126 }
        //private enum SEL_SIGNED_COMP_TYPE { BYTE = 5120, SHORT = 5122, FLOAT = 5126 }
        private SEL_MENU curMenu = SEL_MENU.options;
        const float width = 400f;
        const float height = 520f;
        const float margin = 20f;

        public float finalWidth;
        public float optionsButtonsSize;

        public ExportGLTFOptions opt;

        public static ExportGLTFOptionsWindow CreateInstance()
        {
            ExportGLTFOptionsWindow o = CreateInstance<ExportGLTFOptionsWindow>();

            o.titleContent = new GUIContent("GLTF options");

            o.minSize = new Vector2(width, height);
            o.maxSize = o.minSize;

            var position = o.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            o.position = position;
            o.Show();

            o.finalWidth = width - (margin * 2);
            o.optionsButtonsSize = ((width - (margin * 2)) / 4)-1f;
            o.opt = new ExportGLTFOptions();
            o.opt.GetPrefsValues();

            return o;
        }

        
        private void OnGUI()
        {
            if (opt == null)
                this.Close();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginArea(new Rect(new Vector2(margin-2f, 20f), new Vector2(finalWidth+10f, 60f)));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Options", GUILayout.Height(30f), GUILayout.Width(optionsButtonsSize)))
                curMenu = SEL_MENU.options;
            if (GUILayout.Button("Textures", GUILayout.Height(30f), GUILayout.Width(optionsButtonsSize)))
                curMenu = SEL_MENU.textures;
            if (GUILayout.Button("Lightmaps", GUILayout.Height(30f), GUILayout.Width(optionsButtonsSize)))
                curMenu = SEL_MENU.lightmap;
            if (GUILayout.Button("Quantization", GUILayout.Height(30f), GUILayout.Width(optionsButtonsSize)))
                curMenu = SEL_MENU.quantization;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Scene", GUILayout.Height(30f), GUILayout.Width(optionsButtonsSize)))
                curMenu = SEL_MENU.scene;
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(new Vector2(margin-2f, 100f), new Vector2(finalWidth, 420f)));
            
            switch (curMenu)
            {
                //==GENERAL OPTIONS==//
                case SEL_MENU.options:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    opt.exportGameObjectName = EditorGUILayout.Toggle("Export GameObject Names", opt.exportGameObjectName);
                    opt.exportMaterialName = EditorGUILayout.Toggle("Export Material Names", opt.exportMaterialName);
                    opt.exportTexturesName = EditorGUILayout.Toggle("Export Texture Names", opt.exportTexturesName);
                    opt.exportCameras = EditorGUILayout.Toggle("Export Cameras", opt.exportCameras);
                    opt.exportInactive = EditorGUILayout.Toggle("Export Inactive GameObjects", opt.exportInactive);
                    opt.exportGameObjectsTag = EditorGUILayout.Toggle("Export Tag", opt.exportGameObjectsTag);
                    opt.exportGameObjectsLayer = EditorGUILayout.Toggle("Export Layer", opt.exportGameObjectsLayer);
                    opt.exportBatching = EditorGUILayout.Toggle("Export 'batching' flag", opt.exportBatching);
                    opt.exportNavMesh = EditorGUILayout.Toggle("Export navMesh", opt.exportNavMesh);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    opt.exportSubmeshesInExtra = EditorGUILayout.Toggle("Export Submeshes in extra", opt.exportSubmeshesInExtra);
                    opt.reduceGLTFChars = EditorGUILayout.Toggle("Reduce GLTF Code", opt.reduceGLTFChars);
                    opt.exportNormals = EditorGUILayout.Toggle("Export Mesh Normals", opt.exportNormals);
                    
                    GUI.enabled = false;
                    opt.exportLightmapUVs = EditorGUILayout.Toggle("Export Lightmap UVs", opt.exportLightmapUVs);
                    opt.convertToGLB = EditorGUILayout.Toggle("Convert to GLB", opt.convertToGLB);
                    opt.createUVOffsetExtras = EditorGUILayout.Toggle("Save Offset Scale in Extras", opt.createUVOffsetExtras);
                    GUI.enabled = true;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    break;
                //==TEXTURES QUALITY==//
                case SEL_MENU.textures:
                    opt.colorTextureSaturation = EditorGUILayout.Slider("Color Texture Saturation", opt.colorTextureSaturation, 0f, 1f);
                    opt.exportSeparatedAlphaMap = EditorGUILayout.Toggle("Export alfa in extra map", opt.exportSeparatedAlphaMap);
                    
                    opt.allImagesQuality = EditorGUILayout.IntSlider("All Images Quality", opt.allImagesQuality, 1, 100);
                    EditorGUILayout.LabelField("Quality Overrides");
                    opt.overrideLightmapQuality = EditorGUILayout.IntSlider("Lightmap Quality", opt.overrideLightmapQuality, 0, 100);
                    opt.overrideNormalQuality = EditorGUILayout.IntSlider("Normal Quality", opt.overrideNormalQuality, 0, 100);
                    opt.overrideMetallicSmoothnessQuality = EditorGUILayout.IntSlider("Metallic Smoothness Quality", opt.overrideMetallicSmoothnessQuality, 0, 100);
                    opt.overrideCubemapQuality = EditorGUILayout.IntSlider("Cubemap Quality", opt.overrideCubemapQuality, 0, 100);
                    opt.overrideDefaultQuality = EditorGUILayout.IntSlider("Default Quality", opt.overrideDefaultQuality, 0, 100);

                    opt.exportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Export Texture Type: ", opt.exportTextureType);    

                    if (opt.exportTextureType == TextureExportType.DEFAULT)
                        GUI.enabled = false;
                    opt.fallbackGLTFTexture = EditorGUILayout.Toggle("Fallback to jpeg", opt.fallbackGLTFTexture);
                    EditorGUILayout.LabelField("Export Overrides: (suggested DEFAULT in lightmap)");
                    opt.overrideNormalExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Normal Type: ", opt.overrideNormalExportTextureType);
                    opt.overrideDefaultExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Default Type: ", opt.overrideDefaultExportTextureType);
                    opt.overrideMetallicSmoothnessExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Smnoothness Type: ", opt.overrideMetallicSmoothnessExportTextureType);
                    opt.overrideLightmapExportTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Lightmap Type: ", opt.overrideLightmapExportTextureType);
                    opt.overrideCubemapTextureType = (TextureExportType)EditorGUILayout.EnumPopup("Cubemap Type: ", opt.overrideCubemapTextureType);
                    GUI.enabled = true;
                    break;
                //==LIGHTMAPS==//
                case SEL_MENU.lightmap:
                    opt.maxLightmapClamp = EditorGUILayout.FloatField("Max Clamp", opt.maxLightmapClamp);
                    opt.saturationLightmap = EditorGUILayout.FloatField("Saturation Compensation", opt.saturationLightmap);
                    opt.lightmapIntensityMultiplier = EditorGUILayout.FloatField("Intensity Multiplier*", opt.lightmapIntensityMultiplier);
                    opt.lightmapContrastCheat = EditorGUILayout.FloatField("lightmap Contrast Cheat*", opt.lightmapContrastCheat);
                    opt.whiteImageForNonStaticLightmaps = EditorGUILayout.Toggle("White Image on non static", opt.whiteImageForNonStaticLightmaps);
                    break;
                

                //==QUANTIZATION==//
                case SEL_MENU.quantization:
                    opt.quantizeGLTF = EditorGUILayout.Toggle("Quantize values", opt.quantizeGLTF);
                    if (!opt.quantizeGLTF)
                        GUI.enabled = false;
                    opt.quantizeMainUVsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Main UVs: ", opt.quantizeMainUVsTo);
                    opt.quantizeLightUVsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Lightmap UVs: ", opt.quantizeLightUVsTo);
                    opt.quantizeVerticesTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Vertices: ", opt.quantizeVerticesTo);
                    opt.quantizeNormalsTo = (ComponentTypeSelected)EditorGUILayout.EnumPopup("Normals: ", opt.quantizeNormalsTo);
                    GUI.enabled = true;
                    break;
                case SEL_MENU.scene:
                    opt.exportSceneSkyboxEnvironment = EditorGUILayout.Toggle("Export Skybox Environment", opt.exportSceneSkyboxEnvironment);
                    opt.exportSceneSkyboxBackground = EditorGUILayout.Toggle("Export Skybox Background", opt.exportSceneSkyboxBackground);
                    opt.exportSceneSkyboxBackgroundEvenDefault = EditorGUILayout.Toggle("Consider Default Skybox", opt.exportSceneSkyboxBackgroundEvenDefault);
                    opt.createCubeForSkybox = EditorGUILayout.Toggle("Create Cubemap Mesh", opt.createCubeForSkybox);
                    opt.exportSceneFog = EditorGUILayout.Toggle("Export Fog if active", opt.exportSceneFog);
                    break;
            }
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(new Vector2(margin - 2f, 460f), new Vector2(finalWidth, 40f)));
            if (GUILayout.Button("SAVE CHANGES", GUILayout.Height(30f)))
            {
                opt.SetPrefsValues();
                Debug.Log("CHANGES SUCCESFULLY SAVED");
                this.Close();
            }
            GUILayout.EndArea();
            if (EditorGUI.EndChangeCheck())
            {
                if (opt.exportTextureType == TextureExportType.NONE_SET)
                    opt.exportTextureType = TextureExportType.DEFAULT;
            }
        }
    }
}
