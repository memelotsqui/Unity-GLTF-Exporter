using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class AskUserInputWindow : EditorWindow
    {
        public delegate void SetString(string st);
        protected SetString callbackFunction;
        protected string titleWindow;
        protected string descriptionWindow;
        protected string okButton;
        protected string cancelButton;

        string _value;
        public static AskUserInputWindow CreateInstance(string title_window, string description_window, SetString callback_function, string cancel_button = "Cancel", string ok_button = "Ok")
        {

            AskUserInputWindow o = CreateInstance<AskUserInputWindow>();

            // Then directly set the values
            o.descriptionWindow = description_window;
            o.titleWindow = title_window;
            o.callbackFunction = callback_function;
            o.okButton = ok_button;
            o.cancelButton = cancel_button;

            o.minSize = new Vector2(300f, 120f);
            o.maxSize = o.minSize;
            o.titleContent = new GUIContent(o.titleWindow);

            var position = o.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            o.position = position;
            o.Show();
            /* .... */

            return o;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(new Vector2(20f, 0f), new Vector2(260f, 400f)));
            EditorGUILayout.LabelField(descriptionWindow, EditorStyles.boldLabel, GUILayout.Height(50f));
            
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(new Vector2(20f, 45f), new Vector2(260f, 400f)));
            _value = EditorGUILayout.TextField(_value,GUILayout.Height(20f));
            GUILayout.EndArea();

            //EditorGUILayout.BeginHorizontal();
            GUILayout.BeginArea(new Rect(new Vector2(50f,80f),new Vector2(400f, 200f)));
            if (GUILayout.Button(cancelButton, GUILayout.Width (90f), GUILayout.Height(30f)))
                Close();
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(new Vector2(150f, 80f), new Vector2(400f, 200f)));
            if (GUILayout.Button(okButton, GUILayout.Width(90f), GUILayout.Height(30f)))
            {
                callbackFunction(_value);
                Close();
            }
            GUILayout.EndArea();
            //EditorGUILayout.EndHorizontal();
        }
    }
}
