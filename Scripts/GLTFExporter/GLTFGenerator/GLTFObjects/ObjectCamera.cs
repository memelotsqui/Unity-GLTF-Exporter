using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectCamera
    {

        //statics
        public static int globalIndex = -1;
        public static List<ObjectCamera> allUniqueCameras;

        //vars
        public int index = -1;

        public List<ObjectProperty> cameraProperties;

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueCameras = new List<ObjectCamera>();
        }
        /// <summary>
        /// Get Index of Perspective Camera
        /// </summary>
        /// <param name="aspect_ratio"></param>
        /// <param name="y_fov"></param>
        /// <param name="z_far"></param>
        /// <param name="z_near"></param>
        /// <returns></returns>
        public static int GetCameraIndex(float aspect_ratio, float y_fov, float z_far, float z_near, string _tag = "", int layer_mask = -1)
        {
            ObjectCamera object_camera = new ObjectCamera(aspect_ratio,y_fov,z_far,z_near,_tag,layer_mask);
            allUniqueCameras.Add(object_camera);
            return object_camera.index;
        }
        /// <summary>
        /// Get Index of Ortographic Camera
        /// </summary>
        /// <param name="mag_scale"></param>
        /// <param name="z_far"></param>
        /// <param name="z_near"></param>
        /// <returns></returns>
        public static int GetCameraIndex(Vector2 mag_scale, float z_far, float z_near, string _tag = "", int layer_mask = -1)
        {
            ObjectCamera object_camera = new ObjectCamera(mag_scale, z_far, z_near,_tag, layer_mask);
            allUniqueCameras.Add(object_camera);
            return object_camera.index;
        }
        // Perspective Camera
        public ObjectCamera(float aspect_ratio, float y_fov, float z_far, float z_near,string _tag = "", int layer_mask = -1)
        {
            cameraProperties = new List<ObjectProperty>();
            cameraProperties.Add(new ObjectProperty("type", "perspective"));
            List<ObjectProperty> _perspProperties = new List<ObjectProperty>();
            _perspProperties.Add(new ObjectProperty("aspectRatio",aspect_ratio));
            _perspProperties.Add(new ObjectProperty("yfov", y_fov));
            _perspProperties.Add(new ObjectProperty("zfar", z_far));
            _perspProperties.Add(new ObjectProperty("znear", z_near));
            cameraProperties.Add(new ObjectProperty("perspective",_perspProperties));
            ObjectExtraProperties _extras = new ObjectExtraProperties();
            if (layer_mask != -1)
                _extras.Add(new ObjectProperty("layerMask",layer_mask));
            if (_tag != "")
                _extras.Add(new ObjectProperty("tag",_tag));
            if (_extras.extrasProperties.Count > 0)
                cameraProperties.Add(new ObjectProperty(_extras));

            index = globalIndex;
            if (_tag == "MainCamera")
                if (ExportToGLTF.options.extraExportMainCameraInGLTF)
                    if (ExportToGLTF.options.extraCameraIndex == -1)
                        ExportToGLTF.options.extraCameraIndex = index;
            globalIndex++;
        }
        // Ortographic Camera
        public ObjectCamera(Vector2 mag_scale, float z_far, float z_near,string _tag = "", int layer_mask = -1)
        {
            cameraProperties = new List<ObjectProperty>();
            cameraProperties.Add(new ObjectProperty("type", "ortographic"));
            List<ObjectProperty> _perspProperties = new List<ObjectProperty>();
            _perspProperties.Add(new ObjectProperty("xmag", mag_scale.x));
            _perspProperties.Add(new ObjectProperty("ymag", mag_scale.y));
            _perspProperties.Add(new ObjectProperty("zfar", z_far));
            _perspProperties.Add(new ObjectProperty("znear", z_near));
            cameraProperties.Add(new ObjectProperty("ortographic", _perspProperties));

            ObjectExtraProperties _extras = new ObjectExtraProperties();
            if (layer_mask != -1)
                _extras.Add(new ObjectProperty("layerMask", layer_mask));
            if (_tag != "")
                _extras.Add(new ObjectProperty("tag", _tag));
            if (_extras.extrasProperties.Count > 0)
                cameraProperties.Add(new ObjectProperty(_extras));

            index = globalIndex;
            if (_tag == "MainCamera")
                if (ExportToGLTF.options.extraExportMainCameraInGLTF)
                    if (ExportToGLTF.options.extraCameraIndex == -1)
                        ExportToGLTF.options.extraCameraIndex = index;
            globalIndex++;
        }
        
        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueCameras.Count > 0)
            {
                result += "\"cameras\" : [\n";
                for (int i = 0; i < allUniqueCameras.Count; i++)
                {
                    result += ObjectProperty.GetObjectProperties(allUniqueCameras[i].cameraProperties);
                    if (i < allUniqueCameras.Count - 1)
                        result += ",\n";
                }
                result += "]";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }
    }
}
