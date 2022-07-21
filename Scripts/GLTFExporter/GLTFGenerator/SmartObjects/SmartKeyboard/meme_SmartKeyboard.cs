using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF.SMART.meme_SmartKeyboard
{
    public class meme_SmartKeyboard : Mono_ExportToGLTF_SmartObject
    {
        public InputField targetinputField;
        private void Reset()
        {
            displayClassName = false;
            smartObjectClassName = "meme_SmartKeyboard";
            smartType = SmartType.ui;
        }
        public override List<ObjectProperty> SetupCustomExtrasData()
        {
            List<ObjectProperty> customData = new List<ObjectProperty>();
            if (targetinputField != null)
            {
                customData.Add(new ObjectProperty("targetInputField", targetinputField.gameObject));
            }
            return customData;
        }
    }
}
