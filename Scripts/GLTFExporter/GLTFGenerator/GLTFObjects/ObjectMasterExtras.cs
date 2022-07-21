using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectMasterExtras
    {
        //public static ObjectExtraProperties extra_properties;
        public static List<ObjectProperty> properties;
        public static List<ObjectProperty> lightmapTextures;
        public static void Reset()
        {
            //extra_properties = new ObjectExtraProperties();
            properties = new List<ObjectProperty>();
            lightmapTextures = new List<ObjectProperty>();
        }
        public static void Add(ObjectProperty object_property) 
        {
            properties.Add(object_property);
        }
        public static void AddList(List<ObjectProperty> object_porperties)
        {
            properties.AddRange(object_porperties);
        }
        public static string GetGLTFData(bool add_end_comma = true)
        {
            GetGenericExtras();
            string result = "";
            if (properties.Count > 0) 
            {
                result += "\"extras\":";
                result += ObjectProperty.GetObjectProperties(properties);
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
            //return ObjectProperty.GetObjectProperties(new ObjectProperty(extra_properties)) + _comma;
        }
        public static void GetGenericExtras()
        {
            GetLightmapData();

            if (ExportToGLTF.options.extraExportMainCameraInGLTF)
            {
                if (ExportToGLTF.options.exportCameras)
                {
                    if (ExportToGLTF.options.extraCameraIndex == -1)
                    {
                        Debug.LogWarning("NO CAMERA WITH MAIN CAMERA TAG FOUND, WILL BE USING FIRST IN CAMERAS ARRAY");
                        ExportToGLTF.options.extraCameraIndex = 0;
                    }
                    GetMainCameraData(ExportToGLTF.options.extraCameraIndex);
                }
            }
        }
        
        public static void GetMainCameraData(int camera_index)
        {
            properties.Add(new ObjectProperty("mainCamera",camera_index));
        }
        public static void AddLightmapTexture(int lightmap_index, int lightmap_texture )
        {
            List<ObjectProperty> lightmaps_properties = new List<ObjectProperty>();
            lightmaps_properties.Add(new ObjectProperty("lightmapIndex", lightmap_index));
            lightmaps_properties.Add(new ObjectProperty("textureIndex", lightmap_texture));
            lightmapTextures.Add(new ObjectProperty("", lightmaps_properties));
        }

        public static void GetLightmapData()
        {
            if (lightmapTextures.Count > 0)
            {
                properties.Add(new ObjectProperty("lightmapTextures", lightmapTextures,true));
            }
        }

    }
}
