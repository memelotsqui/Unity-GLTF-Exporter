using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtrasCubeTextures
    {
        //consts
        const string exportPreName = "cbm_";
        const bool exportPropertyName = false;

        //statics
        public static int globalIndex = -1;
        public static List<ObjectExtrasCubeTextures> allUniqueCubemaps;
        public static int environmentCubemap = -1;

        //vars
        public int index = -1;

        public Cubemap cubemap;
        public CubemapExtra cubemapExtra;
        public List<ObjectProperty> cubemapProperties;
        public int quality = 0;

        string exportName;
        public static void Reset()
        {
            globalIndex = 0;
            allUniqueCubemaps = new List<ObjectExtrasCubeTextures>();
        }

        public static int GetCubemapIndex(Cubemap _cubemap, int _quality = 0)
        {
            if (_cubemap == null)
                return -1;
            if (allUniqueCubemaps == null)
            {
                allUniqueCubemaps = new List<ObjectExtrasCubeTextures>();

                allUniqueCubemaps.Add(new ObjectExtrasCubeTextures(_cubemap,_quality));

                return allUniqueCubemaps.Count - 1;
            }
            else
            {
                foreach (ObjectExtrasCubeTextures oc in allUniqueCubemaps)
                {
                    if (oc.isSameCubemap(_cubemap))
                    {
                        return oc.index;
                    }
                }
                // IF NO EXISTING ANIMATION CLIP WAS FOUND CREATE A NEW ANIMATION CLIP

                allUniqueCubemaps.Add(new ObjectExtrasCubeTextures(_cubemap,_quality));
                return allUniqueCubemaps.Count - 1;
            }
        }

        private bool isSameCubemap(Cubemap _cubemap)
        {
            if (_cubemap == cubemap)
                return true;
            return false;
        }

        public ObjectExtrasCubeTextures(Cubemap _cubemap, int _quality = 0)
        {
            cubemap = _cubemap;
            quality = _quality;
            if (quality > 100) quality = 100;
            if (quality < 0) quality = 0;
            index = globalIndex;
            exportName = exportPreName + globalIndex;
            cubemapExtra = new CubemapExtra(_cubemap, exportName);
            cubemapProperties = GetTextureProperties(_cubemap, cubemapExtra, _quality);
            index = globalIndex;
            globalIndex++;
        }
        public List<ObjectProperty> GetTextureProperties(Cubemap _cubemap, CubemapExtra _cubemapExtra, int _quality = 0)
        {
            bool _exportFallback = ExportToGLTF.options.SaveFallback(TextureType.CUBEMAP);

            List<ObjectProperty> _texture_properties = new List<ObjectProperty>();
            string _name = exportName;
            int _sampler = ObjectSampler.GeSamplerIndex(_cubemap);
            int[] _source = new int[6];
            Texture2D[] _cubemapTextures = _cubemapExtra.GetCubemapTextures();

            for (int i =0; i < _cubemapTextures.Length;i++)
            {
                _source[i] = ObjectImage.GetImageIndex(_cubemapTextures[i], TextureType.CUBEMAP, false, false, false, _exportFallback, _quality);
            }
            if (exportPropertyName) _texture_properties.Add(new ObjectProperty("name", _name));
            _texture_properties.Add(new ObjectProperty("sampler", _sampler));

            TextureExportType _export_type = ExportToGLTF.options.GetExportTextureType(TextureType.CUBEMAP);

            // WEBP OR NON WEBP SECTION
            if (_export_type == TextureExportType.DEFAULT || _exportFallback)    // IF FALLBACK SAVE HERE SOURCE TOO
            {
                _texture_properties.Add(new ObjectProperty("source", _source));
            }
            if (_export_type != TextureExportType.DEFAULT)
            {
                int[] _source_default = new int[6];
                for (int i = 0; i < _source.Length; i++)
                    _source_default[i] = _source[i];

                if (ExportToGLTF.options.fallbackGLTFTexture)
                {           
                    for (int i =0; i < _source.Length; i++)
                    {
                        _source_default[i]+=1;                              // IF WE HAVE FALLBACK ON TOP, WE MUST ADD 6 TO THE SOURCE, SINCE THE ORIGINAL IMAGES COMES AFTER THE FALLBACK IMAGES (SOURCE)
                    }
                }
                bool _requiredExtensions = _exportFallback ? false:true;    // IF WE HAVE FALLBACK, WE DONT NECESSARY REQUIRE THE EXTENSION

                ObjectProperty _extensions = new ObjectProperty();          // CREATE AN EMPTY OBJECT THAT WILL HOLD THE EXTENSION
                if (_export_type == TextureExportType.WEBP)
                    _extensions.AddExtensionObject(CreateEXT_texture_webp(_source_default, _requiredExtensions));
                if (_export_type == TextureExportType.KTX2)
                    _extensions.AddExtensionObject(CreateKHR_texture_basisu(_source_default, _requiredExtensions));
                _texture_properties.Add(_extensions);
            }

            return _texture_properties;


        }

        public ObjectExtension CreateEXT_texture_webp(int[] source, bool is_required)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (source.Length > 0)
            {
                _properties.Add(new ObjectProperty("source", source));
                ObjectExtension ext_texture_webp = new ObjectExtension("EXT_texture_webp", _properties, is_required);
                return ext_texture_webp;
            }
            return null;
        }

        public ObjectExtension CreateKHR_texture_basisu(int[] source, bool is_required)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (source.Length > 0)
            {
                _properties.Add(new ObjectProperty("source", source));
                ObjectExtension ext_texture_webp = new ObjectExtension("KHR_texture_basisu", _properties, is_required);
                return ext_texture_webp;
            }
            return null;
        }

        public static void AddGLTFDataToExtras()
        {
            //if (ExportToGLTF.options.extraExportEnviromentCubemapInGLTF)
                //GetEnviromentData(ExportToGLTF.options.computedEnvironmentCubemap);

            if (allUniqueCubemaps.Count > 0)
            {
                List<ObjectProperty> _cubemaps = new List<ObjectProperty>();

                foreach (ObjectExtrasCubeTextures oc in allUniqueCubemaps)
                {
                    _cubemaps.Add(new ObjectProperty("", oc.cubemapProperties));
                }
                ObjectMasterExtras.Add(new ObjectProperty("cubeTextures", _cubemaps, true));
            }
        }
        //public static void GetEnviromentData(Cubemap _cubemap)
        //{
        //    environmentCubemap = GetCubemapIndex(_cubemap);
        //    if (environmentCubemap != -1)
        //    {
        //        ObjectMasterExtras.Add(new ObjectProperty("environment", environmentCubemap));
        //    }
        //}

        //public static void GetEnviromentData(Cubemap cubemap_extra, string export_location)
        //{
        //    if (cubemap_extra == null)
        //        Debug.LogWarning("NULL enviroment cubemap, a default texture will be created");
        //    if (cubemap_extra == null)
        //        cubemap_extra = new CubemapExtra(new Color(0.2f, 0.2f, 0.2f), "env");

        //    Texture2D[] refProbeTextures = cubemap_extra.GetCubemapTextures();
        //    string[] textureNames = new string[6];
        //    for (int i = 0; i < refProbeTextures.Length; i++)
        //    {
        //        FileExporter.ExportToJPEG(refProbeTextures[i], cubemap_extra.name + i, export_location, 75);
        //        textureNames[i] = cubep_extra.name + i + ".jpg";
        //    }
        //    properties.Add(new ObjectProperty("environment", textureNames));
        //}
    }
}
