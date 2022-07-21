using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace WEBGL_EXPORTER
{
    [ExecuteAlways]
    public class ChangeChildMaterials : MonoBehaviour
    {
        public bool test = false;
        public Material searchFor;
        public Material changeTo;
        public string matName = "";

        public bool withName = false;
        
        public void ChangeChildMaterialsAction()
        {
            //string curPath = AssetDatabase.GetAssetPath(changeTo);
            //Material newMat = (Material)AssetDatabase.LoadAssetAtPath(curPath, typeof(Material));
            
            

            MeshRenderer[] childRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            if (!withName)
            {
                foreach (MeshRenderer mr in childRenderers)
                {
                    if (mr.sharedMaterial == searchFor)
                    {
                        mr.sharedMaterial = changeTo;
                    }
                }
            }
            else
            {
                foreach (MeshRenderer mr in childRenderers)
                {
                    if (mr.sharedMaterial.name == matName)
                    {
                        Debug.Log("itis");
                        mr.sharedMaterial = changeTo;
                    }
                }
            }
        }
    }


}

namespace WEBGL_EXPORTER
{
    
    [CustomEditor(typeof(ChangeChildMaterials))]
    public class ChangeChildMaterialsEditor : Editor
    {
        ChangeChildMaterials myScript;
        MonoScript script;

        private void OnEnable()
        {
            myScript = (ChangeChildMaterials)target;
            script = MonoScript.FromMonoBehaviour(myScript);
        }

        public override void OnInspectorGUI()
        {
            if (myScript.withName == false)
            {
                if (GUILayout.Button("change with string", GUILayout.Height(30f)))
                {
                    myScript.withName = true;
                }
            }
            else
            {
                if (GUILayout.Button("change with material", GUILayout.Height(30f)))
                {
                    myScript.withName = false;
                }
            }
            if (!myScript.withName)
            {
                myScript.searchFor = (Material)EditorGUILayout.ObjectField("Search For: ", myScript.searchFor, typeof(Material), false);
            }
            else
            {
                myScript.matName = EditorGUILayout.TextField("Material Name: ", myScript.matName);
            }
            myScript.changeTo = (Material)EditorGUILayout.ObjectField("Change To: ",myScript.changeTo, typeof(Material), false);
            if (!myScript.withName)
            {
                if (myScript.searchFor != null && myScript.changeTo != null)
                {
                    if (GUILayout.Button("Change child materials", GUILayout.Height(40f)))
                    {
                        myScript.ChangeChildMaterialsAction();
                    }
                }
            }
            else
            {
                if (myScript.matName != "" && myScript.changeTo != null)
                {
                    if (GUILayout.Button("Change child materials", GUILayout.Height(40f)))
                    {
                        myScript.ChangeChildMaterialsAction();
                    }
                }
            }
        }
    }
}
#endif
