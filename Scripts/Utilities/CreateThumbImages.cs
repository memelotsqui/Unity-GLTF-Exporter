using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.IO;

namespace WEBGL_EXPORTER
{
    public class CreateThumbImages
    {
        public static void CreateThumbnailsFromFolderObject(string targetFolder, string extension, System.Type type, bool overwrite = true)
        {
            string target_folder_correct = targetFolder;
            if (!target_folder_correct.StartsWith("Assets/"))
                target_folder_correct = "Assets/"+target_folder_correct; 
            if (Directory.Exists(StringUtilities.GetFullPathFromLocalPath(target_folder_correct)))
            {
                CreateThumbnails(target_folder_correct, extension, type, overwrite);
            }
            else
            {
                Debug.LogError("Thumbnail folder does not exists, please check");
            }
        }
        public static void CreateThumbnailsFromFolderObject(Object targetFolder,string extension,System.Type type, bool overwrite = true)
        {
            if (targetFolder != null)
            {
                CreateThumbnails(AssetDatabase.GetAssetPath(targetFolder),extension,type, overwrite);
            }
        }
        public static void CreateThumbnails(string targetPath, string extension, System.Type type, bool overwrite = true)
        {
            string[] pathLocations = StringUtilities.GetFilesPathFromFolder(targetPath, true, extension, true, true);
            List<string> destinationList = new List<string>();          // used to later set labels for the images created
            foreach (string st in pathLocations)
            {
                
                var thumbMat = AssetDatabase.LoadAssetAtPath(st, type);
                Texture2D newTexture = GetThumbnailPreview(thumbMat);
                if (newTexture != null)
                {
                    string destination = Path.GetDirectoryName(st);
                    string thumbName = StringUtilities.GetFileNameFromPath(st) + "_thumb";
                    string finalDestination = destination + "/" + thumbName + ".png";
                    destinationList.Add(finalDestination);
                    if (overwrite || !File.Exists(finalDestination))
                    {
                        FileExporter.ExportToPNG(newTexture, thumbName, destination, overwrite);
                    }
                }
            }

            AssetDatabase.Refresh();

            foreach(string st in destinationList)
            {
                Texture2D obj = (Texture2D)AssetDatabase.LoadAssetAtPath(st, typeof(Texture2D));
                
                string[] stAllLabels = AssetDatabase.GetLabels(obj);
                bool addLabel = true;
                foreach (string stlb in stAllLabels)
                {
                    if (stlb == "Thumb")
                    {
                        addLabel = false;
                        break;
                    }
                }
                if (addLabel)
                {
                    string[] newLabels = new string[stAllLabels.Length + 1];
                    for (int i = 0; i < stAllLabels.Length; i++)
                    {
                        newLabels[i] = stAllLabels[i];
                    }
                    newLabels[newLabels.Length - 1] = "Thumb";
                    AssetDatabase.SetLabels(obj, newLabels);
                }
            }
        }
        public static void CreateThumbnail(string targetPath, System.Type type, bool overwrite = true)
        {
            var thumbObj = AssetDatabase.LoadAssetAtPath(targetPath, type);
            Texture2D newTexture = GetThumbnailPreview(thumbObj);
            if (newTexture != null)
            {
                string destination = Path.GetDirectoryName(targetPath);
                string thumbName = StringUtilities.GetFileNameFromPath(targetPath) + "_thumb";
                FileExporter.ExportToPNG(newTexture, thumbName, destination, overwrite);
            }
            AssetDatabase.Refresh();
        }
        public static Texture2D GetThumbnailPreview(Object targetObject)
        {

            if (targetObject != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(targetObject);
                while (AssetPreview.IsLoadingAssetPreview(targetObject.GetInstanceID()))
                {
                    previewTexture = AssetPreview.GetAssetPreview(targetObject);
                    Thread.Sleep(30);
                }
                return previewTexture;
            }
            return null;
        }


    }
    

}
