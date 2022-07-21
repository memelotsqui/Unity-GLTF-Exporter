using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER {
    [ExecuteAlways]
    public class OffsetChildOffsetUVs : MonoBehaviour
    {
        //public float offsetQuantity = 0.125f;
        public OffsetUVs[] targetOffsets;
        public void SetOffset(int move)
        {
            if (targetOffsets == null)
            {
                OffsetUVs[] offsetuvs = GetComponentsInChildren<OffsetUVs>();
                foreach (OffsetUVs ofuv in offsetuvs)
                {
                    ofuv.OffsetAllUVs(move);
                }

            }
            else
            {
                if (targetOffsets.Length == 0)
                {
                    OffsetUVs[] offsetuvs = GetComponentsInChildren<OffsetUVs>();
                    foreach (OffsetUVs ofuv in offsetuvs)
                    {
                        ofuv.OffsetAllUVs(move);
                    }
                }
                else
                {
                    foreach (OffsetUVs ofuv in targetOffsets)
                    {
                        if (ofuv != null)
                        {
                            ofuv.OffsetAllUVs(move);
                        }
                    }
                }
            }
        }
    }
}
