using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WEBGL_EXPORTER
{
    public class RenderTextureImageConverter
    {
        private static GameObject renderTextureImageGen;
        private static Camera ortoCam;
        private static MeshRenderer planeMeshRenderer;
        private static Material normalMapMaterial;
        private static Material hdrMaterial;
        private static Material basicMaterial;

        private static Vector3 initLocalScale;
        public enum MaterialType { basic, hdr, normal };
        public static void InitializeRenderTexture()
        {
            string loc = "";
            if (!loc.EndsWith("/") || !loc.EndsWith("\\"))
                loc += "/";
            if (RenderPipelineManager.currentPipeline == null)//standard
                loc = PathPreferences.standardTextureGenerationMaterialFolder;
            else
                loc = PathPreferences.URPTextureGenerationMaterialFolder;   //FOR NOW WILL EITHER BE URP OR STANDARD
            renderTextureImageGen = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(PathPreferences.renderTextureHolderPrefabPath, typeof(GameObject)));
            ortoCam = renderTextureImageGen.GetComponent<Camera>();
            planeMeshRenderer = renderTextureImageGen.GetComponentInChildren<MeshRenderer>();
            initLocalScale = planeMeshRenderer.transform.localScale;
            normalMapMaterial = AssetDatabase.LoadAssetAtPath(loc + PathPreferences.normalMapSimpleMaterialName, typeof(Material)) as Material;
            hdrMaterial = AssetDatabase.LoadAssetAtPath(loc + PathPreferences.HDRSimpleMaterialName, typeof(Material)) as Material;
            //basicMaterial = AssetDatabase.LoadAssetAtPath(loc + PathPreferences.basicSimpleMaterialName, typeof(Material)) as Material;
            basicMaterial = AssetDatabase.LoadAssetAtPath(loc + PathPreferences.basicEditableMaterialName, typeof(Material)) as Material;
            basicMaterial.EnableKeyword("_BaseMap");
        }
        public static void TerminateRenderTexture()
        {
            GameObject.DestroyImmediate(renderTextureImageGen);
        }

        public static void CreateDualTextureWithHDR(Texture2D texture, string base_name, string extra_name, string path, float shadow_multiply=1, float light_dim=1, int quality = 75)
        {
            hdrMaterial.SetFloat("_ShadowMultiplier", shadow_multiply);
            hdrMaterial.SetFloat("_LightDim", light_dim);
            planeMeshRenderer.sharedMaterial.SetInt("_GetShadow", 1);
            CreateTextureWithRenderTexture(texture, base_name, path,MaterialType.hdr,quality);
            planeMeshRenderer.sharedMaterial.SetInt("_GetShadow", 0);
            CreateTextureWithRenderTexture(texture, extra_name, path, MaterialType.hdr, quality);
        }
        public static float CreateNormalizedHDR(Texture2D texture_hdr,  string name, string path, int quality = 75, bool isLightmap = true, float saturate_compensation = 1.3f, float max_factor = 20, float contrast_modify = 1f, int textureSizeDivide = 1)
        {
            if (isLightmap)
                ImageGenerator.MakeTextureTypeLightmap(texture_hdr);
            planeMeshRenderer.sharedMaterial = hdrMaterial;
            planeMeshRenderer.sharedMaterial.SetTexture("_BaseMap", texture_hdr);
            planeMeshRenderer.sharedMaterial.SetFloat("_SaturateCompensation", saturate_compensation);
            float _factor = ImageUtilities.GetImageMaxPixelFloatValue(texture_hdr, max_factor,true);
            planeMeshRenderer.sharedMaterial.SetInt("_GetFactor256", 0);
            planeMeshRenderer.sharedMaterial.SetFloat("_ScaleFactor",_factor);
            planeMeshRenderer.sharedMaterial.SetFloat("_Contrast", contrast_modify);
            CreateTextureWithRenderTexture(texture_hdr,name,path,MaterialType.hdr,quality,1, textureSizeDivide);
            return _factor;
        }

        //private static float GetMultiplyFactorOld(Texture2D hdr_texture, int max_factor, string name, string path, int quality = 75)
        //{
        //    if (ortoCam.targetTexture != null)
        //    {
        //        RenderTexture.active = null;
        //        ortoCam.targetTexture.Release();
        //    }

        //    RenderTexture renderTexture = new RenderTexture(hdr_texture.width, hdr_texture.height, 0);
        //    ortoCam.targetTexture = renderTexture;

        //    if (hdr_texture.width == hdr_texture.height)
        //        planeMeshRenderer.gameObject.transform.localScale = initLocalScale;

        //    float multiply = (float)hdr_texture.width / (float)hdr_texture.height;
        //    if (hdr_texture.height > hdr_texture.width)
        //        multiply = (float)hdr_texture.width / (float)hdr_texture.height;

        //    planeMeshRenderer.gameObject.transform.localScale = new Vector3(initLocalScale.x * multiply, initLocalScale.y, initLocalScale.z);

        //    ortoCam.Render();

        //    Texture2D final = new Texture2D(hdr_texture.width, hdr_texture.height);
        //    RenderTexture.active = renderTexture;
        //    final.ReadPixels(new Rect(0, 0, final.width, final.height), 0, 0);
        //    final.Apply();


        //    FileExporter.ExportToJPEG(final, name, path, quality);

        //    Color32[] allColors = final.GetPixels32();
        //    float maxVal = 0;
        //    Debug.Log(allColors[57531].r);
        //    Debug.Log(allColors[0].r);
        //    int savei = 0;
        //    int incolor = 0;
        //    for (int i =0; i < allColors.Length;i++)
        //    {
        //        if (allColors[i].r > maxVal)
        //        {
        //            maxVal = allColors[i].r;
        //            incolor = 0;
        //            savei = i;
        //        }
        //        if (allColors[i].g > maxVal)
        //        {
        //            maxVal = allColors[i].g;
        //            incolor = 1;
        //            savei = i;
        //        }
        //        if (allColors[i].b > maxVal)
        //        {
        //            maxVal = allColors[i].b;
        //            incolor = 2;
        //            savei = i;
        //        }
        //    }
        //    Debug.Log(maxVal + " in " + savei + " in color " + incolor);
        //    float result = maxVal * 100f;
        //    if (result > max_factor)
        //        result = max_factor;
        //    if (ortoCam.targetTexture != null)
        //    {
        //        RenderTexture.active = null;
        //        ortoCam.targetTexture.Release();
        //    }
            
        //    return result;

        //}
        public static void CreateTextureWithRenderTexture(Texture2D texture, string name, string path, MaterialType mat = MaterialType.basic,int quality = 75, float basicMaterialSaturation = 1f, int textureSizeDivide = 1, bool test = false)
        {
           
            if (ortoCam.targetTexture != null)
            {
                RenderTexture.active = null;
                ortoCam.targetTexture.Release();
            }

            // SET MATERIAL
            if (mat == MaterialType.basic)
            {
                planeMeshRenderer.sharedMaterial = basicMaterial;
                basicMaterial.SetTexture("_BaseMap", texture);

                //planeMeshRenderer.sharedMaterial.SetTexture("_BaseMap", texture as Texture2D);

                planeMeshRenderer.sharedMaterial.SetFloat("_Saturation", basicMaterialSaturation);

            }


            if (mat == MaterialType.hdr)
            {
                planeMeshRenderer.sharedMaterial = hdrMaterial;
                planeMeshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
            }
            else if (mat == MaterialType.normal)
            {
                planeMeshRenderer.sharedMaterial = normalMapMaterial;
                planeMeshRenderer.sharedMaterial.SetTexture("_NormalTextureMap", texture);
            }

            //SET MATERIAL ATTRIBUTES IN BASIC MATERIAL
            

            //CREATE TEXTURE FROM RENDER TEXTURE
            RenderTexture renderTexture = new RenderTexture(texture.width/ textureSizeDivide, texture.height/ textureSizeDivide, 0);
            ortoCam.targetTexture = renderTexture;

            if (texture.width == texture.height)
            {
                planeMeshRenderer.gameObject.transform.localScale = initLocalScale;
            }

            //Debug.Log("=== texture map dimensions === width: " + texture.width + " - height: " + texture.height + texture.name);
            if (texture.width > texture.height)
            {
                float multiply = (float)(texture.width/ textureSizeDivide) / (float)(texture.height/ textureSizeDivide);
                planeMeshRenderer.gameObject.transform.localScale = new Vector3 (initLocalScale.x * multiply, initLocalScale.y, initLocalScale.z);
            }
            if (texture.height > texture.width)
            {
                
                float multiply = (float)(texture.width/ textureSizeDivide) / (float)(texture.height/ textureSizeDivide);
                planeMeshRenderer.gameObject.transform.localScale = new Vector3(initLocalScale.x * multiply, initLocalScale.y, initLocalScale.z);
            }

            ortoCam.Render();

            Texture2D final = new Texture2D(texture.width/ textureSizeDivide, texture.height/ textureSizeDivide);
            RenderTexture.active = renderTexture;
            final.ReadPixels(new Rect(0, 0, final.width, final.height), 0, 0);
            final.Apply();

            FileExporter.ExportToJPEG(final, name, path, quality);
            

            if (ortoCam.targetTexture != null)
            {
                RenderTexture.active = null;
                ortoCam.targetTexture.Release();
            }
        }
        public static void MultiCreateTexturesFromHDRs(List<Texture2D> HDRTextures, string prename, string texturesPath)
        {
            //Texture2D[] result = new Texture2D[HDRTextures.Length];
            if (HDRTextures.Count > 0)
            {
                //GameObject renderTextureImageGen = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/MaquetaWEB/PrefabUtils/RenderTextureHolder.prefab", typeof(GameObject)));
                //Camera ortoCam = renderTextureImageGen.GetComponent<Camera>();
                //MeshRenderer planeMesh = renderTextureImageGen.GetComponentInChildren<MeshRenderer>();



                for (int i = 0; i < HDRTextures.Count; i++)
                {
                    if (ortoCam.targetTexture != null)
                    {
                        RenderTexture.active = null;
                        ortoCam.targetTexture.Release();
                    }
                    RenderTexture renderTexture = new RenderTexture(HDRTextures[i].width, HDRTextures[i].height, 0);
                    ortoCam.targetTexture = renderTexture;
                    ImageGenerator.MakeTextureTypeDefault(HDRTextures[i]);
                    planeMeshRenderer.sharedMaterial.SetTexture("_BaseMap", HDRTextures[i]);
                    ortoCam.Render();

                    Texture2D final = new Texture2D(HDRTextures[i].width, HDRTextures[i].height);
                    RenderTexture.active = renderTexture;
                    final.ReadPixels(new Rect(0, 0, final.width, final.height), 0, 0);
                    final.Apply();

                    FileExporter.ExportToJPEG(final, prename + i, texturesPath);
                }
                if (ortoCam.targetTexture != null)
                {
                    RenderTexture.active = null;
                    ortoCam.targetTexture.Release();
                }
                GameObject.DestroyImmediate(renderTextureImageGen);
            }
        }
    }
}
