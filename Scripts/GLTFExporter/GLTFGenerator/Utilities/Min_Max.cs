using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class Min_Max
    {
        public float[] min;
        public float[] max;
        public float[] scale;
        public float[] offset;
        //public bool reqSign = false;    // USED FOR QUANTIZATION

        public Min_Max(float _min, float _max, int values_qty)
        {
            min = new float[values_qty];
            max = new float[values_qty];
            for (int i =0; i < values_qty; i++)
            {
                min[i] = _min;
                max[i] = _max;
            }
        }

        public Min_Max(float[] f)
        {
            min = new float[1];
            max = new float[1];

            min[0] = f[0];
            max[0] = f[0];

            foreach (int val in f)
            {
                if (val > max[0]) max[0] = val;
                if (val < min[0]) min[0] = val;
            }
            //if (min[0] < 0 && max[0] > 0)
            //    reqSign = true;
        }
        public Min_Max(Vector2[] v2)
        {
            min = new float[2];
            max = new float[2];

            min[0] = v2[0].x;
            min[1] = v2[0].y;

            max[0] = v2[0].x;
            max[1] = v2[0].y;
            for (int i =0; i < v2.Length; i++)
            {
                if (v2[i].x > max[0]) max[0] = v2[i].x;
                if (v2[i].y > max[1]) max[1] = v2[i].y;

                if (v2[i].x < min[0]) min[0] = v2[i].x;
                if (v2[i].y < min[1]) min[1] = v2[i].y;
            }
            //if (min[0] < 0 && max[0] > 0)
            //    reqSign = true;
            //if (min[1] < 0 && max[1] > 0)
            //    reqSign = true;
        }
        public Min_Max(Vector3[] v3, Vector3 modifier)
        {
            min = new float[3];
            max = new float[3];

            min[0] = v3[0].x * modifier.x;
            min[1] = v3[0].y * modifier.y;
            min[2] = v3[0].z * modifier.z;

            max[0] = v3[0].x * modifier.x;
            max[1] = v3[0].y * modifier.y;
            max[2] = v3[0].z * modifier.z;
            for (int i = 0; i < v3.Length; i++)
            {
                Vector3 nv3 = new Vector3(v3[i].x * modifier.x, v3[i].y * modifier.y, v3[i].z * modifier.z);
                if (nv3.x > max[0]) max[0] = nv3.x;
                if (nv3.y > max[1]) max[1] = nv3.y;
                if (nv3.z > max[2]) max[2] = nv3.z;

                if (nv3.x < min[0]) min[0] = nv3.x;
                if (nv3.y < min[1]) min[1] = nv3.y;
                if (nv3.z < min[2]) min[2] = nv3.z;
            }

            //if (min[0] < 0 && max[0] > 0)
            //    reqSign = true;
            //if (min[1] < 0 && max[1] > 0)
            //    reqSign = true;
            //if (min[2] < 0 && max[2] > 0)
            //    reqSign = true;
        }

        public Min_Max(Vector4[] v4, Vector4 modifier)
        {
            min = new float[4];
            max = new float[4];

            min[0] = v4[0].x * modifier.x;
            min[1] = v4[0].y * modifier.y;
            min[2] = v4[0].z * modifier.z;
            min[3] = v4[0].w * modifier.w;

            max[0] = v4[0].x * modifier.x;
            max[1] = v4[0].y * modifier.y;
            max[2] = v4[0].z * modifier.z;
            max[3] = v4[0].w * modifier.w;
            for (int i = 0; i < v4.Length; i++)
            {
                Vector4 nv4 = new Vector4(v4[i].x * modifier.x, v4[i].y * modifier.y, v4[i].z * modifier.z, v4[i].w * modifier.w);
                if (nv4.x > max[0]) max[0] = nv4.x;
                if (nv4.y > max[1]) max[1] = nv4.y;
                if (nv4.z > max[2]) max[2] = nv4.z;
                if (nv4.w > max[3]) max[3] = nv4.w;

                if (nv4.x < min[0]) min[0] = nv4.x;
                if (nv4.y < min[1]) min[1] = nv4.y;
                if (nv4.z < min[2]) min[2] = nv4.z;
                if (nv4.w < min[3]) min[3] = nv4.w;
            }
        }
        public Min_Max(int[] i)
        {
            min = new float[1];
            max = new float[1];

            min[0] = i[0];
            max[0] = i[0];

            foreach (int val in i)
            {
                if (val > max[0]) max[0] = val;
                if (val < min[0]) min[0] = val;
            }
            //if (min[0] < 0 && max[0] > 0)
            //    reqSign = true;
        }

        public string GetValues(bool get_min, bool as_int = false)
        {
            if (get_min)
                return GetValueFromArray(min, as_int);
            else
                return GetValueFromArray(max, as_int);

        }

        public string GetValueFromArray(float [] array, bool as_int)
        {
            string result = "";
            foreach (float f in array)
            {
                if (as_int)
                {
                    result += Mathf.RoundToInt(f).ToString("D");
                }
                else
                {
                    result += f.ToString();
                }
                result += ",";
            }
            return StringUtilities.RemoveCharacterFromString(result, 1, false);
        }
        public float[] GetQuantizationScale()
        {
            if (scale != null)
                return scale;

            float[] result = new float[min.Length];

            for (int i =0; i < result.Length; i++)
                if (max[i] != min[i])
                    result[i] = max[i] - min[i];
                else
                    result[i] = 1;

            scale = result;
            return result;
        }
        public float[] GetQuantizationOffset()
        {
            if (offset != null)
                return offset;

            float[] result = new float[min.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = min[i];

            offset = result;
            return result;
        }
        public static float Normalize(float _val, float _min, float _max, bool neg_one_to_pos_one = false)
        {
            if (_min == _max)
                return 0;
            if (_min > _max)
            {
                Debug.LogWarning("MIN VALUE IS HIGER THAN MAX VALUE, RETURNING -1");
                return -1;
            }
            else
            {
                if (!neg_one_to_pos_one)        // VALUE WILL GO FROM 0 TO 
                {
                    return (_val - _min) / (_max - _min);
                }
                else
                {
                    float _modif = (_min + _max) / 2;
                    return (_val - _modif) / (_max - _modif);
                }
            }
        }

        public float GetMaxValueFromVectors()
        {
            float maxVal = 0;
            foreach (float f in max)
            {
                if (f > maxVal)
                    maxVal = f;
            }
            return maxVal;
        }

        public float GetMinValueFromVectors()
        {
            float minVal = 0;
            foreach (float f in min)
            {
                if (f < minVal)
                    minVal = f;
            }
            return minVal;
        }
    }
}
