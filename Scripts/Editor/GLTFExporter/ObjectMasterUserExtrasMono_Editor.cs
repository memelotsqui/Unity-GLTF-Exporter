using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor(typeof(ObjectMasterUserExtrasMono))]
    public class ObjectMasterUserExtrasMono_Editor : Editor
    {
        ObjectMasterUserExtrasMono myScript;
        MonoScript script;

        bool boolVal;
        int intVal;
        float floatVal;
        Color colorVal;
        Vector2 vector2Val;
        Vector3 vector3Val;
        string stringVal;
        GameObject gameObjectVal;

        SerializedProperty floatArr;
        SerializedProperty intArr;
        SerializedProperty stringArr;


        string propName;

        public enum property { NONE, BOOL, INT, FLOAT, FLOAT_TO_RAD, STRING, COLOR, VECTOR2, VECTOR3, STRING_ARRAY, INT_ARRAY, FLOAT_ARRAY, GAMEOBJECT, GAMEOBJECT_ARRAY }
        property selectedProperty;
        private void OnEnable()
        {
            myScript = (ObjectMasterUserExtrasMono)target;
            script = MonoScript.FromMonoBehaviour(myScript);
            selectedProperty = property.NONE;

            myScript.floatArrayVal = new float[0];
            myScript.intArrayVal = new int[0];
            myScript.stringArrayVal = new string[0];
            myScript.gameObjectArrayVal = new GameObject[0];


        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false);

            serializedObject.Update();
            SerializedProperty floatArr = serializedObject.FindProperty("floatArrayVal");
            SerializedProperty intArr = serializedObject.FindProperty("intArrayVal");
            SerializedProperty stringArr = serializedObject.FindProperty("stringArrayVal");
            SerializedProperty gameObjectArr = serializedObject.FindProperty("gameObjectArrayVal");

            EditorGUI.BeginChangeCheck();
            
            selectedProperty = (property)EditorGUILayout.EnumPopup(selectedProperty, GUILayout.Height(20f));
            if (selectedProperty != property.NONE)
                propName = EditorGUILayout.TextField("Property Name", propName);


            switch (selectedProperty)
            {
                case property.BOOL:
                    boolVal = EditorGUILayout.Toggle("bool", boolVal);
                    break;
                case property.INT:
                    intVal = EditorGUILayout.IntField("int", intVal);
                    break;
                case property.FLOAT:
                    floatVal = EditorGUILayout.FloatField("float", floatVal);
                    break;
                case property.FLOAT_TO_RAD:
                    floatVal = EditorGUILayout.FloatField("float to rad", floatVal);
                    break;
                case property.STRING:
                    stringVal = EditorGUILayout.TextField("string", stringVal);
                    break;
                case property.VECTOR2:
                    vector2Val = EditorGUILayout.Vector2Field("vector2", vector2Val);
                    break;
                case property.VECTOR3:
                    vector3Val = EditorGUILayout.Vector3Field("vector3", vector3Val);
                    break;
                case property.COLOR:
                    colorVal = EditorGUILayout.ColorField("color", colorVal);
                    break;
                case property.STRING_ARRAY:
                    EditorGUILayout.PropertyField(stringArr, true);
                    break;
                case property.FLOAT_ARRAY:
                    EditorGUILayout.PropertyField(floatArr, true);
                    break;
                case property.INT_ARRAY:
                    EditorGUILayout.PropertyField(intArr, true);
                    break;
                case property.GAMEOBJECT:
                    gameObjectVal = (GameObject)EditorGUILayout.ObjectField("Game Object", gameObjectVal, typeof(GameObject), true);
                    break;
                case property.GAMEOBJECT_ARRAY:
                    EditorGUILayout.PropertyField(gameObjectArr, true);
                    break;
                    // ARRAY CASES
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (propName != "" && selectedProperty != property.NONE)
            {
                if (GUILayout.Button("Add", GUILayout.Height(20f)))
                {
                    bool reset_values = true;
                    Undo.RecordObject(myScript, "Add Property");
                    switch (selectedProperty)
                    {
                        case property.BOOL:
                            myScript.AddProperty(new ObjectProperty(propName, boolVal));
                            boolVal = false;
                            break;
                        case property.INT:
                            myScript.AddProperty(new ObjectProperty(propName, intVal));
                            intVal = 0;
                            break;
                        case property.FLOAT:
                            myScript.AddProperty(new ObjectProperty(propName, floatVal));
                            floatVal = 0;
                            break;
                        case property.FLOAT_TO_RAD:
                            myScript.AddProperty(new ObjectProperty(propName, floatVal * (Mathf.PI / 180)));
                            floatVal = 0;
                            break;
                        case property.STRING:
                            myScript.AddProperty(new ObjectProperty(propName, stringVal));
                            stringVal = "";
                            break;
                        case property.VECTOR2:
                            myScript.AddProperty(new ObjectProperty(propName, vector2Val));
                            vector2Val = Vector2.zero;
                            break;
                        case property.VECTOR3:
                            myScript.AddProperty(new ObjectProperty(propName, vector3Val));
                            vector3Val = Vector3.zero;
                            break;
                        case property.COLOR:
                            myScript.AddProperty(new ObjectProperty(propName, colorVal));
                            break;
                        case property.GAMEOBJECT:
                            myScript.AddProperty(new ObjectProperty(propName, gameObjectVal));
                            break;
                        case property.STRING_ARRAY:
                            if (myScript.stringArrayVal.Length == 0)
                            {
                                Debug.LogWarning("ADD AT LEAST 1 STRING VALUE");
                                reset_values = false;
                            }
                            else
                                myScript.AddProperty(new ObjectProperty(propName, myScript.stringArrayVal));
                            break;
                        case property.FLOAT_ARRAY:
                            if (myScript.floatArrayVal.Length == 0)
                            {
                                Debug.LogWarning("ADD AT LEAST 1 FLOAT VALUE");
                                reset_values = false;
                            }
                            else
                                myScript.AddProperty(new ObjectProperty(propName, myScript.floatArrayVal));
                            break;
                        case property.INT_ARRAY:
                            if (myScript.intArrayVal.Length == 0)
                            {
                                Debug.LogWarning("ADD AT LEAST 1 INT VALUE");
                                reset_values = false;
                            }
                            else
                                myScript.AddProperty(new ObjectProperty(propName, myScript.intArrayVal));
                            break;
                        case property.GAMEOBJECT_ARRAY:
                            if (myScript.gameObjectArrayVal.Length == 0)
                            {
                                Debug.LogWarning("ADD AT LEAST 1 INT VALUE");
                                reset_values = false;
                            }
                            else
                                myScript.AddProperty(new ObjectProperty(propName, myScript.gameObjectArrayVal));
                            break;
                    }
                    if (reset_values)
                    {
                        propName = "";
                        myScript.intArrayVal = new int[0];
                        myScript.floatArrayVal = new float[0];
                        myScript.stringArrayVal = new string[0];
                    }
                }
            }
            else
            {
                GUILayout.Label("", GUILayout.Height(20f)); ;
            }
            // == show values here == //

            if (myScript.properties != null)
            {
                for (int i = 0; i < myScript.properties.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(myScript.properties[i].propertyType.ToString(), GUILayout.Width(80f));
                    string label = myScript.properties[i].GetPropertyGLTF();
                    if (myScript.properties[i].propertyType == ObjectProperty.PropertyType.GameObject)
                        if (myScript.properties[i].propertyGameObject != null)
                            label += " (" + myScript.properties[i].propertyGameObject.name + ")";
                    if (myScript.properties[i].propertyType == ObjectProperty.PropertyType.GameObjectArray)
                    {
                        if (myScript.properties[i].propertyGameObjectArray != null)
                        {
                            label += " (";
                            foreach (GameObject go in myScript.properties[i].propertyGameObjectArray)
                            {
                                if (go != null)
                                {
                                    label += go.name + ",";
                                }
                            }
                            label = StringUtilities.RemoveCharacterFromString(label, 1, false);
                            label += ")";
                        }
                    }
                    GUILayout.Label(label,GUILayout.MaxWidth(300f));
                    if (GUILayout.Button("X", GUILayout.Width(40f)))
                    {
                        Undo.RecordObject(myScript, "Remove Property");
                        myScript.properties.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}
