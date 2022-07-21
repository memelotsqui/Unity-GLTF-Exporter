using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class PathPreferences
    {
        public const string webpConverterLocalPath = "Assets/Unity-GLTF-Exporter/ExternalUtilities/libwebp-1.2/";
        public const string ktx2ConverterLocalPath = "Assets/Unity-GLTF-Exporter/ExternalUtilities/KTX-Software/bin/";

        public const string chromeLocation = "C:/Program Files (x86)/Google/Chrome/Application/chrome.exe";
        public const string localServerLocation = "C:/wamp64/www/";

        public const string cubemapModelFBXRelativePath = "Assets/Unity-GLTF-Exporter/Models/Utilities/cubemap_threejs.prefab";

        public const string standardTextureGenerationMaterialFolder = "Assets/Unity-GLTF-Exporter/PrefabUtils/MaterialEditor/Standard/";
        public const string URPTextureGenerationMaterialFolder = "Assets/Unity-GLTF-Exporter/PrefabUtils/MaterialEditor/URP/";

        public const string renderTextureHolderPrefabPath = "Assets/Unity-GLTF-Exporter/PrefabUtils/RenderTextureHolder.prefab";

        public const string normalMapSimpleMaterialName = "NormalMapSimpleMaterial.mat";
        public const string HDRSimpleMaterialName = "HDRSimpleMaterial.mat";
        public const string basicSimpleMaterialName = "BasicSimpleMaterial.mat";
        public const string basicEditableMaterialName = "BasicEditableMaterial.mat";

        public const string skyboxToCubemapPrefabPath = "Assets/Unity-GLTF-Exporter/PrefabUtils/Skybox_Converter.prefab";
        public const string sphericalToCubemapPrefabPath = "Assets/Unity-GLTF-Exporter/PrefabUtils/SphericalToCubemap_Converter.prefab";
    }
}
