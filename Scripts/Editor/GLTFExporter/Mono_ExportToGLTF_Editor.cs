using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor(typeof(Mono_ExportToGLTF),true)]
    public class Mono_ExportToGLTF_Editor : Editor
    {
        Mono_ExportToGLTF myScript;
        string idValueTemp = "";

        private void OnEnable()
        {
            myScript = (Mono_ExportToGLTF)target;
        }
        public override void OnInspectorGUI()
        {
            myScript.gltfCustomOptions = (SO_ExportGLTFOptions)EditorGUILayout.ObjectField("GLTF Custom Options: ", myScript.gltfCustomOptions, typeof(SO_ExportGLTFOptions), true);
            if (myScript.gltfCustomOptions == null)
            {
                //EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit Options", GUILayout.Height(50f)))
                {
                    ExportGLTFOptionsWindow gltfOptionsWindow = ExportGLTFOptionsWindow.CreateInstance();
                }
            }
            myScript.targetParent = (Transform)EditorGUILayout.ObjectField("Target Parent: ", myScript.targetParent, typeof(Transform),true);
            myScript.startPosition = (Transform)EditorGUILayout.ObjectField("Start Position: ", myScript.startPosition, typeof(Transform),true);
            myScript.optionalEnvironmentReflections = (ReflectionProbe)EditorGUILayout.ObjectField("reflection probe: ", myScript.optionalEnvironmentReflections, typeof (ReflectionProbe),true);
            myScript.gltfName = EditorGUILayout.TextField("gltf name: ", myScript.gltfName);
            //myScript.exportForNFT = EditorGUILayout.Toggle("NFT Export", myScript.exportForNFT);

            
            EditorGUI.BeginChangeCheck();
            if (idValueTemp != "")
            {
                myScript.modelId = idValueTemp;
                idValueTemp = "";
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
            //base.OnInspectorGUI();
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
                myScript.ExportGLTF(myScript.exportLocation,test_build);
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
