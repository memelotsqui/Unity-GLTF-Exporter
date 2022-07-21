using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(OffsetChildOffsetUVs))]
    public class OffsetChildOffsetUVsEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            OffsetChildOffsetUVs myScript = (OffsetChildOffsetUVs)target;
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(0);
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(1);
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(2);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(3);
            if (GUILayout.Button("", GUILayout.Height(30)))
                Debug.Log("peekaboo");
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(6);
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(7);
            if (GUILayout.Button("", GUILayout.Height(30)))
                myScript.SetOffset(8);
            GUILayout.EndHorizontal();
        }
    }
}
