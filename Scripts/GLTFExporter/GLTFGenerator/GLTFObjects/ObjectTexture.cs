using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectTexture
    {
        //consts
        public string exportPreName = "txr_";
        public bool exportPropertyName = false;
        
        //statics
        public static int globalIndex = -1;
        public static List<ObjectTexture> allUniqueTextures;

        //vars
        public int index = -1;
        public TextureType textureType;

        public List<ObjectProperty> textureProperties;
        public ObjectImage objectImage;
        string exportName;

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueTextures = new List<ObjectTexture>();
        }
        public static int GetTextureIndex(Texture2D texture_2D, TextureType texture_type, bool metal_smooth_map, bool uses_alpha, bool save_alpha_map = false,int lightmap_index = -1, float _smoothnessMult = 1, float _metallicMult = 1)
        {
            if (texture_2D == null)
                return -1;
            if (allUniqueTextures == null)
            {
                allUniqueTextures = new List<ObjectTexture>();
                allUniqueTextures.Add(new ObjectTexture(texture_2D, texture_type, metal_smooth_map, uses_alpha,false,lightmap_index, _smoothnessMult, _metallicMult)); 
                if (uses_alpha && save_alpha_map)
                {
                    allUniqueTextures.Add(new ObjectTexture(texture_2D, texture_type, metal_smooth_map, uses_alpha, true,lightmap_index, _smoothnessMult, _metallicMult));     // SAVE THE SAME TEXTURE TWICE, THIS WAY WE SAVE THE SPOT FOR THE ALPHA VALUE
                    return allUniqueTextures.Count - 2;                                                         // AND RETURN THE POSITION OF THE FIRST ONE
                }
                return allUniqueTextures.Count - 1;
                

            }
            else
            {
                foreach (ObjectTexture ot in allUniqueTextures)
                {
                    if (ot.IsSameTexture(texture_2D, metal_smooth_map))
                    {
                        return ot.index;
                    }
                }
                allUniqueTextures.Add(new ObjectTexture(texture_2D, texture_type, metal_smooth_map, uses_alpha,false,lightmap_index, _smoothnessMult, _metallicMult));
                if (uses_alpha && save_alpha_map)
                {
                    allUniqueTextures.Add(new ObjectTexture(texture_2D,texture_type, metal_smooth_map, uses_alpha, true,lightmap_index, _smoothnessMult, _metallicMult));     // SAVE THE SAME TEXTURE TWICE, THIS WAY WE SAVE THE SPOT FOR THE ALPHA VALUE
                    return allUniqueTextures.Count - 2;                                                     // AND RETURN THE POSITION OF THE FIRST ONE
                }
                return allUniqueTextures.Count - 1;

            }
        }
        private bool IsSameTexture(Texture2D texture_2D, bool metal_smooth_map)
        {
            if (texture_2D == objectImage.image && metal_smooth_map == objectImage.metalSmoothMap)
            {
                return true;
            }
            return false;
        }
        public ObjectTexture(Texture2D texture_2D, TextureType texture_type, bool metal_smooth_map, bool uses_alpha, bool is_alpha = false, int lightmap_index = -1, float _smoothnessMult = 1, float _metallicMult = 1)
        {
            index = globalIndex;
            exportName = exportPreName + globalIndex;
            textureType = texture_type;
            textureProperties = GetTextureProperties(texture_2D, metal_smooth_map, uses_alpha, is_alpha, lightmap_index, _smoothnessMult, _metallicMult);
            globalIndex++;
        }
        public List<ObjectProperty> GetTextureProperties(Texture2D texture_2D, bool metal_smooth_map, bool uses_alpha, bool is_alpha = false, int lightmap_index = -1, float _smoothnessMult=1, float _metallicMult = 1)
        {
            //bool _exportFallback = ExportToGLTF.options.fallbackGLTFTexture;
            bool _exportFallback = ExportToGLTF.options.SaveFallback(textureType);

            List<ObjectProperty> _texture_properties = new List<ObjectProperty>();
            string _name = exportName;
            int _sampler = ObjectSampler.GeSamplerIndex(texture_2D);
            int _source = ObjectImage.GetImageIndex(texture_2D, textureType, metal_smooth_map, uses_alpha, is_alpha, _exportFallback,0, _smoothnessMult, _metallicMult);


            if (exportPropertyName) _texture_properties.Add(new ObjectProperty("name",_name));
            _texture_properties.Add(new ObjectProperty("sampler", _sampler));

            TextureExportType _export_type = ExportToGLTF.options.GetExportTextureType(textureType);

            // WEBP OR NON WEBP SECTION
            if (_export_type == TextureExportType.DEFAULT || _exportFallback)    // IF FALLBACK SAVE HERE SOURCE TOO
            {
                _texture_properties.Add(new ObjectProperty("source", _source));
            }
            if (_export_type != TextureExportType.DEFAULT)
            {
                if (ExportToGLTF.options.fallbackGLTFTexture)           // IF WE HAVE FALLBACK ON TOP, WE MUST ADD 1 TO THE SOURCE, SINCE THE ORIGINAL IMAGE COMES AFTER THE FALLBACK IMAGE (SOURCE)
                    _source += 1;

                bool _requiredExtensions = true;
                if (_exportFallback)                // IF WE HAVE FALLBACK, WE DONT NECESSARY REQUIRE THE EXTENSION
                    _requiredExtensions = false;

                ObjectProperty _extensions = new ObjectProperty();  // CREATE AN EMPTY OBJECT THAT WILL HOLD THE EXTENSION
                if (_export_type == TextureExportType.WEBP)
                    _extensions.AddExtensionObject(CreateEXT_texture_webp(_source, _requiredExtensions));
                if (_export_type == TextureExportType.KTX2)
                    _extensions.AddExtensionObject(CreateKHR_texture_basisu(_source,_requiredExtensions));
                _texture_properties.Add(_extensions);  
            }
            if (textureType == TextureType.LIGHTMAP)
            {
                ObjectMasterExtras.AddLightmapTexture(lightmap_index, index);
            }

            // save the objectimage for comparison
            objectImage = ObjectImage.allUniqueImages[_source];

            return _texture_properties;


        }
        private ObjectExtension CreateKHR_texture_basisu(int source, bool is_required)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (source >= 0)
            {
                _properties.Add(new ObjectProperty("source", source));
                ObjectExtension ext_texture_webp = new ObjectExtension("KHR_texture_basisu", _properties, is_required);
                return ext_texture_webp;
            }
            return null;
        }
        private ObjectExtension CreateEXT_texture_webp(int source, bool is_required)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (source >= 0)
            {
                _properties.Add(new ObjectProperty("source", source));
                ObjectExtension ext_texture_webp = new ObjectExtension("EXT_texture_webp", _properties, is_required);
                return ext_texture_webp;
            }
            return null;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueTextures.Count > 0)
            {
                result += "\"textures\" : [\n";
                for (int i = 0; i < allUniqueTextures.Count; i++)
                {
                    result += ObjectProperty.GetObjectProperties(allUniqueTextures[i].textureProperties);
                    if (i < allUniqueTextures.Count - 1)
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
