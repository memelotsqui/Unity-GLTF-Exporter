using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER
{
    public class ChangeSelectedObjectsMaterialWindow : EditorWindow
    {
        public static Material targetMaterial;
        public bool changeMaterialToChilds = true;

        public static void ShowWindow()
        {
            GetPlayerPrefs();
            EditorWindow.GetWindow(typeof(ChangeSelectedObjectsMaterialWindow));
        }
        private void OnGUI()
        {
            GUILayout.Label("Target Material", EditorStyles.boldLabel);
            targetMaterial = (Material)EditorGUILayout.ObjectField("Target Material", targetMaterial, (typeof(Material)));
            changeMaterialToChilds = EditorGUILayout.Toggle("Change childs materials",changeMaterialToChilds);
            if (GUILayout.Button("Assign Material to selection", GUILayout.Height(80f)))
            {
                AssignMaterialToSelectedAssets();
            }
            if (GUILayout.Button("Save to prefs", GUILayout.Height(80f)))
            {
                SavePlayerPrefs();
            }
        }
        public void AssignMaterialToSelectedAssets()
        {
            if (targetMaterial != null)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (changeMaterialToChilds)
                    {
                        MeshRenderer[] allMeshRend = go.GetComponentsInChildren<MeshRenderer>(true);
                        foreach (MeshRenderer mr in allMeshRend)
                        {
                            mr.sharedMaterial = targetMaterial;
                        }
                    }
                    else
                    {
                        MeshRenderer mr = go.GetComponent<MeshRenderer>();
                        if (mr != null)
                        {
                            mr.sharedMaterial = targetMaterial;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("NO MATERIAL SELECTED, please assign material before setting material to selection");
            }
        }
        public void SavePlayerPrefs()
        {
            if (targetMaterial != null)
            {
                PlayerPrefs.SetString("ChangeSelectedObjectsMaterial_Material", AssetDatabase.GetAssetPath(targetMaterial));
            }
        }
        public static void GetPlayerPrefs()
        {
            if (targetMaterial == null)
            {
                targetMaterial = (Material)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString("ChangeSelectedObjectsMaterial_Material"),typeof (Material));
            }
        }
    }
}
