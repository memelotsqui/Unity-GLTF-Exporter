using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    [ExecuteAlways]
    public class ObjectNodeUserExtrasMono : MonoBehaviour
    {
        [HideInInspector]
        public string tooltip = "";

        [HideInInspector]
        public string extrasName;
        [SerializeField,HideInInspector]
        public List<ObjectProperty> computedProperties;
        [SerializeField, HideInInspector]
        public List<ObjectProperty> userProperties;

        [HideInInspector]
        public GameObject[] gameObjectArrayVal;
        [HideInInspector]
        public float[] floatArrayVal;
        [HideInInspector]
        public int[] intArrayVal;
        [HideInInspector]
        public string[] stringArrayVal;

        [HideInInspector]
        public bool displayOptions = true;

        [HideInInspector]
        public int optionalIdentifier = -1;
        
        public virtual void ResetComputedProperties()
        {
            computedProperties = new List<ObjectProperty>();
        }
        public void AddProperty(ObjectProperty object_property, bool computed = true, int index = 0)
        {
            if (index < 0)
                index = 0;
            if (computed)
            {
                if (computedProperties == null)
                    computedProperties = new List<ObjectProperty>();
                computedProperties.Insert(index, object_property);
            }
            else
            {
                if (userProperties == null)
                    userProperties = new List<ObjectProperty>();

                userProperties.Insert(index,object_property);
            }
        }
        public void CombineProperties()
        {
            if (computedProperties == null)
                computedProperties = new List<ObjectProperty>();
            if (userProperties != null)
                computedProperties.AddRange(userProperties);
        }
        public virtual void SetGLTFComputedData()   // Called before creating gltf file
        {
            // add property...
            // set always computed to true, only user interaction will set it to false to avoid deletion
        }
        //public virtual void SetGLTFLastData()
        //{
        //    // not used yet, its called before saving gltf data
        //}

    }
}
