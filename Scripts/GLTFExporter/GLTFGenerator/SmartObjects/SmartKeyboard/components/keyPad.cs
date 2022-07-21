using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER.GLTF.SMART.meme_SmartKeyboard
{
    public class keyPad : ObjectNodeUserExtrasMono
    {
        public enum KeyType { key = 0, submit = 1, switcher = 2, delete = 3, supreme = 4, close = 5, home = 6 , exitvr= 7}
        public KeyType keyType = KeyType.key;
        public string value = "";
        public GameObject[] turnOff;
        public GameObject[] turnOn;
        private void Reset()
        {
            displayOptions = false;
            extrasName = "keyPad";

            tooltip = "Choose the type of keys, and if required, the additional data";
        }
        public override void SetGLTFComputedData()
        {
            AddProperty(new ObjectProperty("keyType", keyType.ToString()));
            if (keyType == KeyType.key)
            {
                AddProperty(new ObjectProperty("value", value));
            }
            if (keyType == KeyType.switcher)
            {
                foreach(GameObject go in turnOff)
                {
                    Debug.Log(go.GetComponent<ObjectNodeMono>());
                }
                foreach (GameObject go in turnOn)
                {
                    Debug.Log(go.GetComponent<ObjectNodeMono>());
                }

                AddProperty(new ObjectProperty("enable", turnOn));
                AddProperty(new ObjectProperty("disable", turnOff));

            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(keyPad), true)]
    public class keyPadEditor : Editor
    {
        keyPad myScript;
        MonoScript script;

        private void OnEnable()
        {
            myScript = (keyPad)target;
            script = MonoScript.FromMonoBehaviour((keyPad)target);

        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false);
            //base.OnInspectorGUI();
            myScript.keyType = (keyPad.KeyType)EditorGUILayout.EnumPopup("Key Type", myScript.keyType);
            if (myScript.keyType == keyPad.KeyType.key)
                myScript.value = EditorGUILayout.TextField("Value: ", myScript.value);
            
            if (myScript.keyType == keyPad.KeyType.switcher)
            {
                serializedObject.Update();
                var turnOff = serializedObject.FindProperty("turnOff");
                var turnOn = serializedObject.FindProperty("turnOn");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(turnOn, true);
                EditorGUILayout.PropertyField(turnOff, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}

