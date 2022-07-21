using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(OffsetUVs), true), CanEditMultipleObjects]
    public class OffsetUVsEditor : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            OffsetUVs myScript = (OffsetUVs)target;

            Undo.RecordObject(myScript, "offset uv values");
            Undo.RecordObject(myScript.meshFilter, "offset uv values");
            //Undo.RecordObject(myScript.ori);

                base.OnInspectorGUI();
                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(0);
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(1);
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(2);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(3);
                if (GUILayout.Button("", GUILayout.Height(30)))
                {
                    myScript.SetOriginalMesh();
                    //myScript.ResetOriginalMesh();
                    myScript.DebugClick();
                }
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(5);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(6);
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(7);
                if (GUILayout.Button("", GUILayout.Height(30)))
                    myScript.OffsetAllUVs(8);
                GUILayout.EndHorizontal();

            if (!myScript.uvsFromLightmapUvs)
            {
                if (GUILayout.Button("Set Uv from lightmap uvs", GUILayout.Height(30)))
                    myScript.SetLightmapUvsAsMainUvs(true);
            }
            else
            {
                if (GUILayout.Button("Get Original uvs", GUILayout.Height(30)))
                    myScript.SetLightmapUvsAsMainUvs(false);
            }

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Set All To Original UVs", GUILayout.Height(30)))
                OffsetUVs.SetAllToOriginalMesh(true);
            if (GUILayout.Button("Set All To Edited UVs", GUILayout.Height(30)))
                OffsetUVs.SetAllToOriginalMesh(false);
            GUILayout.EndHorizontal();
        }
    }
}
