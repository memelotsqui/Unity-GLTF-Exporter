using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtraProperties
    { 
        public List<ObjectProperty> extrasProperties;

        public ObjectExtraProperties(List<ObjectProperty> existing_properties = null)
        {
            if (existing_properties == null)
                extrasProperties = new List<ObjectProperty>();
            else
                extrasProperties = existing_properties;
        }
        public void Add(ObjectProperty object_property)
        {
            if (object_property != null)
                extrasProperties.Add(object_property);
        }
    }
}
