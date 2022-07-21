using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(CreateMeshColliderOnChilds))]
    public class CreateMeshColliderOnChildsEditor : Editor
    {
        MonoScript script;
        private void OnEnable()
        {
            script = MonoScript.FromMonoBehaviour((CreateMeshColliderOnChilds)target);
        }
        public override void OnInspectorGUI()
        {
            CreateMeshColliderOnChilds myScript = (CreateMeshColliderOnChilds)target;

            script = EditorGUILayout.ObjectField("Script: ", script, (typeof(MonoScript)), false) as MonoScript;

            if (myScript.displayButtons)
            {
                if (GUILayout.Button("Add Mesh Colliders to Childs", GUILayout.Height(80f)))
                {
                    myScript.CreateMeshColliders();
                }
                if (myScript.createdsColliders)
                {
                    if (GUILayout.Button("Revert as initial mode", GUILayout.Height(80f)))
                    {
                        myScript.RevertMeshColliders();
                    }
                }
            }

        }
    }
}
