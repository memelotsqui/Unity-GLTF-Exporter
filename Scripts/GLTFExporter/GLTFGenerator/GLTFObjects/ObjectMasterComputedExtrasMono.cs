using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER.GLTF 
{ 
    [ExecuteAlways]
    public class ObjectMasterComputedExtrasMono : MonoBehaviour
    {
        public List<ObjectProperty> properties;

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
