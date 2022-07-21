using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtension
    {
        public static List <string> extensionsUsedList;
        public static List <string> extensionsRequiredList;
        public static List<ObjectProperty> globalProperties;
        
        public string name;
        public List<ObjectProperty> properties;
        public static void Reset()
        {
            extensionsUsedList = new List<string>();
            extensionsRequiredList = new List<string>();
            globalProperties = new List<ObjectProperty>();
        }
        public static void Add(ObjectProperty object_property)
        {
            globalProperties.Add(object_property);
        }
        public ObjectExtension(string extension_name, List<ObjectProperty> properties_list, bool isRequired, bool ExportInGLTF = true)
        {
            name = extension_name;
            AddToExtensionsUsed();
            if (isRequired)
                AddToExtensionRequired();
            properties = properties_list;
        }
        public static string GetGLTFHeaderExtensionData(bool add_end_comma = true)
        {
            string result = "";
            if (extensionsUsedList.Count > 0)
            {
                //result += "{\n";
                result += "\"extensionsUsed\" : [\n";
                for (int i = 0; i < extensionsUsedList.Count; i++)
                {
                    result += "\""+ extensionsUsedList[i] + "\"";
                    if (i < extensionsUsedList.Count - 1)
                        result += ",";
                }
                result += "\n]";
                if (extensionsRequiredList.Count > 0)
                {
                    result += ",\n";
                    result += "\"extensionsRequired\" : [\n";
                    for (int i = 0; i < extensionsRequiredList.Count; i++)
                    {
                        result += "\"" + extensionsRequiredList[i] + "\"";
                        if (i < extensionsRequiredList.Count - 1)
                            result += ",\n";
                    }
                    result += "\n]";
                }
                //result += "}";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (globalProperties.Count > 0)
            {
                result += "\"extensions\" : {\n";
                for (int i = 0; i < globalProperties.Count; i++)
                {
                    result += globalProperties[i].GetPropertyGLTF();
                }
                result += "\n}";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }
 

        public void AddToExtensionsUsed()
        {
            bool isNewExtension = true;
            foreach (string st in extensionsUsedList)
            {
                if (name == st)
                {
                    isNewExtension = false;
                    break;
                }
            }
            if (isNewExtension)
                extensionsUsedList.Add(name);
        }

        public void AddToExtensionRequired()
        {
            bool isNewExtension = true;
            foreach (string st in extensionsRequiredList)
            {
                if (name == st)
                {
                    isNewExtension = false;
                    break;
                }
            }
            if (isNewExtension)
                extensionsRequiredList.Add(name);

        }


        
    }
}
