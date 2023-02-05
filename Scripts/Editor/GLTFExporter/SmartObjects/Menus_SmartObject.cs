using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System;

namespace WEBGL_EXPORTER.GLTF.SMART
{
    
    public class Menus_SmartObjects : Editor
    {
        [MenuItem("Assets/Create/Smart Script", false, 70)]
        private static void CreateNewAsset()
        {
            CreateAssetInCurrentFolder<TextAsset>("NewSmartScript");
        }

        public static void CreateAssetInCurrentFolder<T>(string initialAssetName, Action<T> onCreated = null, Action onCanceled = null)
        where T : TextAsset
        {
            // Process the asset name:
            if (string.IsNullOrWhiteSpace(initialAssetName))
                initialAssetName = "New " + ObjectNames.NicifyVariableName(typeof(T).Name);

            const string requiredExtension = ".jsmart";

            if (!initialAssetName.EndsWith(requiredExtension, StringComparison.InvariantCultureIgnoreCase))
                initialAssetName += requiredExtension;


            // Set up the end name edit action callback object:
            var endNameEditAction = CreateInstance<AssetCreatorEndNameEditAction>();

            endNameEditAction.canceledCallback = onCanceled;

            if (onCreated != null)
                endNameEditAction.createdCallback = (_instance) => onCreated((T)_instance);

            TextAsset text = new TextAsset();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(text.GetInstanceID(), endNameEditAction, initialAssetName, AssetPreview.GetMiniThumbnail(text), null);
        }

        private class AssetCreatorEndNameEditAction : EndNameEditAction
        {
            public Action<UnityEngine.Object> createdCallback;
            public Action canceledCallback;


            ///          
            /// <inheritdoc/>
            ///          

            public override void Action(int instanceId, string pathName, string resourceFile)
            {

                string savePath = StringUtilities.GetFullPathFromLocalPath(StringUtilities.GetFolderNameFromFile(pathName));
                string className = getClassName(pathName);
                string extension = StringUtilities.GetExtensionFromFile(pathName);

                FileExporter.ExportToText(getInitialString(getClassName(pathName)), className, savePath, extension);
                AssetDatabase.Refresh();

            }


            ///          
            /// <inheritdoc/>
            ///          

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Selection.activeObject = null;

                canceledCallback?.Invoke();
            }
        }

        private static string getInitialString(string className)
        {
            return "class " + className + " extends ObjectComponent{\n" +
                "\tconstructor(){\n" +
                "\t\t\n" +
                "\t}\n" +
                "}";
        }
        private static string getClassName(string path)
        {
            string[] splitPath = path.Split('.')[0].Replace('\\','/').Split('/');
            return splitPath[splitPath.Length - 1];
        }
    }
}
