using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF.SMART
{
    [CustomEditor (typeof(Mono_ExportToGLTF_SmartObject),true)]
    public class Mono_ExportToGLTF_SmartObject_Editor : Editor
    {
        Mono_ExportToGLTF_SmartObject myScript;
        string idValueTemp = "";
        private void OnEnable()
        {
            myScript = (Mono_ExportToGLTF_SmartObject)target;
        }
        public override void OnInspectorGUI()
        {

            GUILayout.Box("SmartObjects may override defined GLTF Custom Options values depending on the Smart Object Type, please use export to GLTF if you want to keep GLTF Custom Options as they are.", GUILayout.Width(GuiLayoutExtras.GetCenteredLabelWidth(50f)));
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            if (idValueTemp != "")
            {
                myScript.modelId = idValueTemp;
                idValueTemp = "";
            }

            if (myScript.displayClassName)
            {
                myScript.smartType = (SmartType)EditorGUILayout.EnumPopup("Smart Type", myScript.smartType);
                myScript.smartObjectClassName = EditorGUILayout.TextField("Class Name: ", myScript.smartObjectClassName);
                
                
            }
            if (myScript.smartType == SmartType.space)
            {
                myScript.exportFog = EditorGUILayout.Toggle("Export Fog: ", myScript.exportFog);
                myScript.optionalReflectionProbe = (ReflectionProbe)EditorGUILayout.ObjectField("Optional Reflection Probe: ", myScript.optionalReflectionProbe, typeof(ReflectionProbe), true);
                myScript.startPosition = (GameObject)EditorGUILayout.ObjectField("Start Position: ", myScript.startPosition, typeof(GameObject), true);
                if (myScript.startPosition != null)
                {
                    if (myScript.startPosition.transform.IsChildOf(myScript.transform))
                    {
                        myScript.AddStartPositionToArray(myScript.startPosition);
                    }
                    else
                    {
                        Debug.LogError("GameObject must be a child of Smart Object");
                    }
                    myScript.startPosition = null;
                }
                if (myScript.startPositionList != null)
                {
                    if (myScript.startPositionList.Count > 0)
                    {

                        for (int i = 0; i < myScript.startPositionList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUI.enabled = false;
                            GameObject go = (GameObject)EditorGUILayout.ObjectField(myScript.startPositionList[i], typeof(GameObject), true);
                            GUI.enabled = true;
                            if (GUILayout.Button("X", GUILayout.Width(20f), GUILayout.Height(20f)))
                            {
                                myScript.startPositionList.RemoveAt(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                    }
                }
            }
            myScript.modelId = EditorGUILayout.TextField("model create folder: ", myScript.modelId);
            myScript.exportLocation = EditorGUILayout.TextField("export location: ", myScript.exportLocation);
            myScript.curBuild = EditorGUILayout.IntField("Current Build: ", myScript.curBuild);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export GLTF TestBuild", GUILayout.Height(50f)))
            {
                if (myScript.modelId == "")
                {
                    AskUserInputWindow inputWindow = AskUserInputWindow.CreateInstance("Name Required", "ID name", ValidateCallExport, "Cancel", "Save");
                }
                else
                {
                    if (myScript.exportLocation == "")
                    {
                        myScript.exportLocation = EditorUtility.SaveFolderPanel(
                            "Save To",
                            "",
                            "");
                        if (myScript.exportLocation != "")
                        {
                            ExportGLTFModel(true);
                        }
                    }
                    else
                    {
                        ExportGLTFModel(true);
                    }
                }

            }
            if (GUILayout.Button("Export GLTF Final", GUILayout.Height(50f)))
            {
                if (myScript.exportLocation == "")
                {
                    myScript.exportLocation = EditorUtility.SaveFolderPanel(
                        "Save To",
                        "",
                        "");
                    if (myScript.exportLocation != "")
                    {
                        ExportGLTFModel(false);
                    }
                }
                else
                {
                    ExportGLTFModel(false);
                }

            }
            EditorGUILayout.EndHorizontal();

            
        }
        private void ValidateCallExport(string value)
        {
            if (value != "")
            {
                idValueTemp = value;
            }

        }
        private void ExportGLTFModel(bool test_build)
        {

            if (FileExporter.DirectoryExists(myScript.exportLocation))
            {
                myScript.ExportGLTF(myScript.exportLocation, test_build);
                Debug.Log("Exported to: " + myScript.exportLocation);
            }
            else
            {
                Debug.LogWarning("export location does not exists");
                myScript.exportLocation = "";
            }
        }
    }
}
