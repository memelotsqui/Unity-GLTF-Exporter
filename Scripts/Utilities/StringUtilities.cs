using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
namespace WEBGL_EXPORTER
{
    public class StringUtilities : MonoBehaviour
    {
        public static string RemoveExtraChars(string code)
        {
            string result = code;
            result = result.Replace("\n","");
            result = result.Replace(" ", "");

            return result;
        }
        public static List<string> GetNonRepeatingStringList(string addString, List<string> existingList = null)
        {
            List<string> result;
            if (existingList == null)
            {
                result = new List<string>();
            }
            else
            {
                result = existingList;
            }
            // MAKE SURE WERE NOT ADDING AN EMPTY STRING TO THE ARRAY
            if (addString != "")
            {
                bool repeat = false;
                foreach (string s in result)
                {
                    // IF WE FIND THE STRING ALREADY EXISTS, WE BREAK AND DONT ADD THIS STRING TO THE LIST
                    if (s == addString)
                    {
                        repeat = true;
                        break;
                    }
                }
                if (!repeat)
                {
                    result.Add(addString);
                }
            }
            return result;
        }
        public static string RemoveCharacterFromString(string tarString, int quantity, bool initChars = true)
        {
            string result = tarString;
            if (initChars)
            {
                result = result.Substring(quantity);
            }
            else
            {
                if ((result.Length - quantity) >= 0)
                {
                    result = result.Substring(0, result.Length - quantity);
                }
                else
                {
                    Debug.LogWarning("Error while trying to remove last chars from string, please check");
                }
            }
            return result;
        }
        //public static string Vector3ToGLTFString(Vector3 vector3)
        //{
        //    return (-vector3.x).ToString() + "," + vector3.z.ToString() + "," + (-vector3.y).ToString();
        //}
        public static string Vector3ToString(Vector3 vector3, bool multiply_static, float x_modif = 1f, float y_modif =1f, float z_modif =1f)
        {
            Vector3 finalVector3 = CleanVector3(vector3);
            if (multiply_static)
                finalVector3 = new Vector3(vector3.x * StaticVariables.THREEPositionModifier.x, vector3.y * StaticVariables.THREEPositionModifier.y, vector3.z * StaticVariables.THREEPositionModifier.z);
            //if (negativeZ)
                //finalVector3 = new Vector3(finalVector3.x, finalVector3.y, finalVector3.z);
            return (finalVector3.x * x_modif).ToString() + "," + (finalVector3.y * y_modif).ToString() + "," + (finalVector3.z * z_modif).ToString();
        }
        public static string QuaternionToString(Quaternion quaternion, bool unity_to_threejs_quaternion = false)
        {
            quaternion = CleanQuaternion(quaternion);   //make siure to remove exponentional floats
            if (!unity_to_threejs_quaternion)  
                return quaternion.x.ToString() + "," + quaternion.y.ToString() + "," + quaternion.z.ToString() + "," + quaternion.w.ToString();
            else
                return quaternion.w.ToString() + "," + quaternion.x.ToString() + "," + quaternion.y.ToString() + "," + quaternion.z.ToString();
        }
        private static Vector3 CleanVector3(Vector3 tar_vector3)
        {
            return new Vector3(CleanFloat(tar_vector3.x), CleanFloat(tar_vector3.y), CleanFloat(tar_vector3.z));
        }
        private static Quaternion CleanQuaternion(Quaternion tar_quaternion)
        {
            return new Quaternion(CleanFloat(tar_quaternion.x), CleanFloat(tar_quaternion.y), CleanFloat(tar_quaternion.z), CleanFloat(tar_quaternion.w));
        }
        private static float CleanFloat(float tar_float)
        {
            if (tar_float < 0.00001f && tar_float > -0.00001f)
            {
                return 0;
            }
            return tar_float;
        }
        public static string Vector2ToString(Vector2 vector2, bool THREEVector2)
        {
            Vector2 finalVector2 = vector2;
            if (THREEVector2)
                finalVector2 = new Vector3(vector2.x * StaticVariables.THREEPositionModifier.x, vector2.y * StaticVariables.THREEPositionModifier.y);
            return finalVector2.x.ToString() + "," + finalVector2.y.ToString();
        }
        public static string Vector3RotationToString(Vector3 vector3)   //CHECAR!!!, NO ESTOY SEGURO SI LOS 3 VALORES SON NEGATIVOS!!
        {
            Vector3 finalVector3 = new Vector3(-(vector3.x * 3.1416f / 180f), -(vector3.y * 3.1416f / 180f), -(vector3.z * 3.1416f / 180f));
            return finalVector3.x.ToString() + "," + finalVector3.y.ToString() + "," + finalVector3.z.ToString();

        }
        public static string FloatRotationToThreeString(float value)
        {
            return (value * 3.1416f / 180f).ToString();
        }
        public static string DegreeToRadians(float degree) 
        {
            return (degree * 3.1416f / 180f).ToString("F3");
        }

