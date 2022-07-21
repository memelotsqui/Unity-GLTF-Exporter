using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER
{
    public class MaterialThumbGenWindow : EditorWindow
    {
        Object targetFolder;
        bool getMaterials = false;
        bool getPrefabs = false;
        bool overwriteExistingThumbs = true;


        // Add menu item named "My Window" to the Window menu
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MaterialThumbGenWindow));
        }

        void OnGUI()
        {
            GUILayout.Label("Folder Location", EditorStyles.boldLabel);
            targetFolder = (Object)EditorGUILayout.ObjectField("Parent folder", targetFolder, (typeof(Object)));
            overwriteExistingThumbs = EditorGUILayout.Toggle("Overwrite if exists?", overwriteExistingThumbs);
            getMaterials = EditorGUILayout.Toggle("Get Materials", getMaterials);
            getPrefabs = EditorGUILayout.Toggle("Get Prefabs", getPrefabs);
            if (GUILayout.Button("Create Thumbnails", GUILayout.Height(80f)))
            {

                //CreateThumbImages.CreateMaterialThumbnailsFromFolderObject(targetFolder,overwriteExistingThumbs);
                if (getMaterials)
                {
                    CreateThumbImages.CreateThumbnailsFromFolderObject(targetFolder, "mat", typeof(Material), overwriteExistingThumbs);
                }
                if (getPrefabs)
                {
                    CreateThumbImages.CreateThumbnailsFromFolderObject(targetFolder, "prefab", typeof(GameObject), overwriteExistingThumbs);
                }
            }
        }
    }
}
