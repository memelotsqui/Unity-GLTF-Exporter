using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(ChangeChildName))]
    public class ChangeChildNameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ChangeChildName myScript = (ChangeChildName)target;
            if (GUILayout.Button("Change First Child Names", GUILayout.Height(100)))
            {
                myScript.ChangeFirstChildNames();
            }
            if (GUILayout.Button("Change All Child Names", GUILayout.Height(100)))
            {
                myScript.ChangeAllChildNames();
            }
        }

    }
}
