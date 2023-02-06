using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor(typeof(SmartObjectBehaviour),true)]
    public class SmartObjectBehaviour_Editor : Editor
    {
        SmartObjectBehaviour myScript;

        private void OnEnable()
        {
            myScript = (SmartObjectBehaviour)target;



            if (myScript.javascript == null)
            {
                // find th js code
            }
            string displayTitle = StringUtilities.GetFileNameFromPath(StringUtilities.GetFullPathFromAsset(myScript.javascript));
            var inspectorTitles = ObjectNamesUtility.GetInternalInspectorTitlesCache();
            inspectorTitles[typeof(SmartObjectBehaviour)] = displayTitle + " (JS Smart)";

            if (myScript.javascript != null)
            {

                string file_path = StringUtilities.GetFullPathFromAsset(myScript.javascript);
                StreamReader inp_stm = new StreamReader(file_path);
                List<JSVar> jsvars = new List<JSVar>();
                while (!inp_stm.EndOfStream)
                {
                   
                    string inp_ln = inp_stm.ReadLine();
                    //Debug.Log(inp_ln);
                    string lineVal = inp_ln.Replace('\t', ' ').Replace(" ", "").Replace(";", "");
                    if (lineVal.Contains(":") && !lineVal.StartsWith("//")) {
                       
                        string[] splitString = lineVal.Split(":");
                        AddVar(splitString[0], splitString[1], ref jsvars);
                    }
                }
                inp_stm.Close();
                myScript.allVars = jsvars;
            }
        }
        
        private void AddVar(string var_name, string type_name, ref List<JSVar> final_list)
        {
            //Debug.Log("enters");

            JSVar v = myScript.getExistingJSVar(var_name, type_name);

            if (v != null)
                final_list.Add(v);
            else
                final_list.Add(new JSVar(var_name, type_name));
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            myScript.javascript = EditorGUILayout.ObjectField("Javascript",myScript.javascript,typeof(Object), false);
            GUI.enabled = true;
            for (int i =0; i < myScript.allVars.Count; i++)
            {
                JSVar jsvar = myScript.allVars[i];
                switch (myScript.allVars[i].getJsVarName())
                {
                    case "number":
                        jsvar.numberValue = EditorGUILayout.FloatField(jsvar.varName, jsvar.numberValue);
                        
                        break;
                    case "string":
                        jsvar.stringValue = EditorGUILayout.TextField(jsvar.varName, jsvar.stringValue);
                        break;
                    //default:
                    //    JSScript sc = myScript.allVars[i] as JSScript;
                    //    sc.varValue = (SmartObjectBehaviour)EditorGUILayout.ObjectField(sc.varName, sc.varValue, typeof(SmartObjectBehaviour), true);
                    //    break;
                }
            }
        }
    }
}
