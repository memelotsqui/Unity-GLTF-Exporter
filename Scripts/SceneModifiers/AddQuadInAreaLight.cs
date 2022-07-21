using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER
{
    public class AddQuadInAreaLight : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public void UpdateQuad()
        {
            Light light = GetComponent<Light>();
            if (light != null)
            {
                if (light.type == LightType.Area)
                {
                    if (meshFilter == null || meshRenderer == null)
                    {
                        GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                        if (meshFilter == null)
                            meshFilter = gameObject.AddComponent<MeshFilter>();
                        if (meshRenderer == null)
                            meshRenderer = gameObject.AddComponent<MeshRenderer>();

                        meshFilter.sharedMesh = quadObject.GetComponent<MeshFilter>().sharedMesh;
                        meshRenderer.sharedMaterial = quadObject.GetComponent<MeshRenderer>().sharedMaterial;
                    }



                    //quadObject.transform.localPosition = Vector3.zero;
                    //quadObject.transform.localEulerAngles = new Vector3(180, 0, 0);

                    Debug.Log(transform.parent.lossyScale);

                    transform.localScale = new Vector3(light.areaSize.x/ transform.parent.lossyScale.x, light.areaSize.y/ transform.parent.lossyScale.y, -1);

                }
                else
                {
                    Debug.Log("not area light");
                }
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(AddQuadInAreaLight))]
    public class AddQuadInAreaLightEditor : Editor
    {
        private MonoScript script;
        private AddQuadInAreaLight myScript;
        private void OnEnable()
        {
            myScript = (AddQuadInAreaLight)target;
            script = MonoScript.FromMonoBehaviour(myScript);

        }

        public override void OnInspectorGUI()
        {
            string quadUpdate = (myScript.meshFilter == null ||  myScript.meshRenderer == null) ? "Create Quad" : "Update Quad";
            if (GUILayout.Button(quadUpdate, GUILayout.Height(40f)))
            {
                myScript.UpdateQuad();
            }
            //base.OnInspectorGUI();
        }
    }
    #endif
}
