using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    [CustomEditor (typeof(ObjectNodeUserExtrasMono),true)]
    public class ObjectNodeUserExtrasMono_Editor : Editor
    {
        ObjectNodeUserExtrasMono myScript;
        MonoScript script;

        bool boolVal;
        int intVal;
        float floatVal;
        Color colorVal;
        Vector2 vector2Val;
        Vector3 vector3Val;
        string stringVal;
        GameObject gameObjectVal;

        bool convertedToRad = false;

        ObjectProperty editingProperty;
        int editPropertyIndex;

        string propName;


        public enum property {NONE, BOOL, INT, FLOAT, STRING, COLOR, VECTOR2, VECTOR3, STRING_ARRAY, INT_ARRAY, FLOAT_ARRAY ,GAMEOBJECT, GAMEOBJECT_ARRAY }
        property selectedProperty;
        private void OnEnable()
        {
            myScript = (ObjectNodeUserExtrasMono)target;
            script = MonoScript.FromMonoBehaviour(myScript);
            selectedProperty = property.NONE;

            myScript.floatArrayVal = new float[0];
            myScript.intArrayVal = new int[0];
            myScript.stringArrayVal = new string[0];
            myScript.gameObjectArrayVal = new GameObject[0];
        }
        private void OnDisable()
        {
            if (editingProperty != null)
            {
                if (editingProperty.propertyType != ObjectProperty.PropertyType.EmptyHolder)
                {
                    myScript.userProperties.Insert(editPropertyIndex, editingProperty);
                    editingProperty = null;
                    editPropertyIndex = -1;
                }
            }
        }
        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            if (myScript.tooltip != "")
            {
                GUILayout.Box(myScript.tooltip,GUILayout.Width(GuiLayoutExtras.GetCenteredLabelWidth(50f)));
            }
            if (myScript.displayOptions)
            {
                //EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false);
                serializedObject.Update();
                SerializedProperty gameObjectArr = serializedObject.FindProperty("gameObjectArrVal");
                SerializedProperty floatArr = serializedObject.FindProperty("floatArrayVal");
                SerializedProperty intArr = serializedObject.FindProperty("intArrayVal");
                SerializedProperty stringArr = serializedObject.FindProperty("stringArrayVal");


                EditorGUI.BeginChangeCheck();
                string _extrasName = myScript.extrasName;
                _extrasName = EditorGUILayout.TextField("Extras Name", myScript.extrasName);

                if (_extrasName != "")
                {
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
                        //case property.FLOAT_TO_RAD:
                        //    floatVal = EditorGUILayout.FloatField("float to rad", floatVal);
                        //    break;
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
                        case property.INT_ARRAY:
                            EditorGUILayout.PropertyField(intArr, true);
                            break;
                        case property.STRING_ARRAY:
                            EditorGUILayout.PropertyField(stringArr, true);
                            break;
                        case property.FLOAT_ARRAY:
                            EditorGUILayout.PropertyField(floatArr, true);
                            break;
                        case property.GAMEOBJECT:
                            gameObjectVal = (GameObject)EditorGUILayout.ObjectField("Game Object", gameObjectVal, typeof(GameObject), true);
                            break;
                        case property.GAMEOBJECT_ARRAY:
                            EditorGUILayout.PropertyField(gameObjectArr, true);
                            break;
                    }
                    
                    if (propName != "" && selectedProperty != property.NONE && propName != null)
                    {
                        if (selectedProperty == property.FLOAT || selectedProperty == property.VECTOR2 || selectedProperty == property.VECTOR3)
                        {

                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Degree to Rad", GUILayout.Height(20f)))
                            {
                                EditorGUI.FocusTextInControl(null);
                                Undo.RecordObject(myScript, "Degree to Rad");
                                switch (selectedProperty)
                                {
                                    case property.FLOAT:
                                        floatVal = floatVal * (Mathf.PI / 180);
                                        break;
                                    case property.VECTOR2:
                                        vector2Val = vector2Val * (Mathf.PI / 180);
                                        break;
                                    case property.VECTOR3:
                                        vector3Val = vector3Val * (Mathf.PI / 180);
                                        break;
                                }
                            }
                            if (GUILayout.Button("Rad to Degree", GUILayout.Height(20f)))
                            {
                                EditorGUI.FocusTextInControl(null);
                                Undo.RecordObject(myScript, "Rad to Degree");
                                switch (selectedProperty)
                                {
                                    case property.FLOAT:
                                        floatVal = floatVal * (180 / Mathf.PI);
                                        break;
                                    case property.VECTOR2:
                                        vector2Val = vector2Val * (180 / Mathf.PI);
                                        break;
                                    case property.VECTOR3:
                                        vector3Val = vector3Val * (180 / Mathf.PI);
                                        break;
                                }

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        //EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add", GUILayout.Height(20f)))
                        {
                            EditorGUI.FocusTextInControl(null);
                            bool reset_values = true;
                            Undo.RecordObject(myScript, "Add Property");
                            switch (selectedProperty)
                            {
                                case property.BOOL:
                                    myScript.AddProperty(new ObjectProperty(propName, boolVal),false, editPropertyIndex);
                                    boolVal = false;
                                    break;
                                case property.INT:
                                    myScript.AddProperty(new ObjectProperty(propName, intVal), false, editPropertyIndex);
                                    intVal = 0;
                                    break;
                                case property.FLOAT:
                                    myScript.AddProperty(new ObjectProperty(propName, floatVal), false, editPropertyIndex);
                                    floatVal = 0;
                                    break;
                                //case property.FLOAT_TO_RAD:
                                //    myScript.AddProperty(new ObjectProperty(propName, floatVal * (Mathf.PI / 180)), false, editPropertyIndex);
                                //    floatVal = 0;
                                //    break;
                                case property.STRING:
                                    myScript.AddProperty(new ObjectProperty(propName, stringVal), false, editPropertyIndex);
                                    stringVal = "";
                                    break;
                                case property.VECTOR2:
                                    myScript.AddProperty(new ObjectProperty(propName, vector2Val), false, editPropertyIndex);
                                    vector2Val = Vector2.zero;
                                    break;
                                case property.VECTOR3:
                                    myScript.AddProperty(new ObjectProperty(propName, vector3Val), false, editPropertyIndex);
                                    vector3Val = Vector3.zero;
                                    break;
                                case property.COLOR:
                                    myScript.AddProperty(new ObjectProperty(propName, colorVal), false, editPropertyIndex);
                                    break;
                                case property.GAMEOBJECT:
                                    myScript.AddProperty(new ObjectProperty(propName, gameObjectVal), false, editPropertyIndex);
                                    break;
                                case property.GAMEOBJECT_ARRAY:
                                    if (myScript.gameObjectArrayVal.Length == 0)
                                    {
                                        Debug.LogWarning("ADD AT LEAST 1 GAMEOBJECT VALUE");
                                        reset_values = false;
                                        break;
                                    }
                                    myScript.AddProperty(new ObjectProperty(propName, myScript.gameObjectArrayVal), false, editPropertyIndex);
                                    break;
                                case property.INT_ARRAY:
                                    if (myScript.intArrayVal.Length == 0)
                                    {
                                        Debug.LogWarning("ADD AT LEAST 1 INT VALUE");
                                        reset_values = false;
                                        break;
                                    }
                                    myScript.AddProperty(new ObjectProperty(propName, myScript.intArrayVal), false, editPropertyIndex);
                                    break;
                                case property.FLOAT_ARRAY:
                                    if (myScript.floatArrayVal.Length == 0)
                                    {
                                        Debug.LogWarning("ADD AT LEAST 1 FLOAT VALUE");
                                        reset_values = false;
                                        break;
                                    }
                                    myScript.AddProperty(new ObjectProperty(propName, myScript.floatArrayVal), false, editPropertyIndex);
                                    break;
                                case property.STRING_ARRAY:
                                    if (myScript.stringArrayVal.Length == 0)
                                    {
                                        Debug.LogWarning("ADD AT LEAST 1 STRING VALUE");
                                        reset_values = false;
                                        break;
                                    }
                                    myScript.AddProperty(new ObjectProperty(propName, myScript.stringArrayVal), false, editPropertyIndex);
                                    break;
                            }
                            editPropertyIndex = -1;
                            editingProperty = null;
                            if (reset_values)
                                propName = "";
                        }
                        
                    }
                    else
                    {
                        GUILayout.Label("", GUILayout.Height(20f)); ;
                    }
                    // == show values here == //

                    if (myScript.userProperties != null)
                    {
                        for (int i = 0; i < myScript.userProperties.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label(myScript.userProperties[i].propertyType.ToString(), GUILayout.Width(80f));
                            string label = myScript.userProperties[i].GetPropertyGLTF();
                            if (myScript.userProperties[i].propertyType == ObjectProperty.PropertyType.GameObject)
                                if (myScript.userProperties[i].propertyGameObject != null)
                                    label += " (" + myScript.userProperties[i].propertyGameObject.name + ")";
                            if (myScript.userProperties[i].propertyType == ObjectProperty.PropertyType.GameObjectArray)
                            {
                                if (myScript.userProperties[i].propertyGameObjectArray != null)
                                {
                                    label += " (";
                                    foreach (GameObject go in myScript.userProperties[i].propertyGameObjectArray)
                                    {
                                        if (go != null)
                                        {
                                            label += go.name + ",";
                                        }
                                    }
                                    //te
                                    Debug.Log(label);
                                    if (label.EndsWith(","))
                                        label = StringUtilities.RemoveCharacterFromString(label, 1, false);
                                    label += ")";
                                }
                            }
                            GUILayout.Label(label, GUILayout.MaxWidth(300f));
                            if (GUILayout.Button("E", GUILayout.Width(20f)))
                            {
                                //Undo.RegisterCompleteObjectUndo(myScript, "Edit Property");
                                //Undo.RecordObject(myScript, "Edit Property");
                                editingProperty = myScript.userProperties[i];
                                editPropertyIndex = i;
                                selectedProperty = GetPropertyType(myScript.userProperties[i].propertyType);
                                SetPropertyValue(editingProperty);
                                propName = myScript.userProperties[i].propertyName;
                                myScript.userProperties.RemoveAt(i);
                            }
                            if (GUILayout.Button("X", GUILayout.Width(20f)))
                            {
                                Undo.RecordObject(myScript, "Remove Property");
                                myScript.userProperties.RemoveAt(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(myScript, "Assign Extras Values");
                    serializedObject.ApplyModifiedProperties();
                    myScript.extrasName = _extrasName;
                }
            }
        }
        private void SetPropertyValue(ObjectProperty prop)
        {
            switch (prop.propertyType)
            {
                case ObjectProperty.PropertyType.Bool:
                    boolVal = prop.propertyBool;
                    break;
                case ObjectProperty.PropertyType.Int:
                    intVal = prop.propertyInt;
                    break;
                case ObjectProperty.PropertyType.Float:
                    floatVal = prop.propertyFloat;
                    break;
                case ObjectProperty.PropertyType.String:
                    stringVal = prop.propertyString;
                    break;
                case ObjectProperty.PropertyType.Vector2:
                    vector2Val = prop.propertyVector2;
                    break;
                case ObjectProperty.PropertyType.Vector3:
                    vector3Val = prop.propertyVector3;
                    break;
                case ObjectProperty.PropertyType.ColorRGBA:
                case ObjectProperty.PropertyType.ColorRGB:
                    colorVal = prop.propertyColor;
                    break;
                case ObjectProperty.PropertyType.IntArray:
                    //intArray = prop.propertyIntArray;
                    break;
                case ObjectProperty.PropertyType.StringArray:
                    //stringarray = prop.propertyStringArray;
                    break;
                case ObjectProperty.PropertyType.FloatArray:
                    //floatarray = prop.propertyFloatArray;
                    break;
                case ObjectProperty.PropertyType.GameObject:
                    gameObjectVal = prop.propertyGameObject;
                    break;
                case ObjectProperty.PropertyType.GameObjectArray:
                    //gameObjectArray = prop.propertyGameObjectArray;
                    break;
                default:
                    //return property.NONE;
                    break;
            }
        }
        private property GetPropertyType(ObjectProperty.PropertyType propType)
        {
            switch (propType)
            {
                case ObjectProperty.PropertyType.Bool:
                    return property.BOOL;
                case ObjectProperty.PropertyType.Int:
                    return property.INT;
                case ObjectProperty.PropertyType.Float:
                    return property.FLOAT;
                case ObjectProperty.PropertyType.String:
                    return property.STRING;
                case ObjectProperty.PropertyType.Vector2:
                    return property.VECTOR2;
                case ObjectProperty.PropertyType.Vector3:
                    return property.VECTOR3;
                case ObjectProperty.PropertyType.ColorRGBA: 
                case ObjectProperty.PropertyType.ColorRGB:
                    return property.COLOR;
                case ObjectProperty.PropertyType.IntArray:
                    return property.INT_ARRAY;
                case ObjectProperty.PropertyType.StringArray:
                    return property.STRING_ARRAY;
                case ObjectProperty.PropertyType.FloatArray:
                    return property.FLOAT_ARRAY;
                case ObjectProperty.PropertyType.GameObject:
                    return property.GAMEOBJECT;
                case ObjectProperty.PropertyType.GameObjectArray:
                    return property.GAMEOBJECT_ARRAY;
                default:
                    return property.NONE;

            }
        }
    }
}
