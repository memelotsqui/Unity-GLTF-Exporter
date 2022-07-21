using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace WEBGL_EXPORTER.GLTF.SMART
{
    public class Menus_SmartObjects : Editor
    {
        //private string 
        [MenuItem("Assets/Create/Smart Script", false, 70)]
        private static void CreateNewAsset()
        {
            //string filePath = AssetDatabase.GenerateUniqueAssetPath(GetSelectedPathOrFallback() + "NewFile.ext");
            //ProjectWindowUtil.CreateAssetWithContent(filePath, filePath);
            //EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;
            //AskUserInputWindow.CreateInstance("Class Name", "Class Name", callStr);
            ProjectWindowUtil.CreateAssetWithContent(
                "Default Name.js",
                string.Empty);

            //ProjectWindowUtil.CreateScriptAssetFromTemplateFile()
        }
        private static void ProjectWindowItemCallback(string guid, Rect selectionRect)
        {
            Debug.Log("called");
            bool keepgoing = true;
            float curTime = 0f;
            while (keepgoing && curTime < 10f)
            {
                curTime += Time.deltaTime;
                //Debug.Log(curTime);
                //Debug.Log("called2");
                Debug.Log(Event.current);
            }
            //yield return null;

            //if (Event.current.type == EventType.MouseDrag)
            //{
            //    Event.current.Use();
            //}
        }
        //static IEnumerator checkevent()
        //{
            
        //}
        static void callStr(string str)
        {
            Debug.Log(str);
        }
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path + "/";
        }
    }
}
