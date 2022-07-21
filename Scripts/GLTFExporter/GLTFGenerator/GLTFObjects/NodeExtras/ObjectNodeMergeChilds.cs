using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{

    public class ObjectNodeMergeChilds : ObjectNodeUserExtrasMono
    {
        private void OnEnable()
        {
            displayOptions = false;
            extrasName = "mergeChilds";
            tooltip = "mergeChilds will always merge the children that share the same material and that are marked as static. \n\nThis helps reduce drawcalls and optimize web performance \n\nWarning: combining childs will remove any object/action from any of its childs";
        }
    }
}
