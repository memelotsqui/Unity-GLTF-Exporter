using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace WEBGL_EXPORTER
{
    public class FileExporter : MonoBehaviour
    {
        public static string ExportToJPEG(Texture2D texture, string name, string path,int quality = 75, bool invertWidth = false, bool invertHeight = false)
        {
            if (quality > 100)
                quality = 100;
            if (quality < 1)
                quality = 1;
            ImageGenerator.MakeReadableTexture(texture);
            texture = ImageGenerator.InvertTexture(texture, invertWidth, invertHeight);
            byte[] textureInfo = ImageConversion.EncodeToJPG(texture,quality);
            // change to path variable
            File.WriteAllBytes(path + "/" + name + ".jpg", textureInfo);

            return path + "/" + name + ".jpg";
        }

        public static void ExportToPNG(Texture2D texture, string name, string path,bool overwrite = true,bool debugWarning = true)
        {
            if (texture != null)
            {
                string pathToSave = path + "/" + name + ".png";
                if (overwrite || !File.Exists(pathToSave))
                {

                    ImageGenerator.MakeReadableTexture(texture);
                    byte[] textureInfo = ImageConversion.EncodeToPNG(texture);
                    // change to path variable

                    

                    File.WriteAllBytes(path + "/" + name + ".png", textureInfo);
                }
                else
                {
                    if (debugWarning)
                        Debug.LogWarning("file in: " + pathToSave + " exists, skipping");
                }
            }
        }
        public static void ExportToEXR(Texture2D texture, string name, string path, Texture2D.EXRFlags flags = Texture2D.EXRFlags.CompressZIP)
        {
            ImageGenerator.MakeReadableTexture(texture,false);
            byte[] textureInfo = ImageConversion.EncodeToEXR(texture,flags);
            File.WriteAllBytes(path + "/" + name + ".exr", textureInfo);
            //DestroyImmediate(texture);

        }
        public static string ExportToText(string tarText, string name, string path, string ext = "txt")
        {
            if (!path.EndsWith("/"))
                path += "/";
            File.WriteAllText(path + name + "." + ext, tarText);
            return path + name + "." + ext;
        }
        public static void ReplaceStringInFile(string filePath, string stringToReplace, string replaceString)
        {
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                text = text.Replace(stringToReplace, replaceString);
                string fileName = StringUtilities.GetFileNameFromPath(filePath);
                string extension = StringUtilities.GetExtensionFromFile(filePath);
                string path = StringUtilities.GetFolderNameFromFile(filePath);
                Debug.Log(text);
                ExportToText(text,fileName,path,extension);
            }
            else
            {
                Debug.Log(filePath);
            }
        }
        public static void DuplicateFolder(string sourceDirName, string destDirName, bool copySubDirs = true, string[] fileExtensions = null)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                Debug.Log(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            else
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Extension != ".meta")
                    {
                        if (fileExtensions == null)
                        {
                            string temppath = Path.Combine(destDirName, file.Name);
                            file.CopyTo(temppath, true);
                        }
                        else
                        {
                            bool hasExtension = false;
                            foreach (string st in fileExtensions)
                            {
                                if (file.Extension == ("." + st))
                                {
                                    hasExtension = true;
                                    break;
                                }
                            }
                            if (hasExtension)
                            {
                                string temppath = Path.Combine(destDirName, file.Name);
                                file.CopyTo(temppath, true);
                            }
                        }
                    }
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DuplicateFolder(subdir.FullName, temppath, copySubDirs, fileExtensions);
                    }
                }
            }
        }
        public static void CopyFileFromAsset(Object obj, string location, string name, bool overwrite = false)
        {
            string origPath = AssetDatabase.GetAssetPath(obj);
            string ext = "." + StringUtilities.GetExtensionFromFile(origPath);
            location = (location.EndsWith("/")|| location.EndsWith("\\")) ? location : location + "/";
            location += name + ext;
            CopyFile(origPath,location,overwrite);
        }
        public static void CopyFile(string sourceFile, string destFile,bool overwrite = false)
        {
            File.Copy(sourceFile, destFile, overwrite);
        }
        public static string DuplicateFile(string sourceFile, int countModif = 0)
        {
            if (File.Exists(sourceFile))
            {
                string[] fileSplit = sourceFile.Split('.');
                string destFile = fileSplit[0] + "_" + countModif.ToString() + "." + fileSplit[1];
                if (File.Exists(destFile))
                {
                    Debug.Log("not copying file");
                    return DuplicateFile(sourceFile, countModif+1);
                }
                else
                {
                    Debug.Log("copying file");
                    File.Copy(sourceFile, destFile);
                    return destFile;
                }
            }
            else
            {
                return "";
            }
        }
        public static void DeleteFile(string tarPath)
        {
            if (File.Exists(tarPath))
            {
                File.Delete(tarPath);
            }
        }
        public static string CreateFolder(string folderFullPath)
        {
            if (!Directory.Exists(folderFullPath))
            {
                Directory.CreateDirectory(folderFullPath);
                return folderFullPath;
            }
            return "";
        }
        public static int GetFilesQty(string folderFullPath)
        {
            if (Directory.Exists(folderFullPath))
            {
                return Directory.GetFiles(folderFullPath).Length;
            }
            return 0;
        }
        public static bool DirectoryExists(string folderFullPath)
        {
            if (Directory.Exists(folderFullPath))
            {
                return true;
            }
            return false;
        }
    }
}
