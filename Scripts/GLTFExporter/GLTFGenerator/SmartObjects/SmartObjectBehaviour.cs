using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WEBGL_EXPORTER.GLTF
{
    public class SmartObjectBehaviour : MonoBehaviour
    {
       
        [SerializeField]
        public List<JSVar> allVars;


        public UnityEngine.Object javascript;

        private void Start()
        {
            // do nothing, just need the checkbox
        }

        public JSVar getExistingJSVar(string var_name, string type = "")
        {
            if (allVars == null)
                return null;

            foreach (JSVar jsvar in allVars)
            {
                if (jsvar.isSame(var_name, type))
                    return jsvar;
            }
            return null;
        }

    }


    [Serializable]
    public class JSVar
    {
        [SerializeField]
        public string varName = "";
        [SerializeField]
        public string typeName = "";

        public SmartObjectBehaviour scriptValue;
        public string stringValue;
        public float numberValue;
        public JSVar(string var_name, string type_name)
        {
            varName = var_name;
            typeName = type_name;
        }
        public virtual bool isSame(string var_name, string var_type)
        {
            return varName == var_name;
        }

        public virtual string getJsVarName()
        {
            return typeName;
        }
    }
    //[Serializable]
    //public class JSScript : JSVar
    //{
    //    [SerializeField]
    //    public SmartObjectBehaviour varValue;

    //    public JSScript(string var_name, string type_name)
    //    {
    //        varName = var_name;
    //        typeName = type_name;
    //    }
    //    public override bool isSame(string var_name, string var_type)
    //    {
    //        if (var_type == "")
    //        {
    //            return false;
    //        }
    //        return base.isSame(var_name, "");
    //    }
    //}
    //[Serializable]
    //public class JSString : JSVar
    //{
        
    //    [SerializeField]
    //    public string varValue;
    //    public JSString(string var_name)
    //    {
    //        varName = var_name;
    //        varValue = "";
    //        typeName = "string";
    //    }
    //    public override bool isSame(string var_name, string var_type)
    //    {
    //        if (var_type != "string")
    //        {
    //            return false;
    //        }
    //        return base.isSame(var_name, "");
    //    }
    //}
    //[Serializable]
    //public class JSNumber : JSVar
    //{
    //    [SerializeField]
    //    public float varValue;
    //    public JSNumber(string var_name)
    //    {
    //        varName = var_name;
    //        varValue = 0f;
    //        typeName = "number";
    //    }
    //    public override bool isSame(string var_name, string var_type)
    //    {
    //        if (var_type != "number")
    //            return false;
    //        return base.isSame(var_name, "");
    //    }

    //}
}
