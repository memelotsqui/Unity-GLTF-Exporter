using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF.SMART.meme_SmartKeyboard
{
    [RequireComponent(typeof(ObjectNodeTextContainer))]
    public class InputField : ObjectNodeUserExtrasMono
    {
        // Start is called before the first frame update
        private void Reset()
        {
            displayOptions = false;
            extrasName = "inputField";

            tooltip = "Input for smart keyboard";
        }
    
    }
}
