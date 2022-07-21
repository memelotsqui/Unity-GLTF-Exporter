using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectBuffer
    {
        public static int byteSize = 0;

        public static void Reset()
        {
            byteSize = 0;
        }
        public static void AddByteSize(int byte_size)
        {
            byteSize += byte_size;
        }
        public static string GetGLTFData(string bufferName, bool add_end_comma = true)
        {
            string result = "\"buffers\" : [\n" +
                "{\n" +
                "\"uri\" : \"" + bufferName + ".bin\",\n" +
                "\"byteLength\" : " + byteSize + "\n" +
                "}\n" +
                "]";                                                          // independent values, must not have comma at the end
            if (add_end_comma)
                result += ",\n";
            return result;
        }

    }
}
