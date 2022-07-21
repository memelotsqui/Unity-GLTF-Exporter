using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
namespace WEBGL_EXPORTER
{
    public class PrefabSOOptions : ScriptableObject     // SO STANDS FOR SCRIPTABLE OBJECT
    {
        public List<Texture2D> previewPrefabs;
        public List<string> prefabFileList;
        public List<string> prefabName;

        public static void CreatePrefabSOOptions(string relativeFolder,string assetName, bool getSubdirectories)
        {
            // MAKE SURE IS A VALID RELATIVE FOLDER, MUST START WITH ASSETS/
            string validRelativeFolder = relativeFolder;
            if (!validRelativeFolder.StartsWith("Assets/"))
                validRelativeFolder = "Assets/" + validRelativeFolder;

            // MAKE SURE DIRECTORY EXISTS, IF NOT, SKIP ALL AND MAKE A WARNING
            if (Directory.Exists(StringUtilities.GetFullPathFromLocalPath(validRelativeFolder)))
            {

                // CREATE THE SCRIPTABLE OBJECT
                PrefabSOOptions asset = ScriptableObject.CreateInstance<PrefabSOOptions>();

                // ASSIGN VARIABLE VALUES
                asset.previewPrefabs = new List<Texture2D>();
                asset.prefabFileList = new List<string>();
                asset.prefabName = new List<string>();
                string[] allFiles = StringUtilities.GetFilesPathFromFolder(validRelativeFolder, true, "", true, getSubdirectories);
                foreach (string st in allFiles)
                {

                    if (st.EndsWith(".prefab"))
                    {
                        asset.prefabFileList.Add(st);
                        asset.prefabName.Add(StringUtilities.GetFileNameFromPath(st));

                        // THUMBNAIL MUST EXISTS ALREADY
                        string thumbFile = StringUtilities.RemoveExtensionFromFile(st) + "_thumb.png";
                        asset.previewPrefabs.Add(AssetDatabase.LoadAssetAtPath(thumbFile, typeof(Texture2D)) as Texture2D);
                        //asset.previewPrefabs.Add(CreateThumbImages.GetAssetPreviewTexture((GameObject)AssetDatabase.LoadAssetAtPath(st, typeof(GameObject)) as GameObject));
                    }

                }

                // FINISH ASSET CREATION
                AssetDatabase.CreateAsset(asset, validRelativeFolder + assetName + ".asset");
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogError("directory: " + validRelativeFolder + "  could not be found, skipping asset creation");
            }
        }
    }
}
