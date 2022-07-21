using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor(typeof(ObjectNodeMirror),true)]
    public class ObjectNodeMirror_Editor : Editor
    {
        ObjectNodeMirror myScript;
        private void OnEnable()
        {
            myScript = (ObjectNodeMirror)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (myScript.tooltip != "")
            {
                GUILayout.Box(myScript.tooltip, GUILayout.Width(GuiLayoutExtras.GetCenteredLabelWidth(50f)));
            }
            if (myScript.mirrorReflectionProbe == null)
            {
                if (GUILayout.Button("Create Reflection Probe", GUILayout.Height(50f)))
                {
                    myScript.mirrorReflectionProbe =  CreateReflectionProbe(myScript.transform);
                    Selection.activeGameObject = myScript.mirrorReflectionProbe.gameObject;
                }
            }
        }

        private ReflectionProbe CreateReflectionProbe(Transform parent)
        {
            GameObject refProbeObject = new GameObject();
            refProbeObject.transform.parent = parent;
            refProbeObject.transform.localPosition = Vector3.zero;
            refProbeObject.transform.localScale = Vector3.one;

            ReflectionProbe refProbe = refProbeObject.AddComponent<ReflectionProbe>();
            refProbe.hdr = false;
            refProbe.resolution = 512;
            refProbe.transform.parent = parent;
            
            return refProbe;

        }
    }

    
}
