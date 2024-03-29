﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor(typeof(SmartObjectBehaviour),true)]
    public class SmartObjectBehaviour_Editor : Editor
    {
        SmartObjectBehaviour myScript;
        private void OnEnable()
        {
            myScript = (SmartObjectBehaviour)target;
        }
        public override void OnInspectorGUI()
        {
            //GUI.enabled = false;
            myScript.javascript = (TextAsset)EditorGUILayout.ObjectField("Javascript",myScript.javascript,typeof(TextAsset), false);
            //GUI.enabled = true;
        }
    }
}
