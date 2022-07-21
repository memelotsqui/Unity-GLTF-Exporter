using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(TurnChildGameObjectVisibility))]
    public class TurnChildGameObjectVisibilityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TurnChildGameObjectVisibility myScript = (TurnChildGameObjectVisibility) target;
            //base.OnInspectorGUI();
            if (GUILayout.Button("Get Child GameObjects"))
                myScript.GetFirstChildObjects();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Last Object"))
                myScript.TurnNextObject(false);
            if (GUILayout.Button("Next Object"))
                myScript.TurnNextObject(true);
            EditorGUILayout.EndHorizontal();
        }
    }
}
