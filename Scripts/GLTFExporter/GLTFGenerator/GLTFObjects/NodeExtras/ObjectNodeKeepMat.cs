using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectNodeKeepMat : ObjectNodeUserExtrasMono
    {
        // Start is called before the first frame update
        public bool changeChilds = true;
        private void OnEnable()
        {
            displayOptions = false;
            extrasName = "keepMat";
            tooltip = "keepMat will tell three js which materials are more important than others.\n\nWhen optimizing a scene in web, materials will be reduced to basic materials, except those selected with this component";
        }

        public override void SetGLTFComputedData()
        {
            if (!changeChilds)
            {
                AddProperty(new ObjectProperty("changeChilds", changeChilds));
            }
        }

    }
}
