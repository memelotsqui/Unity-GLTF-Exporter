using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectImage
    {
        //consts
        public const string exportPreName = "img_";
        //public const bool exportInWebp = false;   //NOT IMPLEMENTED IN TRUE YET

        //statics 
        public static int globalIndex = -1;
        public static List<ObjectImage> allUniqueImages;

        //vars
        public int index = -1;

        TextureType textureType;
        public bool checkedAlpha = false;
        public bool hasAlpha = false;        
        public bool usesAlpha = false;
        public bool isAlpha = false;

        public float smooth = 1;
        public float metal = 1;

        public Texture2D image;
        public bool metalSmoothMap = false;     // METAL SMOOTH MAP MUST BE CREATED INSTEAD OF JUST COPIED, SO TEXTURE MIGHT BE THE SAME, BUT IF THE SAME TEXTURE IS USED IN METAL SMOOTHNESS MAP, A NEW TEXTURE MUST BE SAVED
        public List<ObjectProperty> imageProperties;
        public string exportName;
        public string originalName;

        public bool isFallback = false;

        public int quality = 0;
        public static void Reset()
        {
            globalIndex = 0;
            allUniqueImages = new List<ObjectImage>();
        }

        public static int GetImageIndex(Texture2D texture_2D, TextureType texture_type, bool metal_smooth_map, bool check_for_alpha, bool is_alpha = false, bool save_fallback = false, int _quality = 0, float _smoothnessMult = 1, float _metallicMult = 1)
        {
            if (allUniqueImages == null)
            {
                allUniqueImages = new List<ObjectImage>();
                allUniqueImages.Add(new ObjectImage(texture_2D, texture_type, metal_smooth_map, check_for_alpha, is_alpha, save_fallback,_quality, _smoothnessMult, _metallicMult));          
                if (save_fallback)
                {
                    ObjectImage nonFallbackImage = new ObjectImage(texture_2D, texture_type, metal_smooth_map, check_for_alpha, is_alpha, false,_quality, _smoothnessMult, _metallicMult);
                    allUniqueImages.Add(nonFallbackImage);                     // STORE THE MAIN IMAGE
                    return allUniqueImages.Count - 2;                           // RETURN JPEG FALLBACK IMAGE
                }

                return allUniqueImages.Count - 1;
            }
            else
            {
                foreach (ObjectImage oi in allUniqueImages)
                {
                    if (oi.IsSameImage(texture_2D, is_alpha))
                    {
                        if (check_for_alpha)
                            oi.CheckHasAlpha();
                        return oi.index;
                    }
                }
                // IF IMAGE IS NOT IN LIST, ADD IT TO THE LIST
                

                allUniqueImages.Add(new ObjectImage(texture_2D, texture_type, metal_smooth_map,check_for_alpha,is_alpha,save_fallback,_quality, _smoothnessMult, _metallicMult));           // FIRST SAVED TEXTURE IS NOT ALPHA
                if (save_fallback)
                {
                    ObjectImage nonFallbackImage = new ObjectImage(texture_2D, texture_type, metal_smooth_map, check_for_alpha, is_alpha,false,_quality, _smoothnessMult, _metallicMult);
                    allUniqueImages.Add(nonFallbackImage);                     // STORE THE MAIN IMAGE
                    return allUniqueImages.Count - 2;                           // RETURN JPEG FALLBACK IMAGE
                }
                return allUniqueImages.Count - 1;
            }
        }
        private void CheckHasAlpha()
        {
            if (!checkedAlpha)
            {
                hasAlpha = ImageGenerator.HasAlphaInformation(image);
                checkedAlpha = true;
            }
            if (hasAlpha)
            {
                usesAlpha = true;
                UpdateListToPNG();
            }

        }
        private bool IsSameImage(Texture2D texture_2D, bool is_alpha = false)
        {
            if (texture_2D == image && isAlpha == is_alpha)
            {
                
                return true;
            }
            return false;
        }

        public ObjectImage(Texture2D texture_2D, TextureType texture_type, bool metal_smooth_map, bool check_for_alpha, bool is_alpha, bool is_fallback, int _quality = 0, float _smoothnessMult = 1f, float _metallicMult = 1f)
        {
            
            image = texture_2D;
            quality = _quality;
            metal = _metallicMult;
            smooth = _smoothnessMult;
            if (quality > 100) quality = 100;
            if (quality < 0) quality = 0;       //if value is 0, program will take value from main gltf options
            textureType = texture_type;
            isFallback = is_fallback;
            metalSmoothMap = metal_smooth_map;
            isAlpha = is_alpha;
            index = globalIndex;
            exportName = exportPreName + globalIndex;
            originalName = texture_2D.name;
            if (check_for_alpha)
                CheckHasAlpha();

            imageProperties = GetImageProperties(texture_2D);
            if (usesAlpha)
                UpdateListToPNG();


            globalIndex++;
        }

        public List<ObjectProperty> GetImageProperties(Texture2D texture_2D)
        {
            // IF METAL SMOOTH MAP, A NEW TEXTURE MUST BE CREATED!!
            List<ObjectProperty> _image_properties = new List<ObjectProperty>();
            string _uri = exportName + ".jpg";
            string _name = originalName;
            string _mimeType = "image/jpeg";

            //if (isFallback)
            //Debug.Log("isFallback");
            TextureExportType export_type = ExportToGLTF.options.GetExportTextureType(textureType);
            if (export_type == TextureExportType.WEBP && !isFallback)
            {
                _mimeType = "image/webp";
                _uri = exportName + ".webp";
                // edit cmd
            }
            if (export_type == TextureExportType.KTX2 && !isFallback)
            {
                _mimeType = "image/ktx2";
                _uri = exportName + ".ktx2";
            }


            _image_properties.Add(new ObjectProperty("uri", _uri));
            _image_properties.Add(new ObjectProperty("mimeType", _mimeType));
            if (_name != "" && ExportToGLTF.options.exportTexturesName)
                _image_properties.Add(new ObjectProperty("name",_name));

            return _image_properties;
        }
        public void UpdateListToPNG()
        {
            if (imageProperties != null)
            {
                if (ExportToGLTF.options.exportTextureType == TextureExportType.DEFAULT)
                {
                    if (!ExportToGLTF.options.ExportSeparatedAlphaMap())
                    {
                        imageProperties[0].propertyString = exportName + ".png";
                        imageProperties[1].propertyString = "image/png";
                    }
                }
            }
        }
        public static void ExportAllImages(string location, int hdr_quality = 75, int normal_quality = 75, int metalRough_quality = 75, int default_quality = 75)
        {   
            AddCMDDeleteLine(location); // START CMD LINES END LINES
            RenderTextureImageConverter.InitializeRenderTexture();

            ConvertToEditableTextures();
            foreach (ObjectImage oi in allUniqueImages)
            {
                oi.ExportImage(location, hdr_quality, normal_quality, metalRough_quality, default_quality, oi.isFallback);
            }
            RenderTextureImageConverter.TerminateRenderTexture(); ;
            
        }


        public void ExportImage(string location, int hdr_quality=75, int normal_quality =75, int metalRough_quality = 75, int default_quality = 75, bool is_fallback = false)
        {
            //Debug.Log(exportName + " " + textureType);
            if (quality != 0)
                hdr_quality = normal_quality = metalRough_quality = default_quality = quality;

            int _divideFactor = 1;

            if (metalSmoothMap)
            {

                _divideFactor = ExportToGLTF.options.GetImagesDivideFactor("metallicSmoothness");
                AddCMDStartLine(ExportToGLTF.options.overrideMetallicSmoothnessExportTextureType);
                float metallicMultiplier = ExportToGLTF.options.metallicMultiplier;
                float smoothnessMultiplier = ExportToGLTF.options.smoothnessMultiplier;
                
                if (ExportToGLTF.options.overrideMetallicSmoothnessExportTextureType == TextureExportType.WEBP && !is_fallback)
                {
                    AddCMDWebpLine(exportName, location, "jpg", metalRough_quality);
                    Texture2D metalMap = ImageGenerator.CreateGLTFMetallicRoughness(image, metallicMultiplier * metal, smoothnessMultiplier * smooth);
                    FileExporter.ExportToJPEG(TextureScaler.scaled(metalMap, image.width / _divideFactor, image.height / _divideFactor, FilterMode.Bilinear), exportName, location, 100);

                }
                if (ExportToGLTF.options.overrideMetallicSmoothnessExportTextureType == TextureExportType.KTX2 && !is_fallback)
                {
                    AddCMDKTXLine(exportName, location, "jpg", metalRough_quality);
                    Texture2D metalMap = ImageGenerator.CreateGLTFMetallicRoughness(image, metallicMultiplier * metal, smoothnessMultiplier * smooth,true);
                    FileExporter.ExportToJPEG(TextureScaler.scaled(metalMap, image.width / _divideFactor, image.height / _divideFactor, FilterMode.Bilinear), exportName, location, 100);

                }
                if (ExportToGLTF.options.overrideMetallicSmoothnessExportTextureType == TextureExportType.DEFAULT || is_fallback)
                {
                    Texture2D metalMap = ImageGenerator.CreateGLTFMetallicRoughness(image, metallicMultiplier * metal, smoothnessMultiplier * smooth);
                    FileExporter.ExportToJPEG(TextureScaler.scaled(metalMap, image.width/_divideFactor, image.height/_divideFactor,FilterMode.Bilinear), exportName, location, metalRough_quality);
                    
                }
            }
            else
            {
                //switch (ImageGenerator.GetImageImportType(image))
                switch (textureType)
                {
                    case TextureType.CUBEMAP:
                        _divideFactor = ExportToGLTF.options.GetImagesDivideFactor("cubemap");
                        AddCMDStartLine(ExportToGLTF.options.overrideCubemapTextureType);
                        if (ExportToGLTF.options.overrideCubemapTextureType == TextureExportType.WEBP && !isFallback)
                        {
                            AddCMDWebpLine(exportName, location, "jpg", default_quality);
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                            //FileExporter.ExportToJPEG(image, exportName, location, 100);
                        }
                        if (ExportToGLTF.options.overrideCubemapTextureType == TextureExportType.KTX2 && !isFallback)
                        {
                            AddCMDKTXLine(exportName, location, "jpg", default_quality);
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                            //FileExporter.ExportToJPEG(image, exportName, location, default_quality);
                        }
                        if (ExportToGLTF.options.overrideCubemapTextureType == TextureExportType.DEFAULT || isFallback)
                        {
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, default_quality, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                            //FileExporter.ExportToJPEG(image, exportName, location, default_quality);
                        }
                        break;
                    case TextureType.LIGHTMAP:
                        _divideFactor = ExportToGLTF.options.GetImagesDivideFactor("lightmap");
                        AddCMDStartLine(ExportToGLTF.options.overrideLightmapExportTextureType);
                        if (ExportToGLTF.options.overrideLightmapExportTextureType == TextureExportType.WEBP && !is_fallback)
                        {
                            AddCMDWebpLine(exportName, location, "jpg", hdr_quality);
                            RenderTextureImageConverter.CreateNormalizedHDR(image, exportName, location, 100, true, ExportToGLTF.options.saturationLightmap,ExportToGLTF.options.maxLightmapClamp, ExportToGLTF.options.lightmapContrastCheat, _divideFactor);
                        }
                        if (ExportToGLTF.options.overrideLightmapExportTextureType == TextureExportType.KTX2 && !is_fallback)
                        {
                            AddCMDKTXLine(exportName, location, "jpg", hdr_quality);
                            RenderTextureImageConverter.CreateNormalizedHDR(image, exportName, location, 100, true, ExportToGLTF.options.saturationLightmap, ExportToGLTF.options.maxLightmapClamp, ExportToGLTF.options.lightmapContrastCheat, _divideFactor);
                        }
                        if (ExportToGLTF.options.overrideLightmapExportTextureType == TextureExportType.DEFAULT || is_fallback)      // IF IS A FALLBACK TEXTURE, MAKE SURE TO ALSO EXPORT IT AS A JPEG
                        {
                            RenderTextureImageConverter.CreateNormalizedHDR(image, exportName, location, hdr_quality, true, ExportToGLTF.options.saturationLightmap, ExportToGLTF.options.maxLightmapClamp, ExportToGLTF.options.lightmapContrastCheat, _divideFactor);
                        }
                        break;
                    case TextureType.NORMAL:
                        _divideFactor = ExportToGLTF.options.GetImagesDivideFactor("normal");
                        AddCMDStartLine(ExportToGLTF.options.overrideNormalExportTextureType);
                        if (ExportToGLTF.options.overrideNormalExportTextureType == TextureExportType.WEBP && !is_fallback)
                        {
                            AddCMDWebpLine(exportName, location, "jpg", normal_quality);
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.normal, 100,1, _divideFactor);
                        }
                        if (ExportToGLTF.options.overrideNormalExportTextureType == TextureExportType.KTX2 && !is_fallback)
                        {
                            AddCMDKTXLine(exportName, location, "jpg", normal_quality);
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.normal, 100, 1, _divideFactor);
                        }
                        if (ExportToGLTF.options.overrideNormalExportTextureType == TextureExportType.DEFAULT || is_fallback)  
                        {    
                            RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.normal, normal_quality, 1, _divideFactor);
                        }
                        break;
                    default:
                        _divideFactor = ExportToGLTF.options.GetImagesDivideFactor("default");
                        AddCMDStartLine(ExportToGLTF.options.overrideDefaultExportTextureType);
                        if (usesAlpha)
                        {
                            if (ExportToGLTF.options.ExportSeparatedAlphaMap())
                            {
                                if (!isAlpha)
                                {   // EXPORTS COLOR MAP FOR EACH TEXTURE TYPE == color map section ==
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.WEBP && !is_fallback)
                                    {
                                        AddCMDWebpLine(exportName, location, "jpg", default_quality);
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName,location,RenderTextureImageConverter.MaterialType.basic,100, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                                        //FileExporter.ExportToJPEG(image, exportName, location ,100);
                                    }
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.KTX2 && !is_fallback)
                                    {
                                        AddCMDKTXLine(exportName, location, "jpg", default_quality);
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                                        //FileExporter.ExportToJPEG(image, exportName, location,100);
                                    }
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.DEFAULT || is_fallback)
                                    {
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, default_quality, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                                        //FileExporter.ExportToJPEG(image, exportName, location, default_quality);
                                    }
                                }
                                else
                                {    // EXPORTS SEPARATED ALPHA MAP FOR EACH TEXTURE TYPE == alpha map section ==
                                    Texture2D alphaTexture = ImageGenerator.GetTextureFromAlpha(image, false);         
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.WEBP && !is_fallback)
                                    {
                                        AddCMDWebpLine(exportName, location, "jpg", default_quality);
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(alphaTexture, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, 1, _divideFactor);
                                        //FileExporter.ExportToJPEG(alphaTexture, exportName, location, 100);
                                    }
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.KTX2 && !is_fallback)
                                    {
                                        AddCMDKTXLine(exportName, location, "jpg", default_quality);
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(alphaTexture, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, 1, _divideFactor);
                                        //FileExporter.ExportToJPEG(alphaTexture, exportName, location, 100);
                                    }
                                    if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.DEFAULT || is_fallback)
                                    {
                                        RenderTextureImageConverter.CreateTextureWithRenderTexture(alphaTexture, exportName, location, RenderTextureImageConverter.MaterialType.basic, default_quality, 1, _divideFactor);
                                        //FileExporter.ExportToJPEG(alphaTexture, exportName, location, default_quality);
                                    }
                                }
                            }
                            else
                            {   // IF ALPHA MAP IS NOT SEPARATED == color map + alpha map section ==
                                // MISSING!!, MATERIAL DESATURATION ExportToGLTF.options.colorDesaturation
                                if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.WEBP && !is_fallback)
                                {
                                    AddCMDWebpLine(exportName, location, "png", default_quality);
                                    FileExporter.ExportToPNG(image, exportName, location);
                                }
                                if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.KTX2 && !is_fallback)
                                {
                                    AddCMDKTXLine(exportName, location, "png", default_quality);
                                    FileExporter.ExportToPNG(image, exportName, location);
                                }
                                if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.DEFAULT || is_fallback)
                                {
                                    FileExporter.ExportToPNG(image, exportName, location);
                                }
                            }
                        }
                        else
                        {   //IF IT DOESNT USE ALPHA, EXPORT NORMALLY
                            if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.WEBP && !isFallback)
                            {
                                AddCMDWebpLine(exportName, location, "jpg", default_quality);
                                RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, ExportToGLTF.options.colorTextureSaturation,_divideFactor);
                                //FileExporter.ExportToJPEG(image, exportName, location, 100);
                            }
                            if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.KTX2 && !isFallback)
                            {
                                AddCMDKTXLine(exportName, location, "jpg", default_quality);
                                RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, 100, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                                //FileExporter.ExportToJPEG(image, exportName, location, default_quality);
                            }
                            if (ExportToGLTF.options.overrideDefaultExportTextureType == TextureExportType.DEFAULT || isFallback)
                            {
                                RenderTextureImageConverter.CreateTextureWithRenderTexture(image, exportName, location, RenderTextureImageConverter.MaterialType.basic, default_quality, ExportToGLTF.options.colorTextureSaturation, _divideFactor);
                                //FileExporter.ExportToJPEG(image, exportName, location, default_quality);
                            }
                            //Debug.Log("dont use alpha");
                        }
                        break;
                }
            }
        }
        private static void ConvertToEditableTextures()
        {
            // MAKE SURE TO MAKE ALL TEXTURES DEFAULT FIRST
            //List<Texture2D> normalsHdrTexturesList = new List<Texture2D>();
            List<Texture2D> allTexturesList = new List<Texture2D>();
            foreach (ObjectImage oi in allUniqueImages)
            {
                allTexturesList.Add(oi.image);
                string img_type = ImageGenerator.GetImageImportType(oi.image);
                //if (img_type == "lightmap" || img_type == "normal")     // ONLY CHANGE LIGHTMAPS AND NORMALS, 
                //{
                    //normalsHdrTexturesList.Add(oi.image);
                //}
            }
            ImageGenerator.MakeReadableTexture(allTexturesList, false);
            //ImageGenerator.MakeTextureTypeDefault(normalsHdrTexturesList, false);
            ImageGenerator.RefreshDatabase();
            // END TEXTURE DEFINITION
        }
        private static void AddCMDStartLine(TextureExportType texture_type)        // MOVE TO CWEBP LOCATION
        {
            if (texture_type != TextureExportType.DEFAULT)
            {
                string converter_path = "";
                if (ExportToGLTF.options.exportTextureType == TextureExportType.WEBP)
                    converter_path = StringUtilities.GetFullPathFromLocalPath(PathPreferences.webpConverterLocalPath);
                if (ExportToGLTF.options.exportTextureType == TextureExportType.KTX2)
                    converter_path = StringUtilities.GetFullPathFromLocalPath(PathPreferences.ktx2ConverterLocalPath);
                string result = converter_path.Split(':')[0] + ":\n";
                result += "cd " + converter_path + "\n";
                ExportToGLTF.cmd.AddLine(result);
            }
        }
        public void AddCMDWebpLine(string name, string location, string extension, int img_quality)
        {
            string result = "cwebp -q "+ img_quality +" " + location + "/" + name + "." + extension + " -o " + location + "/" + name + ".webp\n";
            string endResult = "del " + location + "/" + name + "." + extension + "\n";
            endResult = endResult.Replace("/", "\\");
            // DELETE OLD TEXTURE?
            ExportToGLTF.cmd.AddLine(result);
            ExportToGLTF.cmd.AddEndLine(endResult);
        }
        public void AddCMDKTXLine(string name, string location, string extension, int img_quality)
        {
            string result = "toktx --t2 --bcmp --genmipmap --filter box --assign_oetf linear " + location + "/" + name + ".ktx2 " + location + "/" + name + "." + extension + "\n";
            string endResult = "del " + location + "/" + name + "." + extension + "\n";
            endResult = endResult.Replace("/", "\\");
            //string result = "toktx --t2 --bcmp --qlevel " + (Mathf.RoundToInt(img_quality * 2.54f) + 1) + " --genmipmap --filter box " + location + "/" + name + ".ktx2 " + location + "/" + name + "." + extension + "\n";
            //string result = "toktx --t2 --uastc 1 --genmipmap --filter box --zcmp 10 --uastc_rdo_l 1 --uastc_rdo_d 4096 --uastc_rdo_b 10 --uastc_rdo_f --uastc_rdo_m " + location + "/" + name + ".ktx2 " + location + "/" + name + "." + extension + "\n";


            //result += "ktxsc --bcmp --qlevel " + (Mathf.RoundToInt(img_quality * 2.54f) + 1) + " --genmipmap " + location + "/" + name + ".ktx2 " + location + "/" + name + "." + extension + "\n";
            //string result = "toktx --t2 --uastc 0 --zcmp 20 --genmipmap " + location + "/" + name + ".ktx2 " + location + "/" + name + "." + extension + "\n";
            // DELETE OLD TEXTURE?
            ExportToGLTF.cmd.AddLine(result);
            ExportToGLTF.cmd.AddEndLine(endResult);
        }

        private static void AddCMDDeleteLine(string images_location)        // MOVE TO CWEBP LOCATION
        {
            if (ExportToGLTF.options.exportTextureType != TextureExportType.DEFAULT)
            {
                string result = images_location.Split(':')[0] + ":\n";
                //result += "cd " + images_location + "/\n";
                //result += "del " + "*.jpg\n";
                //result += "del " + "*.png\n";
                ExportToGLTF.cmd.AddEndLine(result);
            }
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueImages.Count > 0)
            {
                result += "\"images\" : [\n";
                for (int i = 0; i < allUniqueImages.Count; i++)
                {
                    result += ObjectProperty.GetObjectProperties(allUniqueImages[i].imageProperties);
                    if (i < allUniqueImages.Count - 1)
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
