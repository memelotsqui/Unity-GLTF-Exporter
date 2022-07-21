using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class CreateMeshColliderOnChilds : MonoBehaviour
    {
        [HideInInspector]
        public List <MeshCollider> createdMeshCollider;
        [HideInInspector]
        public List<Collider> lastCollider;
        public bool createdsColliders = false;
        public bool displayButtons = true;
        public void CreateMeshColliders()
        {
            if (createdMeshCollider == null)
                createdMeshCollider = new List<MeshCollider>();

            MeshFilter[] allChildsMF = GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in allChildsMF)                                                                  // GRAB ALL CHILDS
            {
                Collider col = mf.transform.GetComponent<Collider>();                                               // GRAB EACH CHILDS COLLIDER
                if (col == null)                                                                                    // IF THEY DONT HAVE COLLIDER
                {
                    MeshCollider mc = mf.transform.gameObject.AddComponent<MeshCollider>();                         // ADD A MESH COLLIDER AND ADD THIS MESH COLLIDER TO THE LIST
                    createdMeshCollider.Add(mc);
                }
                else
                {
                    if (col.GetType() != typeof(MeshCollider))                                                      // IF IT HAS A COLLIDER BUT ITS TYPE IS DIFFERENTE THAN MESH COLLIDER
                    {
                        MeshCollider mc = mf.gameObject.AddComponent<MeshCollider>();                               // ADD A MESH COLLIDER AND ADD IT TO THE LIST;
                        createdMeshCollider.Add(mc);

                        lastCollider.Add(col);
                        col.enabled = false;

                    }
                }
            }
            createdsColliders = true;
        }

        public void RevertMeshColliders()
        {
            if (createdMeshCollider != null)
            {
                foreach (MeshCollider mc in createdMeshCollider)
                {
                    if (mc != null)
                        DestroyImmediate(mc);
                }
                createdMeshCollider.Clear();
            }
            if (lastCollider != null)
            {
                foreach (Collider col in lastCollider)
                {
                    col.enabled = true;
                }
                lastCollider.Clear();
            }
            createdsColliders = false;
        }
    }
}
