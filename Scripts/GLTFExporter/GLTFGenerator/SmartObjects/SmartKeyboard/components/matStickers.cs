using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF.SMART.meme_SmartKeyboard
{
    public class matStickers : ObjectNodeUserExtrasMono
    {
        private void Reset()
        {
            displayOptions = false;
            extrasName = "matStickers";

            tooltip = "Add this to the key labels, must have the mesh renderer";
        }
    }
}
