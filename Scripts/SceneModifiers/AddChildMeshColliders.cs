using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AddChildMeshColliders : MonoBehaviour
{
    public bool addColliders = false;
    private void OnEnable()
    {
        if (addColliders)
        {
            MeshRenderer[] childs = transform.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in childs)
            {
                Collider col = mr.transform.GetComponent<Collider>();
                if (col == null)
                {
                    mr.gameObject.AddComponent<MeshCollider>();
                }
            }
        }
    }
}
