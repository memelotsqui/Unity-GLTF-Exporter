using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class SphericalPanoramaToCubemapConverter
    {
        public static Cubemap GetCubemapFromSceneSkybox(int face_size = 1024)
        {
            // instantiate a prefab containing the converter
            GameObject sphericalToCubemapConverter = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(PathPreferences.skyboxToCubemapPrefabPath, typeof(GameObject)));

            // size of each cubemap face
            RenderTexture renderTexture = new RenderTexture(face_size, face_size, 0);

            // Get all cameras
            Camera[] ortoCamArray = sphericalToCubemapConverter.GetComponentsInChildren<Camera>();

            // Create the Cubemap
            Cubemap cm = new Cubemap(face_size, TextureFormat.RGBA32, false);

            for (int i = 0; i < ortoCamArray.Length; i++)
            {
                CubemapFace cubeFace = (CubemapFace)i;      //0 = x+, 1 = x-, 2 = y+, 3 = y-, 4 = z+, 5 = z-        
                ortoCamArray[i].targetTexture = renderTexture;
                ortoCamArray[i].Render();
                RenderTexture.active = renderTexture;
                Texture2D finalTexture = new Texture2D(face_size, face_size);
                finalTexture.ReadPixels(new Rect(0, 0, finalTexture.width, finalTexture.height), 0, 0);
                finalTexture.Apply();
                cm.SetPixels(finalTexture.GetPixels(), cubeFace);

                // clear data use
                GameObject.DestroyImmediate(finalTexture);
                RenderTexture.active = null;
                ortoCamArray[i].targetTexture.Release();
            }
            cm.Apply();

            GameObject.DestroyImmediate(sphericalToCubemapConverter);
            return cm;
        }
        public static Cubemap GetCubemapFromSphericalPanorama(Texture2D tarTexture, int optionalSize = 0, bool _skybox = false)
        {

            // instantiate a prefab containing the converter
            GameObject sphericalToCubemapConverter = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(PathPreferences.sphericalToCubemapPrefabPath, typeof(GameObject)));

            // get half size of the target panorama height texture
            int textureSize = (int)tarTexture.height / 2;
            if (optionalSize > 0)
                textureSize = optionalSize;
            RenderTexture renderTexture = new RenderTexture(textureSize, textureSize, 0);

            // Get all cameras
            Camera[] ortoCamArray = sphericalToCubemapConverter.GetComponentsInChildren<Camera>();

            // Create the Cubemap
            Cubemap cm = new Cubemap(textureSize,TextureFormat.RGBA32,false);

            Material sphereMat = sphericalToCubemapConverter.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            sphereMat.SetTexture("_BaseMap",tarTexture);
            sphereMat.mainTexture = tarTexture;

            for (int i = 0; i < ortoCamArray.Length; i++)
            {
                CubemapFace cubeFace = (CubemapFace)i;      //0 = x+, 1 = x-, 2 = y+, 3 = y-, 4 = z+, 5 = z-        
                ortoCamArray[i].targetTexture = renderTexture;
                ortoCamArray[i].Render();
                RenderTexture.active = renderTexture;
                Texture2D finalTexture = new Texture2D(textureSize, textureSize);
                finalTexture.ReadPixels(new Rect(0, 0, finalTexture.width, finalTexture.height), 0, 0);
                finalTexture.Apply();
                cm.SetPixels(finalTexture.GetPixels(), cubeFace);

                // clear data use
                GameObject.DestroyImmediate(finalTexture);
                RenderTexture.active = null;
                ortoCamArray[i].targetTexture.Release();
            }
            cm.Apply();

            GameObject.DestroyImmediate(sphericalToCubemapConverter);
            return cm;
        }
        public static Texture2D[] GetCubemapTexturesFromSphericalPanorama(Texture2D tarTexture, int optionalSize = 0)    //if tarSize = 0, it will take the height half
        {
            ImageGenerator.MakeReadableTexture(tarTexture);
            Cubemap tempCubemap = GetCubemapFromSphericalPanorama(tarTexture,optionalSize);
            return CubemapExtra.CreateCubemapTextures(tempCubemap);
        }
        

    }
}
