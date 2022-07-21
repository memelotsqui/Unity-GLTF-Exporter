using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class ImageGenerator
    {
        //public static TextureHolder GetTextureHolderFromAlpha(TextureHolder tarTexture)
        //{
        //    return new TextureHolder(GetTextureFromAlpha(tarTexture.savedTexture.texture), tarTexture.savedName + "t");
        //}
        public static string GetImageImportType(Texture2D tarTexture)
        {
            string assetPath = AssetDatabase.GetAssetPath(tarTexture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            TextureImporterSettings importSettings = new TextureImporterSettings();
            if (tImporter != null)
            {
                tImporter.ReadTextureSettings(importSettings);

                if (importSettings.textureType == TextureImporterType.Lightmap)
                    return "lightmap";
                if (importSettings.textureType == TextureImporterType.NormalMap)
                    return "normal";
            }
            return "default";
        }
        public static bool HasAlphaInformation(Texture2D tarTexture)
        {
            MakeReadableTexture(tarTexture);
            Color[] pixels = tarTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < .95f)
                {
                    return true;
                }
            }
            return false;
        }

        public static Texture2D[] CreateMultiTextureFromHDR(Texture2D hdrTexture, int textureQty = 2)
        {
            MakeReadableTexture(hdrTexture);
            Texture2D[] result = new Texture2D[textureQty];
            for (int i =0; i < textureQty; i++)
            {
                result[i] = new Texture2D(hdrTexture.width, hdrTexture.height);
                Color[] pixels = hdrTexture.GetPixels();
                float min = i * (1f / textureQty);
                Debug.Log(min);
                //float max = min + (1 / textureQty);
                Debug.Log(hdrTexture.GetPixel(3,3).r);
                
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j].r = Mathf.Clamp01((pixels[j].r - min) * textureQty);
                    pixels[j].g = Mathf.Clamp01((pixels[j].g - min) * textureQty);
                    pixels[j].b = Mathf.Clamp01((pixels[j].b - min) * textureQty);
                }
                result[i].SetPixels(pixels);
                result[i].Apply();
            }
            return result;
        }

        public static Texture2D CreateGLTFMetallicRoughness(Texture2D tarTexture, float metallicMultiplier, float smoothnessMultiplier, bool isKTX = false)
        {
            Debug.Log(smoothnessMultiplier);
            MakeReadableTexture(tarTexture);
            Texture2D result = new Texture2D(tarTexture.width, tarTexture.height);
            Color[] pixels = tarTexture.GetPixels();
            // alpha is smoothness
            // any color is metallic, we pick red
            Debug.Log(pixels.Length);
            if (!isKTX)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].g = 1f - (pixels[i].a * smoothnessMultiplier);
                    pixels[i].r = pixels[i].r * metallicMultiplier;
                    pixels[i].b = 0f;
                    pixels[i].a = 1f;
                }
            }
            else
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].g = 1f - (pixels[i].a * smoothnessMultiplier);
                    pixels[i].r = pixels[i].r * metallicMultiplier;
                    pixels[i].b = 0f;
                    pixels[i].a = 1f;

                    //pixels[i].b = pixels[i].r * metallicMultiplier;

                    //pixels[i].g = pixels[i].a * smoothnessMultiplier;//smoothness
                }
                
            }
            result.SetPixels(pixels);
            return result;
        }
        public static Texture2D GetTextureFromAlpha(Texture2D tarTexture, bool inverse = false)
        {
            MakeReadableTexture(tarTexture);
            Texture2D result = new Texture2D(tarTexture.width, tarTexture.height);
            Color[] pixels = tarTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (!inverse)
                    pixels[i].r = pixels[i].g = pixels[i].b = pixels[i].a;
                else
                    pixels[i].r = pixels[i].g = pixels[i].b = 1.0f - pixels[i].a;
            }
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        public static Texture2D InvertTextureColors(Texture2D tarTexture)
        {
            Texture2D result = new Texture2D(tarTexture.width, tarTexture.height);
            Color[] pixels = tarTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = 1.0f - pixels[i].r;
                pixels[i].r = 1.0f - pixels[i].g;
                pixels[i].r = 1.0f - pixels[i].b;
            }
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        public static Texture2D GetTextureFromPixels(Color[] pixels, int sizeX, int SizeY, bool invWidth = false, bool invHeight = false)
        {
            Texture2D result = new Texture2D(sizeX, SizeY);
            result.SetPixels(pixels);

            return InvertTexture(result, invWidth, invHeight);

        }
        public static Texture2D InvertTexture(Texture2D texture, bool invWidth, bool invHeight)
        {
            if (texture == null)
                return null;
            if (!invWidth && !invHeight)
                return texture;

            Color[] pixels = texture.GetPixels();
            Color[] invPixels = new Color[pixels.Length];
            if (invWidth && !invHeight)//corroborar
            {
                for (int i = 0; i < texture.height; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {
                        invPixels[(texture.width * (i + 1)) - j - 1] = pixels[(texture.width * i) + j];
                    }
                }
            }
            if (!invWidth && invHeight)//corroborar
            {
                for (int i = 0; i < texture.height; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {

                        invPixels[((texture.height - i - 1) * texture.width) + j] = pixels[(texture.width * i) + j];
                    }
                }
            }
            if (invWidth && invHeight)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    invPixels[i] = pixels[pixels.Length - 1 - i];
                }
                
            }
            Texture2D result = new Texture2D(texture.width, texture.height);
            result.SetPixels(invPixels);
            result.Apply();

            return result;
            
            
        }

        public static Texture2D GetTextureFromColor(Color tarColor, int sizeX, int sizeY, bool convertToLinear = false)
        {
            Texture2D result = new Texture2D(sizeX, sizeY);
            if (convertToLinear)
                tarColor = tarColor.linear;
            Color[] pixels = new Color[sizeX * sizeY];
            for (int i = 0; i < pixels.Length; i ++)
                pixels[i] = tarColor;
            result.SetPixels(pixels);
            result.Apply();

            return result;
        }
        public static float SingleFloatFromAlpha(Texture2D tarTexture) // returns a float if all the alpha image has a single color
        {
            MakeReadableTexture(tarTexture);
            Color[] pixels = tarTexture.GetPixels();
            float result = pixels[0].a;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (result != pixels[i].a)
                {
                    result = -1f;
                    break;
                }
            }
            return result;
        }
        public static float SinlgeFloatFromTexture(Texture2D tarTexture) // returns a float if all the red image has the same color, this is useful for black and white images (metal)
        {
            MakeReadableTexture(tarTexture);
            Color[] pixels = tarTexture.GetPixels();
            float result = pixels[0].r;
            float result2 = pixels[0].g;
            float result3 = pixels[0].b;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (result != pixels[i].r)
                {
                    result = -1f;
                    break;
                }
                if (result2 != pixels[i].g)
                {
                    result = -1f;
                    break;
                }
                if (result3 != pixels[i].b)
                {
                    result = -1f;
                    break;
                }
            }
            return result;
        }

        public static void MakeReadableTexture(Texture2D texture, bool uncompressed = true)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            bool changed = false;
            if (tImporter != null)
            {
                if (uncompressed && tImporter.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }
                if (!tImporter.isReadable || tImporter.maxTextureSize != 8192)
                {
                    //tImporter.textureType = TextureImporterType.Default;
                    tImporter.isReadable = true;
                    tImporter.maxTextureSize = 8192;
                    changed = true;

                }
                if (changed)
                {
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }

            }
        }
        public static void MakeReadableTexture(List<Texture2D> textures, bool uncompressed = true, bool refresh = true)
        {
            bool changed = false;
            for (int i =0; i < textures.Count; i++) { 
                string assetPath = AssetDatabase.GetAssetPath(textures[i]);
                var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                
                if (tImporter != null)
                {
                    if (uncompressed && tImporter.textureCompression != TextureImporterCompression.Uncompressed)
                    {
                        tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                        changed = true;
                    }
                    if (!tImporter.isReadable || tImporter.maxTextureSize != 8192)
                    {
                        //tImporter.textureType = TextureImporterType.Default;
                        tImporter.isReadable = true;
                        tImporter.maxTextureSize = 8192;
                        changed = true;

                    }
                    if (changed)
                    {
                        AssetDatabase.ImportAsset(assetPath);
                    }
                }
            }
            if (changed && refresh)
            {
                AssetDatabase.Refresh();
            }
        }

        public static void MakeTextureTypeDefault(List<Texture2D> textures, bool refresh = true)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(textures[i]);
                var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (tImporter != null)
                {
                    if (tImporter.textureType != TextureImporterType.Default)
                    {
                        tImporter.textureType = TextureImporterType.Default;
                        AssetDatabase.ImportAsset(assetPath);
                    }
                }
            }
            if (refresh)
                AssetDatabase.Refresh();
        }
        public static void RefreshDatabase()
        {
            AssetDatabase.Refresh();
        }
        public static void MakeTextureTypeDefault(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                if (tImporter.textureType != TextureImporterType.Default)
                {
                    tImporter.textureType = TextureImporterType.Default;
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
            }
        }
        public static void MakeReadableCubemap(Cubemap texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                if (!tImporter.isReadable || tImporter.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    tImporter.isReadable = true;
                    tImporter.textureCompression = TextureImporterCompression.Uncompressed;

                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
            }
        }

        public static void MakeTextureTypeLightmap(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                bool changed = false;
                if (tImporter.textureType != TextureImporterType.Lightmap)
                {
                    tImporter.textureType = TextureImporterType.Lightmap;
                    changed = true;
                }
                if (!tImporter.isReadable)
                {
                    tImporter.isReadable = true;
                    changed = true;
                }
                if (tImporter.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }
                if (changed)
                {
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
            }
        }
        public static Color MultColor(Color color1, Color color2)
        {
            return new Color(color1.r * color2.r, color1.g * color2.g, color1.b * color2.b);

        }
        public static Color ClampRangeColor(Color color)
        {
            Color result = new Color(MathExtraUtils.ClampRangeValue (color.r,0.2f,0.8f),
                                        MathExtraUtils.ClampRangeValue (color.g, 0.2f, 0.8f),
                                        MathExtraUtils.ClampRangeValue(color.b, 0.2f, 0.8f), 
                                        color.a);
            return result;
        }

        public static Texture2D CreateSimpleTexture(Color color, int size = 16)
        {
            Color[] pixels = new Color[Mathf.RoundToInt(Mathf.Pow(size, 2f))];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            return GetTextureFromPixels(pixels, size, size);
        }
        public static Vector2 GetImageOriginalSize(Texture2D tarTexture)       // widt/height
        {

            Texture2D tmpTexture = new Texture2D(1, 1);
            byte[] tmpBytes = System.IO.File.ReadAllBytes(StringUtilities.GetFullPathFromAsset(tarTexture));
            tmpTexture.LoadImage(tmpBytes);
            Vector2 result = new Vector2(tmpTexture.width, tmpTexture.height);
            Object.DestroyImmediate(tmpTexture);
            return result;
        }

    }
}
