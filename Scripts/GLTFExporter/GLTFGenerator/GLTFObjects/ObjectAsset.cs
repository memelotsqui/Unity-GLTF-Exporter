using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectAsset
    {
        const string version = "2.0";
        const string generator = "ola k ase";

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            result += "\"asset\": {\n" +
                "\"version\": \"" + version + "\",\n" +
                "\"generator\": \"" + generator + "\"\n" +
                "}";
            if (add_end_comma)
                result += ",\n";
            return result;
        }
    }
}
