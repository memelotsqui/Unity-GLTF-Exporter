using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(OffsetUVsTemporaryReset))]
    public class OffsetUVsTemporaryResetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            OffsetUVsTemporaryReset myScript = (OffsetUVsTemporaryReset)target;

            if (GUILayout.Button("Set Original Materials", GUILayout.Height(100)))
            {
                myScript.SetOriginalMesh();
            }
            if (GUILayout.Button("Set Edited Materials", GUILayout.Height(100)))
            {
                myScript.SetEditedMesh();
            }
        }
    }
}
