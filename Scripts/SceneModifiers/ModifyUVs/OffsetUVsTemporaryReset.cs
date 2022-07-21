using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class OffsetUVsTemporaryReset : MonoBehaviour
    {
        public void SetOriginalMesh()
        {
            OffsetUVs [] allOffsetUVs = GameObject.FindObjectsOfType<OffsetUVs>();
            foreach (OffsetUVs ou in allOffsetUVs)
            {
                ou.SetOriginalMesh();
            }
        }
        public void SetEditedMesh()
        {
            OffsetUVs[] allOffsetUVs = GameObject.FindObjectsOfType<OffsetUVs>();
            foreach (OffsetUVs ou in allOffsetUVs)
            {
                ou.SetOriginalMesh(false);
            }
        }

    }
}
