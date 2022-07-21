using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectNodeTeleport : ObjectNodeUserExtrasMono
    {
        // Start is called before the first frame update
        private void OnEnable()
        {
            displayOptions = false;
            extrasName = "teleport";
            tooltip = "Teleports user on VR click";
        }

    }
}
