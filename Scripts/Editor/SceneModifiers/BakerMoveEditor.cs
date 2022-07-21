using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(BakerMover), true)]
    public class BakerMoveEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BakerMover myScript = (BakerMover)target;

            if (myScript.firstTime == false)
            {
                GUILayout.BeginHorizontal("box");

                if (GUILayout.Button("Move To Saved Position", GUILayout.Height(100)))
                {
                    myScript.MoveToSavedPosition();
                }
                if (GUILayout.Button("Save Current Position", GUILayout.Height(100)))
                {
                    if (EditorUtility.DisplayDialog("Confirm reset", "Are you sure you want to reset position", "ok", "cancel"))
                    {
                        myScript.GetObjectPosition();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Move To Test Position", GUILayout.Height(80)))
                {
                    myScript.MoveToOutsidePosition();
                }
                if (GUILayout.Button("Save Test Position", GUILayout.Height(80)))
                {

                    myScript.GetObjectTestPosition();

                }
                GUILayout.EndHorizontal();


            }
            else
            {
                if (GUILayout.Button("Save Current Position", GUILayout.Height(100)))
                {
                    myScript.GetObjectPosition();
                    myScript.firstTime = false;
                }

            }
            base.OnInspectorGUI();
        }
    }
}
