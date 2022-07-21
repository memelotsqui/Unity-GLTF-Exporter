using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    public class OffsetUVsOrigMesh : MonoBehaviour
    {
        public Mesh originalMesh;
        private void OnEnable()
        {
            if (originalMesh == null)
            {
                originalMesh = GetComponent<MeshFilter>().sharedMesh;
            }
        }
/*        public void GetOriginalMesh()
        {
           
        }*/
    }
}
