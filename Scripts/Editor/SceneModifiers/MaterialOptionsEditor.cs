using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER
{
    [CustomEditor(typeof(MaterialOptions))]
    public class MaterialOptionsEditor : Editor
    {
        MonoScript script;
        private void OnEnable()
        {
            script = MonoScript.FromMonoBehaviour((MaterialOptions)target);
        }
        public override void OnInspectorGUI()
        {
            MaterialOptions myScript = (MaterialOptions)target;
            script = EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false) as MonoScript;
            Undo.RecordObject( myScript, "material option");
            if (myScript.changeMesh != null)
            {
                foreach (MeshRenderer mr in myScript.changeMesh)
                {
                    Undo.RecordObject(mr, "material option");
                }
            }
            


            if (myScript.gameObject.activeInHierarchy)
            {
                if (myScript.displayOptions)
                {
                    base.OnInspectorGUI();
                    if (myScript.canOffset)
                    {
                        EditorGUILayout.LabelField("Offset Columns");
                        myScript.offsetColumns = EditorGUILayout.IntSlider(myScript.offsetColumns, 1,10);
                        EditorGUILayout.LabelField("Offset Rows");
                        myScript.offsetRows = EditorGUILayout.IntSlider(myScript.offsetRows, 1,10);

                    }
                }
                else
                {
                    if (GUILayout.Button("display options", GUILayout.Height(30))) myScript.displayOptions = true;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("last Material", GUILayout.Height(30))) myScript.ChangeMaterial(false);
                    if (GUILayout.Button("next Material", GUILayout.Height(30))) myScript.ChangeMaterial(true);
                    GUILayout.EndHorizontal();
                    if (myScript.canOffset)
                    {
                        GuiLayoutExtras.CenterLabel("Column: " + myScript.curColumn + " === Row: " + myScript.curRow, 25);
                        GuiLayoutExtras.CenterLabel("",5);
                        //GUILayout.Label("Columns: " + myScript.offsetColumns + "=== Rows: " + myScript.offsetRows);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("<", GUILayout.Height(50))) myScript.SelectOffset(1);
                        GUILayout.BeginVertical();
                        if (GUILayout.Button("^", GUILayout.Height(25))) myScript.SelectOffset(0);
                        if (GUILayout.Button("v", GUILayout.Height(25))) myScript.SelectOffset(3);
                        GUILayout.EndVertical();
                        if (GUILayout.Button(">", GUILayout.Height(50))) myScript.SelectOffset(2);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            

            
        }
    }
}
