using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

namespace WEBGL_EXPORTER
{
    public class PrefabSelectionWindow : EditorWindow
    {
        public delegate void SetPrefab(GameObject selectedObject);
        protected SetPrefab callbackFunction;

        public List<Texture2D> previewPrefabs;
        public List<string> prefabFileList;
        public List<string> prefabName;

        public List<string> filterPrefabFileList;
        public List<string> filterPrefabName;
        public List<Texture2D> filterPreviewPrefabs;

        public List<string> filterOptions;

        public string currentPrefabName;
        //public string currentPrefabFile;
        public Texture2D currentPreviewPrefab;
        public GameObject selectedPrefab;

        public Vector2 buttonSize;
        public int buttonsRow;

        public string searchOption = "";

        public Vector2 scrollPosition = Vector2.zero;

        public static Texture2D GetAssetPreviewTexture(GameObject go)
        {
            //Debug.Log(go.name);
            Texture2D result = AssetPreview.GetAssetPreview(go);
            while (result == null)
            {

                result = AssetPreview.GetAssetPreview(go);

                Thread.Sleep(5);
            }
            return AssetPreview.GetAssetPreview(go); ;

        }
        public PrefabSelectionWindow(string relativeFolderLocation, SetPrefab callback, int buttons_row, Vector2 button_size, Vector2 min_window_size, string window_title,bool getSubdirectories = false, bool getFromThumbImage = false, string mustStartWith = "")
        {
            previewPrefabs = new List<Texture2D>();
            prefabFileList = new List<string>();
            prefabName = new List<string>();

            filterOptions = new List<string>();

            selectedPrefab = null;

            scrollPosition = Vector2.zero;
            callbackFunction = callback;
            buttonSize = button_size;
            buttonsRow = buttons_row;
            string[] allFiles = StringUtilities.GetFilesPathFromFolder("Assets/" + relativeFolderLocation, true, "", true, getSubdirectories);
            foreach (string st in allFiles)
            {
                
                if (st.EndsWith(".prefab"))
                {
                    if (StringUtilities.GetFileNameFromPath(st).StartsWith(mustStartWith))              // IF THE FILE STARTS WITH THE STRING PROVIDED, ADD IT TO THE LIST
                    {
                        // GET ALL THE PREVIEW IMAGES FROM THE REALITVE FOLDER LOCATION
                        prefabFileList.Add(st);
                        prefabName.Add(StringUtilities.GetFileNameFromPath(st));
                        Texture2D txt;
                        if (!getFromThumbImage)
                        {
                            txt = GetAssetPreviewTexture((GameObject)AssetDatabase.LoadAssetAtPath(st, typeof(GameObject)) as GameObject);
                        }
                        else
                        {
                            string thumbFile = StringUtilities.RemoveExtensionFromFile(st)+"_thumb.png";
                            txt = AssetDatabase.LoadAssetAtPath(thumbFile, typeof(Texture2D)) as Texture2D;
                        }
                        previewPrefabs.Add(txt);

                        filterPreviewPrefabs = new List<Texture2D>(previewPrefabs);
                        filterPrefabFileList = new List<string>(prefabFileList);
                        filterPrefabName = new List<string>(prefabName);
                    }

                }

            }
            //while (AssetPreview.IsLoadingAssetPreviews()) { }
            
            PrefabSelectionWindow window = (PrefabSelectionWindow)EditorWindow.GetWindow(typeof(PrefabSelectionWindow), true, window_title);
            window.minSize = min_window_size;
            window.title = window_title;
        }
        public void FilterOption(List<string> searchFilter, string searchOption = "")
        {
            filterPrefabFileList.Clear();
            filterPreviewPrefabs.Clear();
            filterPrefabName.Clear();

            string[] splitSearch = searchOption.Split(' ');
            for (int i = 0; i < prefabName.Count; i ++)
            {
                bool add = true;
                foreach (string stFilter in searchFilter)       // check if prefab name has the name filters
                {

                    if (!prefabName[i].ToLower().Contains (stFilter.ToLower())) {  // if it does not contain at least 1 of the filters in the list, break and dont add it
                        add = false;
                        break;
                    }
                }


                if (add)
                {
                    foreach (string stSearch in splitSearch) 
                    {
                        if (!prefabName[i].ToLower().Contains(stSearch))      // if prefab name does not contains the searchoption string, dont add it to the list
                        {
                            add = false;
                        }
                    }
                }

                if (add)
                {
                    filterPrefabFileList.Add(prefabFileList[i]);
                    filterPrefabName.Add(prefabName[i]);
                    filterPreviewPrefabs.Add(previewPrefabs[i]);
                }
            }
        }

        public virtual void OnGUI()
        {
            /*            GuiLayoutExtras.CenterLabel("Select Room",30);
                        GuiLayoutExtras.CenterLabel("", 10);*/

            GuiLayoutExtras.CenterLabel("", 10);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.Label("     Search option:",GUILayout.Height(25f));
            searchOption = EditorGUILayout.TextField(searchOption, GUILayout.Height(25f));
            
            if (EditorGUI.EndChangeCheck())
                FilterOption(filterOptions, searchOption);
/*            if (GUILayout.Button("Search", GUILayout.Height(25f)))
            {
                FilterOption(filterOptions,searchOption);
            }*/
            EditorGUILayout.EndHorizontal();

            GuiLayoutExtras.CenterLabel("", 10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(currentPreviewPrefab, GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x)))
            {
                
                if (selectedPrefab != null)
                {
                    callbackFunction(selectedPrefab);
                }
            }
            GUILayout.Label(currentPrefabName,GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x*2));
            EditorGUILayout.EndHorizontal();

            GuiLayoutExtras.CenterLabel("", 10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < filterPreviewPrefabs.Count; i++)
            {
                if (i % buttonsRow == 0)
                    EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(filterPreviewPrefabs[i], GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x)))
                {
                    selectedPrefab = AssetDatabase.LoadAssetAtPath(filterPrefabFileList[i], typeof(GameObject)) as GameObject;
                    currentPrefabName = filterPrefabName[i];
                    //currentPrefabFile = filterPrefabFileList[i];
                    currentPreviewPrefab = filterPreviewPrefabs[i];
                    callbackFunction(selectedPrefab);
                }
                //EditorGUILayout.LabelField(filterPrefabName[i], GUILayout.Width(buttonSize.x), GUILayout.Height(50));
               // GUILayout.LabelField
                EditorGUILayout.EndVertical();

                if (i == filterPreviewPrefabs.Count - 1 || i % buttonsRow == buttonsRow - 1)     // IF ITS THE LAST ELEMENT, OR IF IT IS THE LAST IN THE ROW
                    EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
