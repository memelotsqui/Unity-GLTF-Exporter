using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WEBGL_EXPORTER
{
    public class VectorUtilities
    {
        public static Vector3 addVectors3(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3 multVector3(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
    }
}
