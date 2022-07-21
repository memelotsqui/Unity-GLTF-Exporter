using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class UVVariant 
    {
        //public MeshFilter baseMesh;         // BASE MESH, WITHOUT MODIFICATIONS FROM CODE
        public static List<UVVariant> allUVs;
        public static int globalIndex = -1;

        //vars
        public int index =-1;
        public Vector2 offsetUVs;
        public Vector2 scaleUVs;

        public ObjectAccessors optionalObjectAccessor;

        public int baseAccessorInt;

        public static void Reset()
        {
            allUVs = new List<UVVariant>();
            globalIndex = 0;
        }

        public UVVariant(Vector2 offset_uvs, Vector2 scale_uvs)
        {
            offsetUVs = offset_uvs;
            scaleUVs = scale_uvs;

            index = globalIndex;

            allUVs.Add(this);
            globalIndex++;
        }

        public ObjectAccessors CreateObjectAccessor(Vector2[] uvs, bool modify_values = true, string optional_identifier = "", Vector2? offset_orig = null, Vector2? scale_orig = null)
        {
            Vector2[] final_uvs = new Vector2[uvs.Length];
            //if (optional_identifier == "uvs_2")
            //{
            //    Debug.LogWarning("ENAB");
            //    foreach (Vector2 v in uvs)
            //    {
            //        //if (v.x < 0.000001f)
            //        // {
            //        Debug.Log(v.x);
            //        //}
            //    }
            //}

            Vector2 mult = Vector2.one;
            Vector2 offset = Vector2.zero;
            if (modify_values)
            {
                mult = scaleUVs;
                offset = offsetUVs;
            }
            //if (optional_identifier == "uvs_2") {  
            //    Debug.Log(scaleUVs.x + "   "  + scaleUVs.y);
            //    Debug.Log(offsetUVs.x + "   " + offsetUVs.y);
            //}
            for (int i =0; i < final_uvs.Length; i++)
            {
                float valx = uvs[i].x;
                float valy = uvs[i].y;
                if (offset_orig != null)
                {
                    valx = (valx * scale_orig.Value.x) + offset_orig.Value.x;
                    valy = (valy * scale_orig.Value.y) + offset_orig.Value.y;
                }


                valx = (valx * mult.x) + offset.x;
                valy = (-(valy * mult.y) - offset.y) + 1; // IN GLTF UVS IN Y AR INVERSED, THEY GO FROM LEFT TOP CORNER TO BOTTOM RIGHT CORNER, Y MUST BE INVERSED


                if (optional_identifier == "uvs_2")
                {
                    if (valx < 0.000001f) valx = 0f;
                    if (valy < 0.000001f) valy = 0f;
                }


                final_uvs[i] = new Vector2(valx, valy);
                //final_uvs[i] = new Vector2(
                //(uvs[i].x * mult.x) + offset.x,
                //(-(uvs[i].y * mult.y) - offset.y) + 1  
                //);

            }
            //DEBUGGINGR EMOVE LATER
           
            //if (optional_identifier == "uvs_2")
            //{
            //    Debug.LogWarning("STARTS");
            //    foreach (Vector2 v in final_uvs)
            //    {
            //        //if (v.x < 0.000001f)
            //       // {
            //            Debug.Log(v.x);
            //        //}
            //    }
            //}

            if (!ExportToGLTF.options.quantizeGLTF)
            {
                optionalObjectAccessor = new ObjectAccessors(final_uvs, false, optional_identifier);
            }
            else
            {
                optionalObjectAccessor = new ObjectAccessors(final_uvs, false, optional_identifier, ExportGLTFOptions.GetComponentType(ExportToGLTF.options.quantizeMainUVsTo), false);
            }
            return optionalObjectAccessor;
        }

        public int GetObjectAccessorExportInt()
        {
            if (optionalObjectAccessor != null)
                return optionalObjectAccessor.export_index;
            else
            {
                Debug.LogError("Object accessor is required in uvs but not present!, returning -1");
                return -1;
            }
        }
    }
}
