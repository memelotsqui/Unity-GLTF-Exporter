using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectScene
    {
        public static List<int> nodesList;
        public static List<ObjectProperty> extraProperties;
        public static List<ObjectProperty> properties;

        private static Cubemap environment;
        private static Cubemap background;

        private static GameObject cubeSkybox;
        public static void Reset()
        {
            nodesList = new List<int>();
            extraProperties = new List<ObjectProperty>();
            properties = new List<ObjectProperty>();
            environment = null;
            background = null;
        }

        public static void AddNodeToScene(int node)
        {
            nodesList.Add(node);
        }

        public static void SetupScene()
        {
            List<ObjectProperty> _scenes = new List<ObjectProperty>();



            List<ObjectProperty> _scene = new List<ObjectProperty>();
            _scene.Add(new ObjectProperty("nodes", nodesList));
            ObjectExtraProperties _extra = AddExtraDataToScene();
            if (_extra != null)
                _scene.Add(new ObjectProperty(_extra));

            _scenes.Add(new ObjectProperty("",_scene));

            properties.Add(new ObjectProperty("scenes", _scenes, true));

        }
        public static ObjectExtraProperties AddExtraDataToScene()
        {
            //FOG
            if (RenderSettings.fog)
            {
                List<ObjectProperty> _fog = new List<ObjectProperty>();
                _fog.Add(new ObjectProperty("color", RenderSettings.fogColor));
                _fog.Add(new ObjectProperty("mode", RenderSettings.fogMode.ToString()));
                _fog.Add(new ObjectProperty("near", RenderSettings.fogStartDistance));
                _fog.Add(new ObjectProperty("far", RenderSettings.fogEndDistance));
                extraProperties.Add(new ObjectProperty("fog", _fog));
            }

            // BACKGROUND
            Cubemap scene_background = null;
            if (ExportToGLTF.options.computedBackgroundCubemap != null)    // if this was set, it is more iomportant, so override it
                background = ExportToGLTF.options.computedBackgroundCubemap;
            else
            {
                if (ExportToGLTF.options.exportSceneSkyboxBackground)
                {
                    if (RenderSettings.skybox != null)
                    {
                        if (AssetDatabase.GetAssetPath(RenderSettings.skybox) != "Resources/unity_builtin_extra" || ExportToGLTF.options.exportSceneSkyboxBackgroundEvenDefault)
                        {
                            scene_background = SphericalPanoramaToCubemapConverter.GetCubemapFromSceneSkybox();
                            background = scene_background;
                            cubeSkybox = ExportToGLTF.options.cubeSkybox;
                        }
                    }
                }
            }

            //ENVIRONMENT
            if (ExportToGLTF.options.computedEnvironmentCubemap != null)    // if this was set, it is more iomportant, so override it
                environment = ExportToGLTF.options.computedEnvironmentCubemap;
            else
            {
                if (ExportToGLTF.options.exportSceneSkyboxEnvironment)
                {
                    if (RenderSettings.defaultReflectionMode == UnityEngine.Rendering.DefaultReflectionMode.Skybox  && scene_background != null)
                    {
                        environment = scene_background;
                    }
                    if (RenderSettings.defaultReflectionMode == UnityEngine.Rendering.DefaultReflectionMode.Custom)
                    {
                        environment = RenderSettings.customReflection;
                    }
                }

            }



            


            if (environment != null) { 
                int _environment = ObjectExtrasCubeTextures.GetCubemapIndex(environment);
                if (_environment != -1)
                    extraProperties.Add(new ObjectProperty("environment",_environment));
            }
            
            //BACKGROUND    
            if (background != null)
            {
                List<ObjectProperty> _background = new List<ObjectProperty>();

                
                int _backgroundCubeTexture = ObjectExtrasCubeTextures.GetCubemapIndex(background);
                if (_backgroundCubeTexture != -1)
                {
                    List<ObjectProperty> _cubeTexture = new List<ObjectProperty>();
                    _cubeTexture.Add(new ObjectProperty("index", _backgroundCubeTexture));
                    if (cubeSkybox != null)
                    {
                        _cubeTexture.Add(new ObjectProperty("node", ExportToGLTF.options.cubeSkybox));
                        _cubeTexture.Add(new ObjectProperty("scale", ExportToGLTF.options.cubeSkyboxScale));
                        
                    }
                    _background.Add(new ObjectProperty("cubeTexture", _cubeTexture));

                   
                }

                if (_background.Count > 0)
                {
                    extraProperties.Add(new ObjectProperty("background", _background));
                }
            }
            //BACKGROUND CUSTOM SKYBOX
            
            //SAVE ALL
            if (extraProperties.Count > 0)
            {
                return new ObjectExtraProperties(extraProperties);
            }
            return null;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            result += "\"scene\": 0,\n";
            for (int i = 0; i < properties.Count; i++)
            {
                result += properties[i].GetPropertyGLTF();
            }
            if (add_end_comma)
                result += ",\n";
           
            return result;
        }
    }
}
