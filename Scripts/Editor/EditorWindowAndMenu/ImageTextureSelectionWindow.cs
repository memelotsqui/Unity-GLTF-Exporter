using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER
{
    public class ImageTextureSelectionWindow : EditorWindow
    {
        public delegate void SetImage(Texture2D texture2D);
        protected SetImage callbackFunction; 

        //public string targetFolder = "";
        public List <Texture2D> previewTextures;
        public List<string> imageFileList;
        public Vector2 buttonSize;
        public int buttonsRow;

        public Vector2 scrollPosition = Vector2.zero;

        public ImageTextureSelectionWindow(string relativeFolderLocation, SetImage callback, int buttons_row, Vector2 button_size, Vector2 min_window_size, string window_title,bool getSubdirectories = false)
        {
            previewTextures = new List<Texture2D>();
            imageFileList = new List<string>();
            scrollPosition = Vector2.zero;
            callbackFunction = callback;
            buttonSize = button_size;
            buttonsRow = buttons_row;
            string[] allFiles = StringUtilities.GetFilesPathFromFolder("Assets/" + relativeFolderLocation, true, "", true, getSubdirectories);

            foreach (string st in allFiles)
            {
                if (st.EndsWith(".png") || st.EndsWith(".jpg") || st.EndsWith(".jpeg") || st.EndsWith(".psd") || st.EndsWith(".tga"))
                {
                    // GET ALL THE PREVIEW IMAGES FROM THE REALITVE FOLDER LOCATION
                    imageFileList.Add(st);
                    //previewTextures.Add(AssetPreview.GetAssetPreview(AssetDatabase.LoadAssetAtPath(st, typeof(Texture2D))));
                    previewTextures.Add(AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath(st, typeof(Texture2D))));
                }

            }
            ImageTextureSelectionWindow window = (ImageTextureSelectionWindow)EditorWindow.GetWindow(typeof(ImageTextureSelectionWindow), true, window_title);
            window.minSize = min_window_size;
            window.title = window_title;
        }

        // callback function: will call the function send to this window, and return the selected texture

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < previewTextures.Count; i++)
            {
                if (i % buttonsRow == 0)
                    EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(previewTextures[i], GUILayout.Height(buttonSize.y), GUILayout.Width(buttonSize.x)))
                {
                    Texture2D selectedImage = AssetDatabase.LoadAssetAtPath(imageFileList[i], typeof(Texture2D)) as Texture2D;
                    callbackFunction(selectedImage);
                }

                if (i == previewTextures.Count - 1 || i % buttonsRow == buttonsRow - 1)     // IF ITS THE LAST ELEMENT, OR IF IT IS THE LAST IN THE ROW
                    EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
