using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class PropertiesUniversalRenderPipeline
    {
        public static List<ObjectProperty> GetMaterialProperties(Material _material)
        {
            List<ObjectProperty> material_Properties = new List<ObjectProperty>();

            //extensions

            // MATERIAL MAY BE NULL, BUT IT ALSO MAY HAVE A LIGHTMAP
            if (_material != null)
            {

                ObjectExtension _khr_texture_transform_ext = null;

                //GET ALPHA NOW AND SAVE IT LATER (SOME PROPERTIES ARE REQUIRE TO KNOW IF IT USES ALPHA, BUT IN ORDER ALPHA IN SCHEMA IS LATER)
                #region GET ALPHA
                //    //alphamode -- added first to know if material requires transparency in main texture
                string _alphaMode = "OPAQUE";

                if (_material.GetFloat("_Surface") == 1)
                {
                    // CUTOUT = 1
                    _alphaMode = "BLEND";
                }
                else
                {
                    if (_material.GetFloat("_AlphaClip") == 1)
                    {
                        _alphaMode = "MASK";
                    }
                }
                bool _usesAlpha = _alphaMode != "OPAQUE" ? true : false;

                #endregion



                #region PBR METALLIC ROUGHNESS SECTION
                List<ObjectProperty> _pbrMetallicRoughness = new List<ObjectProperty>();

                Texture2D TXRBaseColorTexture = _material.GetTexture("_MainTex") as Texture2D;
                if (TXRBaseColorTexture == null) TXRBaseColorTexture = _material.mainTexture as Texture2D;   // SOMETIMES _MainTex RETURNS NULL, BUT .mainTexture returns the texture, happens only in instantiated materials, to solve it, were double checking

                Vector2 _t_scale = _material.GetTextureScale("_MainTex");
                Vector2 _t_offset = _material.GetTextureOffset("_MainTex");
                _t_offset = new Vector2(_t_offset.x, _t_offset.y + (1f - _t_scale.y));
                _khr_texture_transform_ext = ObjectMaterial.CreateKHR_texture_transform(_t_scale, _t_offset);
                //  WILL RETURN NULL IF OFFSET IS VECTOR2.ZERO AND SCALE IS VECTOR2.ONE 

                // SAVE COLOR? ONLY IF ITS WHITE, ALSO SAVE IT AS LINEAR
                Color _baseColorFactor = _material.GetColor("_BaseColor");
                if (_baseColorFactor != Color.white) _pbrMetallicRoughness.Add(new ObjectProperty("baseColorFactor", _baseColorFactor.linear));

                //SAVE ALPHA MAP?
                bool export_separated_alpha = ExportToGLTF.options.ExportSeparatedAlphaMap();

                //METALLIC - ROUGHNESS (1-SMOOTHNESS)
                float _smoothness = _material.GetFloat("_Smoothness");

                float _metallicMultiplier = ExportToGLTF.options.metallicMultiplier;
                float _smoothnessMultiplier = ExportToGLTF.options.smoothnessMultiplier;
                float _metallicFactor = _material.GetFloat("_Metallic") * _metallicMultiplier;
                float _roughnessFactor = 1f - (_smoothness * _smoothnessMultiplier);
                int _metallicRoughnessTexture_index = ObjectMaterial.GetIntFromTexture(_material.GetTexture("_MetallicGlossMap") as Texture2D, TextureType.METALLIC_SMOOTHNESS, true, false, false, -1, _smoothness, 1);

                int _baseColorTexture_index = ObjectMaterial.GetIntFromTexture(TXRBaseColorTexture, TextureType.DEFAULT, false, _usesAlpha, export_separated_alpha);

                //IF TEXTURE EXISTS, ADD IT AS A TEXTURE INFO
                if (_baseColorTexture_index != -1)
                {
                    List<ObjectProperty> _baseColorTextureProperties = new List<ObjectProperty>();
                    _baseColorTextureProperties.Add(new ObjectProperty("index", _baseColorTexture_index));
                    //MOVED BELOW CHECK AND DELETE
                    //ObjectProperty _baseColorTexture = new ObjectProperty("baseColorTexture", _baseColorTextureProperties);
                    //_baseColorTexture.AddExtensionObject(_khr_texture_transform_ext);                           //EXTENSION KHR TRANSFORM
                    if (export_separated_alpha && _usesAlpha)
                    {
                        ObjectExtraProperties _extra = new ObjectExtraProperties();
                        List<ObjectProperty> _alphaMapTexture = new List<ObjectProperty>();
                        _alphaMapTexture.Add(new ObjectProperty("index", _baseColorTexture_index + 1));
                        _extra.Add(new ObjectProperty("alphaMapTexture", _alphaMapTexture));
                        _baseColorTextureProperties.Add(new ObjectProperty(_extra));
                    }
                    ObjectProperty _baseColorTexture = new ObjectProperty("baseColorTexture", _baseColorTextureProperties);
                    _baseColorTexture.AddExtensionObject(_khr_texture_transform_ext);                           //EXTENSION KHR TRANSFORM
                    _pbrMetallicRoughness.Add(_baseColorTexture);

                }

                if (_metallicFactor != 1f && _metallicRoughnessTexture_index == -1) _pbrMetallicRoughness.Add(new ObjectProperty("metallicFactor", _metallicFactor));
                //PENDING TO CHECK: IN UNITY YOU CAN MODIFY THE SMOOTHNESS FACTOR WHILE STILL HAVING A TEXTURE, CHECK IF IT CAN HAPPEN SAME IN THREE JS
                if (_roughnessFactor != 1f && _metallicRoughnessTexture_index == -1) _pbrMetallicRoughness.Add(new ObjectProperty("roughnessFactor", _roughnessFactor));

                //IF METALLIC SMOOTHNESS TEXTURE EXISTS, ADD IT AS A TEXTURE INFO
                if (_metallicRoughnessTexture_index != -1)
                {
                    //float _mult = _standardMaterial.GetFloat("_GlossMapScale");

                    List<ObjectProperty> _metallicRoughnessTextureProperties = new List<ObjectProperty>();
                    _metallicRoughnessTextureProperties.Add(new ObjectProperty("index", _metallicRoughnessTexture_index));
                    ObjectProperty _metallicRoughnessTexture = new ObjectProperty("metallicRoughnessTexture", _metallicRoughnessTextureProperties);
                    // MUST BE DONE THIS WAY, EVEN IF ITS GLOSSINESS ITS CONDIERED AS "MULTIPLIED", SO GLOSSINESS AND ROUGNESS INTENSITY ARE THE SAME WHEN AN IMAGE IS PROVIDED
                    //if (_mult != 1f)
                    //_pbrMetallicRoughness.Add(new ObjectProperty("roughnessFactor", _mult));

                    _metallicRoughnessTexture.AddExtensionObject(_khr_texture_transform_ext);                   //EXTENSION KHR TRANSFORM

                    _pbrMetallicRoughness.Add(_metallicRoughnessTexture);
                }

                if (_pbrMetallicRoughness.Count > 0)
                    material_Properties.Add(new ObjectProperty("pbrMetallicRoughness", _pbrMetallicRoughness));
                #endregion

                #region NORMAL MAP SECTION
                Texture2D TXRNormalTexture = _material.GetTexture("_BumpMap") as Texture2D;
                if (TXRNormalTexture != null)
                {
                    List<ObjectProperty> _normalTextureProperties = new List<ObjectProperty>();

                    int _normalTexture_index = ObjectMaterial.GetIntFromTexture(TXRNormalTexture, TextureType.NORMAL);
                    float _normalTexture_scale = _material.GetFloat("_BumpScale");

                    _normalTextureProperties.Add(new ObjectProperty("index", _normalTexture_index));
                    if (_normalTexture_scale != 1f) _normalTextureProperties.Add(new ObjectProperty("scale", _normalTexture_scale));
                    ObjectProperty _normalTexture = new ObjectProperty("normalTexture", _normalTextureProperties);
                    _normalTexture.AddExtensionObject(_khr_texture_transform_ext);                                  //EXTENSION KHR TRANSFORM

                    material_Properties.Add(_normalTexture);
                }
                #endregion

                #region OCLUSION MAP SECTION
                Texture2D TXROcclusionTexture = _material.GetTexture("_OcclusionMap") as Texture2D;
                if (TXROcclusionTexture != null)
                {
                    List<ObjectProperty> _occlusionTextureProperties = new List<ObjectProperty>();

                    int _occlusionTexture_index = ObjectMaterial.GetIntFromTexture(TXROcclusionTexture, TextureType.DEFAULT);
                    float _occlusionTexture_strength = _material.GetFloat("_OcclusionStrength");

                    _occlusionTextureProperties.Add(new ObjectProperty("index", _occlusionTexture_index));
                    if (_occlusionTexture_strength != 1f) _occlusionTextureProperties.Add(new ObjectProperty("strength", _occlusionTexture_strength));
                    ObjectProperty _occlusionTexture = new ObjectProperty("occlusionTexture", _occlusionTextureProperties);

                    //EXTENSION KHR TRANSFORM
                    _occlusionTexture.AddExtensionObject(_khr_texture_transform_ext);

                    material_Properties.Add(_occlusionTexture);
                }
                #endregion

                #region EMISSION MAP AND FACTOR
                if (_material.IsKeywordEnabled("_EMISSION"))
                {
                    //emission texture
                    Texture2D TXREmissiveTexture = _material.GetTexture("_EmissionMap") as Texture2D;
                    if (TXREmissiveTexture != null)
                    {
                        List<ObjectProperty> _emissiveTextureProperties = new List<ObjectProperty>();

                        int _emissiveTexture_index = ObjectMaterial.GetIntFromTexture(TXREmissiveTexture, TextureType.DEFAULT);

                        _emissiveTextureProperties.Add(new ObjectProperty("index", _emissiveTexture_index));
                        ObjectProperty _emissiveTexture = new ObjectProperty("emissiveTexture", _emissiveTextureProperties);
                        _emissiveTexture.AddExtensionObject(_khr_texture_transform_ext);

                        material_Properties.Add(_emissiveTexture);
                    }

                    //emission factor
                    Color _emissiveFactor = _material.GetColor("_EmissionColor");
                    if (_emissiveFactor != Color.black)
                        material_Properties.Add(new ObjectProperty("emissiveFactor", _emissiveFactor, true));

                }
                #endregion

                #region ADD ALPHA
                if (_alphaMode != "OPAQUE") material_Properties.Add(new ObjectProperty("alphaMode", _alphaMode));

                if (_alphaMode == "MASK")
                {
                    float _alphaCutoff = _material.GetFloat("_Cutoff");
                    if (_alphaCutoff != 0.5f)
                    {
                        material_Properties.Add(new ObjectProperty("alphaCutoff", _alphaCutoff));
                    }
                }
                #endregion

            }
            return material_Properties;
        }
    }
}
