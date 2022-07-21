using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER.GLTF 
{ 
    [ExecuteAlways]
    public class ObjectMasterUserExtrasMono : MonoBehaviour
    {
        [SerializeField]
        public List<ObjectProperty> properties;

        public GameObject[] gameObjectArrayVal;
        public float[] floatArrayVal;
        public int[] intArrayVal;
        public string[] stringArrayVal;

        public bool checkedDuplicate = false;

        private void OnEnable()
        {
            if (!checkedDuplicate) 
            {
                ObjectMasterUserExtrasMono[] _oms = transform.root.GetComponentsInChildren<ObjectMasterUserExtrasMono>();
                if (_oms.Length > 1)
                {
                    Debug.LogWarning("MASTER USER DEFINED EXTRAS ALREADY EXIST, REMOVING AND SELECTING EXISTING");
#if UNITY_EDITOR
                    if (_oms[0] == this)
                        Selection.activeGameObject = _oms[1].gameObject;
                    else
                        Selection.activeGameObject = _oms[0].gameObject;
                    DestroyImmediate(this);
#endif
                }
                else
                {
                    checkedDuplicate = true;
                }
            }

        }
        public void AddProperty(ObjectProperty object_property)
        {
            if (properties == null)
                properties = new List<ObjectProperty>();

            properties.Add(object_property);
        }
        public void AddPropertiesToMaster()
        {
            if (properties != null)
                ObjectMasterExtras.AddList(properties);
        }
    }
}
