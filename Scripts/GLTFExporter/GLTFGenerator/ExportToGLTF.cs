using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WEBGL_EXPORTER;
using WEBGL_EXPORTER.GLTF;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    [ExecuteAlways]
    public class ExportToGLTF
    {

        public static CMDGenerator cmd;
        public static ExportGLTFOptions options;

        public static void CallExportGLTF(string location, string gltfName, Transform transform_parent, ExportGLTFOptions export_options = null, bool resetLists= true, string optionalFolderCreateName = "", bool open_after_build = false)
        {
            RearrangeCameras(transform_parent); //cameras are rearranged in the final nodes to be the last siblings, not modifying them from here, results in having an offset in every object after the camera
            if (resetLists)         //USED IN AUTO CODE GENERATOR
                ClearListsAndReset();
            cmd = new CMDGenerator();
            if (export_options != null)
                options = export_options;
            if (options == null)
                options = new ExportGLTFOptions();

            options.SetupTextureOverrides();    //MAKE SURE TO KNOW OVERRIDE OF EACH TEXTURE TYPE

            

            if (location == "")
                location = Application.dataPath;

            string original_location = location;
            if (optionalFolderCreateName != "")
                location = FileExporter.CreateFolder(location + "/" + optionalFolderCreateName);

            options.exportLocation = location;

            //CUBE SKYBOX MAYBE IT WONT BE USED 
            if (options.createCubeForSkybox)
            {
                //BREAK IT FOR NOW, IT MIGHT NO LONGER BE USED
                //CreateCubeSybox(transform_parent);
            }

            //NODES
            bool export_inactive = options.exportInactive;
            Transform[] transform_childs = transform_parent.GetComponentsInChildren<Transform>(export_inactive);

            for (int i = 0; i < transform_childs.Length; i++)
            {
                ObjectNodeMono _node = transform_childs[i].GetComponent<ObjectNodeMono>();
                if (_node == null)
                    _node = transform_childs[i].gameObject.AddComponent<ObjectNodeMono>();

                if (!transform_childs[i].gameObject.activeSelf)
                    _node.SetupNode(false);      // EXPORT HIDDEN OBJECTS
                else
                    _node.SetupNode();           // EVERYTHING HAPPENS HERE
            }


            //SCENE
            ObjectScene.SetupScene();

            //ALL EXTRAS USED IN MONO
            ObjectNodeUserExtrasMono[] extras_childs = transform_parent.GetComponentsInChildren<ObjectNodeUserExtrasMono>(export_inactive);
            for (int i =0; i < extras_childs.Length; i++)
            {
                ObjectNodeUserExtrasMono _node_extras = extras_childs[i];
                _node_extras.ResetComputedProperties();
                _node_extras.SetGLTFComputedData();
                _node_extras.CombineProperties();
            }

            //GENERATE NAVMESH HERE
            if (options.exportNavMesh)
            {
                ObjectNodeMono.SetupNavMesh(transform_parent);
                if (ObjectNodeMono.navMeshNode != -1)
                {
                    ObjectMasterExtras.Add(new ObjectProperty("navMesh", ObjectNodeMono.navMeshNode));
                }
            }

            // USER DEFINED MASTER EXTRAS
            ObjectMasterUserExtrasMono user_extras_master = transform_parent.GetComponentInChildren<ObjectMasterUserExtrasMono>();
            if (user_extras_master != null)
                user_extras_master.AddPropertiesToMaster();
            ObjectMasterComputedExtrasMono computed_extras_master = transform_parent.GetComponentInChildren<ObjectMasterComputedExtrasMono>();
            if (computed_extras_master != null)
                computed_extras_master.AddPropertiesToMaster();

            // QUANTIZATION
            if (options.quantizeGLTF && options.blockLeafNodesCreation == false)
                CreateKHR_mesh_quantization();

            if (location == "")
                Debug.LogError("gltf folder already exists");
            else
                SaveFiles(gltfName, location, open_after_build, original_location);

            if (options.cubeSkybox != null)
                GameObject.DestroyImmediate(options.cubeSkybox);
        }

        private static ObjectExtension CreateKHR_mesh_quantization()
        {
            ObjectExtension khr_mesh_quantization = new ObjectExtension("KHR_mesh_quantization", new List<ObjectProperty>(), true);
            return khr_mesh_quantization;
        }
        private static void CreateCubeSybox(Transform parent)
        {
            if (options.cubeSkybox == null)
                options.cubeSkybox = (GameObject)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath(PathPreferences.cubemapModelFBXRelativePath, typeof(GameObject)));

            options.cubeSkybox.transform.parent = parent;
            options.cubeSkybox.name = "custom_skybox";
            options.cubeSkybox.transform.localScale = new Vector3(0.00001f, 0.00001f, 0.00001f);
        }
        private static void SaveFiles(string file_name, string file_location, bool open_after_build, string original_location)
        {
            ObjectAccessors.WriteToBinary(file_name, file_location);
            WriteToGLTF(file_name, file_location);
            if (open_after_build)
            {
                Application.OpenURL(file_location);
            }
            ExecuteCMD(file_name, file_location, open_after_build, original_location);
        }
        private static void RearrangeCameras(Transform targetTransform)
        {
            Camera [] childCams = targetTransform.GetComponentsInChildren<Camera>();
            for (int i =0; i < childCams.Length; i++)
            {
                childCams[i].transform.SetAsLastSibling();
            }
        }
        public static void ExecuteCMD(string file_name, string file_location, bool open_after_build, string original_location)
        {
            // if export glb
            // cmd.AddLine() gltf-pipeline -i C:/wamp64/www/web_models_3/3dmodels_3/casa_club_4/models/gltf.gltf -o C:/wamp64/www/web_models_3/3dmodels_3/casa_club_4/models/gltf.glb

            string explorer_location = "";
            string local_page_location = "";
            string model_id = "";
            string disc_route = "";
            if (open_after_build)
            {
                explorer_location = PathPreferences.chromeLocation;
                disc_route = PathPreferences.localServerLocation.Split(':')[0] + ":\n";
                local_page_location = "localhost/" + original_location.Replace(PathPreferences.localServerLocation, "") + "/";
                model_id = StringUtilities.GetFolderNameFromSubfolder(file_location);
            }

            string cmdLine = cmd.GetCommand(true, explorer_location, local_page_location, disc_route, model_id);
            Debug.Log(cmdLine);
            if (cmdLine != "")
            {
                //cmdLine = "TIMEOUT /T 1\n" + cmdLine;
                string cmd_location = FileExporter.ExportToText(cmdLine, file_name, file_location, "cmd");
                Debug.Log(cmd_location);
                Application.OpenURL(cmd_location);
            }
        }
        private static void WriteToGLTF(string file_name, string file_location)
        {
            string result = "{\n";
            result += ObjectExtension.GetGLTFHeaderExtensionData();
            result += ObjectBuffer.GetGLTFData(file_name);
            result += ObjectBufferView.GetGLTFData();
            result += ObjectAccessors.GetGLTFData();
            result += ObjectMesh.GetGLTFData();
            result += ObjectMaterial.GetGLTFData();
            result += ObjectSampler.GetGLTFData();
            result += ObjectTexture.GetGLTFData();
            ObjectExtrasCubeTextures.AddGLTFDataToExtras();
            result += ObjectImage.GetGLTFData();
            result += ObjectNodeMono.GetGLTFData();
            result += ObjectScene.GetGLTFData();
            result += ObjectCamera.GetGLTFData();//new
            result += ObjectSkin.GetGLTFData();
            ObjectExtrasAnimationClip.AddGLTFDataToExtras();
            ObjectExtrasAnimationController.AddGLTFDataToExtras();
            ObjectExtensionOmiAudio.AddGLTFDataToExtensions();
            //OmiCollider is only present in nodes
            result += ObjectMasterExtras.GetGLTFData();//new
            result += ObjectExtension.GetGLTFData();//new
            result += ObjectAsset.GetGLTFData(false) + "\n";
            result += "}";

            if (options.reduceGLTFChars)
                result = StringUtilities.RemoveExtraChars(result);

            FileExporter.ExportToText(result, file_name, file_location, "gltf");

            List<int> images_quality = options.GetImagesQuality();
            ObjectImage.ExportAllImages(file_location, images_quality[0], images_quality[1], images_quality[2], images_quality[3]);
            ObjectExtensionOmiAudio.ExportAllAudios(file_location);
        }


        public static void ClearListsAndReset()
        {
            ObjectBuffer.Reset();
            ObjectBufferView.Reset();
            ObjectAccessors.Reset();
            ObjectMaterial.Reset();
            ObjectTexture.Reset();
            ObjectExtrasCubeTextures.Reset();
            ObjectSampler.Reset();
            ObjectImage.Reset();
            ObjectMesh.Reset();
            MeshHolder.Reset();
            UVVariant.Reset();
            ObjectSkin.Reset();
            ObjectScene.Reset();
            ObjectCamera.Reset();//new
            ObjectExtrasAnimationClip.Reset();
            ObjectExtrasAnimationController.Reset();
            ObjectExtensionOmiAudio.Reset();
            ObjectMasterExtras.Reset();//new
            ObjectExtension.Reset();//new
            ObjectNodeMono.ResetValues();

        }
    }
}
