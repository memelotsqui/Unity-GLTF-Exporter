using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WEBGL_EXPORTER
{
    public class SwitchChildMaterials : MonoBehaviour
    {
        public Material originalMaterial;
        public Material targetMaterial;
        // Start is called before the first frame update
        public void SwitchToTargetMaterial()
        {
            Debug.Log("totarget");
            ChangeToMaterial(originalMaterial, targetMaterial);
        }
        public void SwitchToOriginalMaterial()
        {
            Debug.Log("tooriginial");
            ChangeToMaterial(targetMaterial, originalMaterial);
        }
        public void ChangeToMaterial(Material from, Material to)
        {
            Debug.Log(from + "   " + to);
            MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in mrs)
            {
                for (int i = 0; i < mr.sharedMaterials.Length; i++)
                {
                    if (mr.sharedMaterials[i] == from)
                    {
                        Material[] mats = mr.sharedMaterials;
                        mats[i] = to;
                        mr.sharedMaterials = mats;
                    }
                }
            }
        }
    }
}
