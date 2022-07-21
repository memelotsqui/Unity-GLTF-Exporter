using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(SwitchChildMaterials), true)]
    public class SwitchChildMaterialsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal("box");
            SwitchChildMaterials myScript = (SwitchChildMaterials)target;
            if (GUILayout.Button("Set To Original Material", GUILayout.Height(30)))
            {
                myScript.SwitchToOriginalMaterial();
            }
            if (GUILayout.Button("Set To Target Material", GUILayout.Height(30)))
            {
                myScript.SwitchToTargetMaterial();
            }
            GUILayout.EndHorizontal();
        }
    }
}
