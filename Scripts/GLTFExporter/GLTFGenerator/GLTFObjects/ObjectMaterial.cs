using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectMaterial
    {
        //consts
        //public const string exportPreName = "mat_";

        //statics
        public static int globalIndex = -1;
        public static List<ObjectMaterial> allUniqueMaterials;
        public static Texture2D lightmapWhite;

        //vars
        public int index = -1;

        public Material material;
        public int lightmapIndex = -1;  //necessary to compare values with other materials
        public List<ObjectProperty> materialProperties;
        public List<ObjectExtension> materialExtensions;

        public string exportName;

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueMaterials = new List<ObjectMaterial>();
            lightmapWhite = null;
        }
        public static int GetMaterialIndex(Material tarMaterial, Renderer meshRenderer)
        {
            if (tarMaterial == null)
                return -1;

            if (allUniqueMaterials == null)
            {
                allUniqueMaterials = new List<ObjectMaterial>();

                int lightIndex = -1;
                if (meshRenderer != null)
                    lightIndex = meshRenderer.lightmapIndex == 65534 ? -1 : meshRenderer.lightmapIndex;

                allUniqueMaterials.Add(new ObjectMaterial(tarMaterial, lightIndex));

                return allUniqueMaterials.Count - 1;
            }
            else
            {
                foreach (ObjectMaterial om in allUniqueMaterials)
                {
                    if (om.IsSameMaterial(tarMaterial, meshRenderer))
                    {
                        return om.index;
                    }
                }       // IF NO EXISTING MATERIAL WAS FOUND CREATE A NEW OBJECT MATERIAL

                int lightIndex = -1;
                if (meshRenderer != null) 
                    lightIndex = meshRenderer.lightmapIndex == 65534 ? -1 : meshRenderer.lightmapIndex;

                allUniqueMaterials.Add(new ObjectMaterial(tarMaterial, lightIndex));
                return allUniqueMaterials.Count - 1;
            }
        }
        private bool IsSameMaterial(Material tarMaterial, Renderer meshRenderer)
        {
            int lightIndex = -1;
            if (meshRenderer != null) lightIndex = meshRenderer.lightmapIndex == 65534 ? -1 : meshRenderer.lightmapIndex;

            if (tarMaterial == material)
            {
                if (lightIndex == lightmapIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public ObjectMaterial(Material mat, int lightmap_index)
        {
            // REMEMBER IT ALSO SAVES NULL VALUE, FOR NULL VALUE WE WILL HAVE TO CREATE A DEFAULT MATERIAL IN EXPORT
            material = mat;
            lightmapIndex = lightmap_index;
            index = globalIndex;
            exportName = mat.name;
            //exportName = exportPreName + globalIndex;
            materialExtensions = new List<ObjectExtension>();
            materialProperties = GetMaterialProperties(mat,lightmap_index);
            globalIndex++;
        }
        
        private List <ObjectProperty> GetMaterialProperties(Material tarMat, int lightmap_index)
        {
            List<ObjectProperty> material_Properties = new List<ObjectProperty>();
            string _name = exportName;
            //name
            if (ExportToGLTF.options.exportMaterialName)
                material_Properties.Add(new ObjectProperty("name", _name));

            
            
            // MATERIAL MAY BE NULL, BUT IT ALSO MAY HAVE A LIGHTMAP
            if (tarMat != null) 
            {
                switch (tarMat.shader.name)
                {
                    case "Standard":
                        material_Properties.AddRange(PropertiesStandardMaterial.GetMaterialProperties(tarMat));
                        break;
                    case "Universal Render Pipeline/Lit":
                        material_Properties.AddRange(PropertiesUniversalRenderPipeline.GetMaterialProperties(tarMat));
                        break;
                }

            }

            //extras
            //commonConstant (lightmap)
            if (lightmap_index != -1 || ExportToGLTF.options.whiteImageForNonStaticLightmaps)
            {
                Texture2D TXRLightmapTexture = null;
                if (lightmap_index != -1)
                {
                    //Debug.Log(lightmap_index);
                    TXRLightmapTexture = LightmapSettings.lightmaps[lightmap_index].lightmapColor;
                }
                else
                {
                    if (ExportToGLTF.options.whiteImageForNonStaticLightmaps)
                        TXRLightmapTexture = GetLightmapWhite();
                }

                bool moz_lightmap = true;
               
                if (TXRLightmapTexture != null)
                {

                    int _lightmapTexture_index = GetIntFromTexture(TXRLightmapTexture, TextureType.LIGHTMAP, false, false, false, lightmap_index);
                    int _lightmapTexture_texCoord = 1;
                    float _lightmapIntensity = ImageUtilities.GetImageMaxPixelFloatValue(TXRLightmapTexture, ExportToGLTF.options.maxLightmapClamp);
                    _lightmapIntensity *= ExportToGLTF.options.lightmapIntensityMultiplier;

                    if (!moz_lightmap)
                    {
                        List<ObjectProperty> _lightmapTexture = new List<ObjectProperty>();
                        _lightmapTexture.Add(new ObjectProperty("index", _lightmapTexture_index));
                        _lightmapTexture.Add(new ObjectProperty("texCoord", _lightmapTexture_texCoord));


                        ObjectExtraProperties _extra = new ObjectExtraProperties();
                        _extra.Add(new ObjectProperty("lightmapTexture", _lightmapTexture));
                        
                        if (_lightmapIntensity != 1)
                            _extra.Add(new ObjectProperty("lightmapIntensity", _lightmapIntensity));
                            material_Properties.Add(new ObjectProperty(_extra));
                        }
                    else
                    {
                        ObjectExtension moz_lightamp_ext = CreateMOZ_Lightmap(_lightmapTexture_index, _lightmapTexture_texCoord, _lightmapIntensity);
                        if (moz_lightamp_ext != null)
                            materialExtensions.Add(moz_lightamp_ext);
                    }
                }
                
                //_commonConstant.Add(new ObjectProperty(""));
            }


            return material_Properties;
        }

        public static ObjectExtension CreateKHR_texture_transform(Vector2 scale_v2, Vector2 offset_v2)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (offset_v2 != Vector2.zero)
                _properties.Add(new ObjectProperty("offset", offset_v2));
            if (scale_v2 != Vector2.one)
                _properties.Add(new ObjectProperty("scale", scale_v2));

            if (_properties.Count > 0)
            {
                ObjectExtension khr_texture_transform = new ObjectExtension("KHR_texture_transform", _properties, true);
                return khr_texture_transform;
            }
            return null;
        }

        public static ObjectExtension CreateMOZ_Lightmap(int imageIndex, int texCoord, float intensity)
        {
            if (imageIndex == -1)
                return null;
            List<ObjectProperty> _properties = new List<ObjectProperty>();
                _properties.Add(new ObjectProperty("index", imageIndex));
            _properties.Add(new ObjectProperty("texCoord", texCoord));

            if (intensity != 1)
                _properties.Add(new ObjectProperty("intensity", intensity));


            if (_properties.Count > 0)
            {
                ObjectExtension moz_lightmap = new ObjectExtension("MOZ_lightmap", _properties,false);
                return moz_lightmap;
            }
            return null;
        }
        private Texture2D GetLightmapWhite()
        {
            if (lightmapWhite == null)
                lightmapWhite = ImageGenerator.CreateSimpleTexture(new Color(0.75f, 0.75f, 0.75f));
            return lightmapWhite;
        }

        public static int GetIntFromTexture(Texture2D texture_2D, TextureType texture_type, bool metal_smooth_map = false, bool uses_alpha = false, bool save_alpha_map = false, int lightmap_index = -1, float _smoothnessMult = 1, float _metallicMult = 1)
        {
            if (texture_2D != null)
                return ObjectTexture.GetTextureIndex(texture_2D, texture_type, metal_smooth_map, uses_alpha, save_alpha_map, lightmap_index, _smoothnessMult,_metallicMult);
            else
                return -1;  // NO TEXTURE FOUND
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "\"materials\" : [\n";
            for (int i = 0; i < allUniqueMaterials.Count; i++)
            {
                result += ObjectProperty.GetObjectProperties(allUniqueMaterials[i].materialProperties, allUniqueMaterials[i].materialExtensions);
                if (i < allUniqueMaterials.Count - 1)
                    result += ",\n";
            }
            result += "]";
            if (add_end_comma)
                result += ",\n";
            return result;
        }
    }
}
