using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace WEBGL_EXPORTER
{
    public class PrefabSOWindow : EditorWindow
    {
        // CALLBACK FUNCTION
        public delegate void SetPrefab(GameObject selectedObject);
        protected SetPrefab callbackFunction;

        // SCRIPTABLE OBJECT SAVE TEXTURE OPTIONS
        public PrefabSOOptions options;

        // DISPLAY WINDOW SETUP
        public Vector2 buttonSize;
        public Vector2 minWindowSize;
        public int buttonsRow;
        public Vector2 scrollPosition;
        string windowTitle;
        string mustStartWith = "";

        // USER DEFINED ACTIONS ON INTERACTION
        public string searchOption;
        public List<int> filterListValues;
        public List<string> filterOptions;
        public int currentPrefabIndex;
        public GameObject selectedPrefab;


        public void UpdateOptions(PrefabSOOptions prefabOptions, SetPrefab callback, Vector2 button_size, Vector2 min_window_size, string window_title, int buttons_row,string must_start_with = "", string existingFilter = "",int currentIndex = 0)
        {
            options = prefabOptions;
            /*            if (existingFilterList != null)
                            filterListValues = existingFilterList;
                        else
                            filterListValues = new List<int>();*/
            filterListValues = new List<int>();
            mustStartWith = must_start_with;
            currentPrefabIndex = currentIndex;
            buttonSize = button_size;
            buttonsRow = buttons_row;
            callbackFunction = callback;
            minWindowSize = min_window_size;
            windowTitle = window_title;
            searchOption = existingFilter;
        }
        public void ShowWindow()
        {
            PrefabSOWindow window = (PrefabSOWindow)EditorWindow.GetWindow(typeof(PrefabSOWindow),true,windowTitle);
            if (filterOptions == null)
                filterOptions = new List<string>();
            FilterOption(mustStartWith,searchOption);
            window.minSize = minWindowSize;
            window.title = windowTitle;
            
        }
        private void OnGUI()
        {

            
            GuiLayoutExtras.CenterLabel("", 10);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.Label("     Search option:", GUILayout.Height(25f));
            searchOption = EditorGUILayout.TextField(searchOption, GUILayout.Height(25f));

            if (EditorGUI.EndChangeCheck())
                FilterOption(mustStartWith, searchOption);

            EditorGUILayout.EndHorizontal();

            GuiLayoutExtras.CenterLabel("", 10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(options.previewPrefabs[currentPrefabIndex], GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x)))
            {
                callbackFunction(AssetDatabase.LoadAssetAtPath(options.prefabFileList[currentPrefabIndex], typeof(GameObject)) as GameObject);
            }
            GUILayout.Label(options.prefabName[currentPrefabIndex], GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x * 2));
            EditorGUILayout.EndHorizontal();

            GuiLayoutExtras.CenterLabel("", 10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < filterListValues.Count; i++)
            {
                // START ROW SECTION
                if (i % buttonsRow == 0)
                    EditorGUILayout.BeginHorizontal();

                // GET THE VALUE OF THE SAVED INDEX IN FILTER LIST VALUES
                int index = filterListValues[i];

                // BUTTONS SECTION
                if (GUILayout.Button(options.previewPrefabs[index], GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x)))
                {
                    selectedPrefab = AssetDatabase.LoadAssetAtPath(options.prefabFileList[index], typeof(GameObject)) as GameObject;
                    Debug.Log(selectedPrefab);
                    currentPrefabIndex = index;
                    callbackFunction(selectedPrefab);
                    
                }

                // BREAK ROW SECTION
                if (i == filterListValues.Count - 1 || i % buttonsRow == buttonsRow - 1)     // IF ITS THE LAST ELEMENT, OR IF IT IS THE LAST IN THE ROW
                    EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        public void FilterOption(string startWith, string searchOption = "")
        {
            filterListValues.Clear();           //  WE WILL ONLY SAVE THE POSITION OF THE VALUE THAT NEEDS TO BE DISPLAYED

            string[] splitSearch = searchOption.Split(' ');
            for (int i = 0; i < options.prefabName.Count; i++)
            {
                bool add = true;
                if (!options.prefabName[i].ToLower().StartsWith(mustStartWith.ToLower()))
                    add = false;


                if (add)
                {
                    foreach (string stSearch in splitSearch)
                    {
                        if (!options.prefabName[i].ToLower().Contains(stSearch))      // if prefab name does not contains the searchoption string, dont add it to the list
                        {
                            add = false;
                        }
                    }
                }

                if (add)
                {
                    filterListValues.Add(i);    // SAVE THE POSITION OF THE FILTERES OBJECTS
                }
            }
        }

    }
}