        //ARRAYS TO STRING
        public static string ArrayToString(int[] int_values)
        {
            string result = "";
            if (int_values != null)
            {
                foreach(int i in int_values)
                {
                    result += i + ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }
        public static string ArrayToString(List <int> int_values)
        {
            string result = "";
            if (int_values != null)
            {
                foreach (int i in int_values)
                {
                    result += i + ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }
        public static string ArrayToString(float[] float_values)
        {
            string result = "";
            if (float_values != null)
            {
                foreach (float f in float_values)
                {
                    result += f + ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }
        public static string ArrayToString(List <float> float_values)
        {
            string result = "";
            if (float_values != null)
            {
                foreach (float f in float_values)
                {
                    result += f + ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }
        public static string ArrayToString(List <string> string_values, bool with_quotes = true)
        {
            string result = "";
            if (string_values != null)
            {
                foreach (string s in string_values)
                {
                    if (with_quotes) result += "\"";
                    result += s;
                    if (with_quotes) result += "\"";
                    result += ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }
        public static string GetSafeChars(string tarString)
        {
            string result = tarString.Replace("\\","\\\\").Replace("\"", "\\\"");
            return result;
        }
        public static string ArrayToString(string[] string_values, bool with_quotes = true)
        {
            string result = "";
            if (string_values != null)
            {
                foreach (string s in string_values)
                {
                    if (with_quotes) result += "\"";
                    result += s;
                    if (with_quotes) result += "\"";
                    result += ",";
                }
                result = RemoveCharacterFromString(result, 1, false);
            }
            return result;
        }

        //END ARRAYS TO STRING

        public static string GetRandomUuid()
        {
            string result = "";
            for (int i = 0; i < 8; i++)
            {
                result += GetRandomChar();
            }
            result += "-";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result += GetRandomChar();
                }
                result += "-";
            }
            for (int i = 0; i < 12; i++)
            {
                result += GetRandomChar();
            }
            return result;
        }
        public static char GetRandomChar()
        {
            string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char c = st[Random.Range(0, st.Length)];
            return c;
        }
        public static string GetSeparatedString(string targetString, string separationString, bool getFirstString = true)
        {
            string[] splitCode = targetString.Split(new string[] { separationString }, System.StringSplitOptions.None);
            if (getFirstString)
            {
                return splitCode[0];
            }
            else
            {
                return splitCode[splitCode.Length - 1];
            }
        }

        public static string GetFolderNameFromSubfolder(string tarPath)
        {
            string correctPath = tarPath.Replace("\\","/");
            if (correctPath.EndsWith("/"))
            {
                correctPath = StringUtilities.RemoveCharacterFromString(correctPath, 1, false);
            }
            return StringUtilities.GetSeparatedString(correctPath, "/", false);
        }
        public static string GetFolderNameFromFile(string tarPath)
        {
            return Path.GetDirectoryName(tarPath);
        }
        public static string GetFileNameFromPath(string tarPath)
        {
            string correctPath = tarPath.Replace("\\", "/");
            string fileName = StringUtilities.GetSeparatedString(correctPath, "/", false);
            return StringUtilities.GetSeparatedString(fileName,".");
        }
        public static string GetFullPathFromAsset(Object tarObject)
        {
            string result = "";
            result = Application.dataPath + StringUtilities.RemoveCharacterFromString(AssetDatabase.GetAssetPath(tarObject), 6);
            //Debug.Log(result);
            return result;
        }
        public static string[] GetSubfoldersPathsFromFolder(string tarFolder, bool isLocal = true, bool returnLocalFolders = true, bool getSubfolders = false)
        {
            if (isLocal)
                tarFolder = StringUtilities.GetFullPathFromLocalPath(tarFolder);

            SearchOption so = SearchOption.TopDirectoryOnly;
            if (getSubfolders)
                so = SearchOption.AllDirectories;
            string[] result = Directory.GetDirectories(tarFolder, "*", so);
            if (returnLocalFolders)
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = "Assets" + result[i].Substring(Application.dataPath.Length);
                }

            return result;
        }
        public static string[] GetFilesPathFromFolder(string tarFolder, bool isLocal, string extension = "",bool returnLocalFolder = false ,bool getFilesFromSubdirectories = false)
        {
            string searchLocation = tarFolder;
/*            if (!searchLocation.EndsWith("/"))
                searchLocation += "/";*/
            if (isLocal)
                searchLocation = Application.dataPath + StringUtilities.RemoveCharacterFromString(tarFolder, 6);

            if (Directory.Exists(searchLocation))
            {
                string[] allFilesPath;
                if (getFilesFromSubdirectories)
                    allFilesPath = Directory.GetFiles(searchLocation, "*" + extension + "*", SearchOption.AllDirectories);
                else
                    allFilesPath = Directory.GetFiles(searchLocation, "*" + extension + "*", SearchOption.TopDirectoryOnly);

                if (extension != "")
                {
                    List<string> filesList = new List<string>();
                    foreach (string st in allFilesPath)
                    {
                        if (st.ToLower().EndsWith("." + extension.ToLower()))
                            filesList.Add(st);
                    }

                    string[] result = new string[filesList.Count];
                    for (int i = 0; i < filesList.Count; i++)
                    {
                        if (returnLocalFolder)
                            result[i] = "Assets" + StringUtilities.GetSeparatedString(filesList[i], Application.dataPath, false);
                        else
                            result[i] = filesList[i];
                    }
                    return result;
                }
                else
                {
                    if (returnLocalFolder)
                    {
                        for (int i = 0; i < allFilesPath.Length; i++)
                        {
                            allFilesPath[i] = "Assets" + StringUtilities.GetSeparatedString(allFilesPath[i], Application.dataPath, false);
                        }
                    }

                    return allFilesPath;
                }
            }
            else
            {
                Debug.Log("directory does not exists: " + tarFolder);
                return null;
            }
        }
        /// <summary>
        /// Returns the local path starting with "Assets"
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetLocalPathFromFullPath(string fullPath)
        {
            string result = "Assets" + StringUtilities.GetSeparatedString(fullPath, Application.dataPath, false);
            return result;
        }
        public static string GetFullPathFromLocalPath(string localPath)
        {
            //string result = "Assets" + StringUtilities.GetSeparatedString(fullPath, Application.dataPath, false);
            string result = StringUtilities.RemoveCharacterFromString(Application.dataPath,6,false) + localPath;
            return result;
        }
        public static string GetExtensionFromFile(string filePath)
        {
            if (!filePath.Contains("."))
                return "";
            return GetSeparatedString(filePath, ".",false);
        }
        public static string GetExtensionFromObjectAsset(Object obj)
        {
            
            if (obj == null)
                return "";

            return GetSeparatedString(AssetDatabase.GetAssetPath(obj), ".", false);
        }
        public static string RemoveExtensionFromFile(string filePath)
        {
            return StringUtilities.GetSeparatedString(filePath, ".", true);
        }
        public static string GetParentFolderPathFromPath(string tarPath)
        {
            string correctPath = tarPath.Replace("\\", "/");
            if (correctPath.EndsWith("/"))
                correctPath = StringUtilities.RemoveCharacterFromString(correctPath, 1, false);

            string[] splitStringResult = correctPath.Split('/');
            string result = "";
            for(int i = 0; i < splitStringResult.Length - 1; i++)
            {
                result += splitStringResult[i];
                if (i != splitStringResult.Length - 2)
                    result += "/";
            }
            return result;
        }
        // require objects
        public static string GetCurrentScenePath(bool localpath = true,bool removeExtension = false, bool removeAssetsString = false)
        {
            string result = SceneManager.GetActiveScene().path;
            if (removeExtension)
                result = RemoveCharacterFromString(result, 6,false);  // remove .unity extension (".unity" = 6 characters)
            if (!localpath)
                return GetFullPathFromLocalPath(result);        // if a full path is required, we cant remove "Assets/" string

            if (removeAssetsString)
                result = RemoveCharacterFromString(result, 7);  // remove .unity extension ("assets/" = 7 characters)
            return result;
        }
        public static string GetStringJsonLegal(string tarString)
        {
            char[] replaceChars = { '\\', '`','\'', 'ñ','"' };
            foreach (char c in replaceChars)
            {
                tarString = tarString.Replace(c, '_');
            }
            return tarString.ToLower();
        }
        public static string GetStringWithLegalCharacters(string tarString)
        {
            char[] replaceChars = { '\\', '`', '*', '{', '}', '[', ']', '(', ')', '>', '#', '+', '-', '.', '!', '$', '\'',' ', 'ñ',';',':','/',',','@','"'};
            foreach (char c in replaceChars)
            {
                tarString = tarString.Replace(c, '_');
            }
            return tarString.ToLower();
        }

    }
}
