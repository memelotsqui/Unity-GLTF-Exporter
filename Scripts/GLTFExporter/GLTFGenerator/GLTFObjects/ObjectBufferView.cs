using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectBufferView
    {
        //statics
        public static int globalIndex = -1;
        public static List<ObjectBufferView> allUniqueBuffers;

        //vars
        public int buffer = 0;  // FOR NOW, ITS ALWAYS 0, SINCE IT REFERES TO THE BINARY
        public int byteSize;
        public int byteStart;
        public int byteStride = -1;
        public int bufferIndex = -1;

        public static void Reset()
        {
            allUniqueBuffers = new List<ObjectBufferView>();
            globalIndex = 0;
        }

        //public static int GetBufferViewIndex(BufferViewType buffer_type, int byte_size)
        //{
        //    if (allUniqueBuffers == null)
        //    {
        //        allUniqueBuffers = new List<ObjectBufferView>();
        //        allUniqueBuffers.Add(new ObjectBufferView(buffer_type, byte_size));

        //        return allUniqueBuffers.Count - 1;
        //    }
        //    else {
        //        foreach (ObjectBufferView obv in allUniqueBuffers)
        //        {
        //            if (obv.bufferType == buffer_type)
        //            {
        //                //IF FOUND WE MUST ADD BYTE SIZE
        //                obv.byteSize += byte_size;
        //                return obv.bufferIndex;
        //            }
        //        }
        //        allUniqueBuffers.Add(new ObjectBufferView(buffer_type, byte_size));
        //        return allUniqueBuffers.Count - 1;
        //    }   
        //}

        public ObjectBufferView(int byte_start = 0, int byte_stride = -1)
        {
            bufferIndex = globalIndex;
            byteSize = 0;
            byteStart = byte_start;
            byteStride = byte_stride;
            allUniqueBuffers.Add(this);
            globalIndex++;
        }
        public int GetCurrentAndAddDataToBufferView(int byte_size)
        {
            int initialSize = byteSize;
            byteSize += byte_size;
            return initialSize;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueBuffers != null)
            {
                if (allUniqueBuffers.Count > 0) {
                    int byteOffset = 0;
                    result += "\"bufferViews\":[";
                    for (int i =0; i < allUniqueBuffers.Count;i++)
                    {
                        result += allUniqueBuffers[i].GetBufferData(byteOffset) +",\n";
                        byteOffset += allUniqueBuffers[i].byteSize;
                        // IF WE DONT HAVE FOURTHS AMOUNT OF BYTES ADD 2 BYTES
                        if (!HasFourthAmountOfBytes(byteOffset))
                        {
                            byteOffset += (4 - (byteOffset % 4));
                        }
                    }
                    result = StringUtilities.RemoveCharacterFromString (result,2,false) +"]";
                    if (add_end_comma)
                        result += ",\n";
                }
            }
            return result;
        }

        public string GetBufferData(int byte_offset)
        {
            string result = "";
            string _stride = "";
            if (byteStride != -1)
            {
                _stride = "\"byteStride\" : " + byteStride + ",\n";
            }
            result += "{\n" +
                "\"buffer\" : " + buffer + ",\n" +
                "\"byteOffset\" : " + byte_offset + ",\n" +
                _stride +
                "\"byteLength\" : " + byteSize + "\n" +
                "}";
            return result;
        }

        public static bool HasFourthAmountOfBytes(int target_bytes)
        {
            if (target_bytes % 4 == 0)           // IF TOTAL INDICES ARE UNPAIR NUMBER ADD AN OFFSET OF 2 ADDITIONAL BYTES AT THE END
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
