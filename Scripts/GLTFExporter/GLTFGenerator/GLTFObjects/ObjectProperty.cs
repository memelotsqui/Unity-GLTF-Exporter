using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    [Serializable]
    public class ObjectProperty
    {
        public enum PropertyType {Float,ColorRGBA,ColorRGB,Bool,String,StringArray,StringList,IntArray, IntList,FloatArray,Object, ObjectProperty, Array,EmptyHolder,EmptyObject,Int, GameObject, GameObjectArray, GameObjectList, Vector2,Vector3,Quaternion,Extension,Extra}
        public PropertyType propertyType;
        public string propertyName;
        //public ObjectTextureOld parameterTexture;
        public float propertyFloat;
        public string propertyString;
        public string[] propertyStringArray;
        public List<string> propertyStringList;
        public int[] propertyIntArray;
        public List<int> propertyIntList;
        public float[] propertyFloatArray;
        public Color propertyColor;
        public bool propertyBool;
        public Vector2 propertyVector2;
        public Vector3 propertyVector3;
        public Quaternion propertyQuaternion;
        public int propertyInt;
        public GameObject propertyGameObject;
        public GameObject[] propertyGameObjectArray;
        public List<GameObject> propertyGameObjectList;
        public ObjectExtension objectExtension;
        public ObjectExtraProperties propertyExtra;
        public ObjectProperty propertyObject;
        public List<ObjectProperty> propertyObjects;
        public List<ObjectExtension> extensionObjects;

        public bool exportInGLTF;


        public void AddExtensionObject(ObjectExtension object_extension)
        {
            if (object_extension != null)
            {
                if (extensionObjects == null)
                    extensionObjects = new List<ObjectExtension>();

                extensionObjects.Add(object_extension);
            }
        }
        public ObjectProperty(string property_name)        // USED TO STORE AT THE ROOT OF AN OBJECT, EXAMPLE: IN TEXTURES
        {
            propertyName = property_name;
            propertyType = PropertyType.EmptyObject;
        }
        public ObjectProperty(string property_name, Vector2 property_vector2, bool ExportInGLTF = true)
        {
            propertyName = property_name;
            propertyVector2 = property_vector2;
            propertyType = PropertyType.Vector2;
        }
        public ObjectProperty(string property_name, Vector3 property_vector3, bool negativeZ = true)
        {
            propertyName = property_name;
            if (negativeZ)
                propertyVector3 = new Vector3(property_vector3.x, property_vector3.y, -property_vector3.z);
            else
                propertyVector3 = property_vector3;
            propertyType = PropertyType.Vector3;
        }
        public ObjectProperty(string property_name, Quaternion property_quaternion)
        {
            propertyName = property_name;
            propertyQuaternion = property_quaternion;
            propertyType = PropertyType.Quaternion;
        }
        public ObjectProperty(string property_name, List<ObjectProperty> property_object, bool isArray = false)
        {
            propertyName = property_name;
            propertyObjects = property_object;
            if (!isArray)
                propertyType = PropertyType.Object;
            else
                propertyType = PropertyType.Array;
        }
        public ObjectProperty(string property_name, ObjectProperty property_object)
        {
            propertyName = property_name;
            propertyObject = property_object;
            propertyType = PropertyType.ObjectProperty;

        }

        public ObjectProperty(ObjectExtraProperties property_extra)
        {
            propertyName = "extras";
            propertyExtra = property_extra;
            propertyType = PropertyType.Extra;
        }
        public ObjectProperty(string property_name, GameObject property_game_object)
        {
            propertyName = property_name;
            propertyGameObject = property_game_object;
            propertyType = PropertyType.GameObject;
        }
        public ObjectProperty(string property_name, int property_int)
        {
            propertyName = property_name;
            propertyInt = property_int;
            propertyType = PropertyType.Int;
        }
        public ObjectProperty(string property_name, GameObject[] property_game_object_array)
        {
            propertyName = property_name;
            propertyGameObjectArray = property_game_object_array;
            propertyType = PropertyType.GameObjectArray;
        }
        public ObjectProperty(string property_name, List<GameObject> property_game_object_list)
        {
            propertyName = property_name;
            propertyGameObjectList = property_game_object_list;
            propertyType = PropertyType.GameObjectList;
        }
        public ObjectProperty(string property_name, int[] property_int_array)
        {
            propertyName = property_name;
            propertyIntArray = property_int_array;
            propertyType = PropertyType.IntArray;
        }
        public ObjectProperty(string property_name, List<int> property_int_list)
        {
            propertyName = property_name;
            propertyIntList = property_int_list;
            propertyType = PropertyType.IntList;
        }
        public ObjectProperty(string property_name, string property_string, bool ExportInGLTF =true)
        {
            propertyName = property_name;
            propertyString = property_string;
            propertyType = PropertyType.String;
        }
        public ObjectProperty(string property_name, string[] property_string_array)
        {
            propertyName = property_name;
            propertyStringArray = property_string_array;
            propertyType = PropertyType.StringArray;
        }
        public ObjectProperty(string property_name, List<string> property_string_list)
        {
            propertyName = property_name;
            propertyStringList = property_string_list;
            propertyType = PropertyType.StringList;
        }
        public ObjectProperty(string property_name, float property_float, bool exportInGLTF =true)
        {
            propertyName = property_name;
            propertyFloat = property_float;
            propertyType = PropertyType.Float;
        }
        public ObjectProperty(string property_name, float[] property_float_array)
        {
            propertyName = property_name;
            propertyFloatArray = property_float_array;
            propertyType = PropertyType.FloatArray;
        }
        public ObjectProperty(string property_name, Color property_color, bool saveOnlyRGB = false, bool exportInGLTF=true)
        {
            propertyName = property_name;
            propertyColor = property_color;
            if (!saveOnlyRGB)
                propertyType = PropertyType.ColorRGBA;
            else
                propertyType = PropertyType.ColorRGB;
        }
        public ObjectProperty(string property_name, bool property_bool, bool exportInGLTF = true)
        {
            propertyName = property_name;
            propertyBool = property_bool;
            propertyType = PropertyType.Bool;
        }
        public ObjectProperty()        // USED TO STORE AT THE ROOT OF AN OBJECT, EXAMPLE: IN TEXTURES
        {
            propertyType = PropertyType.EmptyHolder;
        }
        //nuevo

        public string GetPropertyGLTF()
        {
            string result = "";         // FOR ARRAYS WITH UNNAMED OBJECTS
            if (propertyName != "")
                result += "\"" + propertyName + "\": ";
            switch (propertyType)
            {
                case PropertyType.EmptyObject:
                    return result + "{}";
                case PropertyType.String:
                    return result + "\"" + StringUtilities.GetSafeChars(propertyString) + "\"";
                case PropertyType.StringArray:
                    for (int i =0; i < propertyStringArray.Length; i++)
                        propertyStringArray[i] = StringUtilities.GetSafeChars(propertyStringArray[i]);
                    return result + "[" + StringUtilities.ArrayToString(propertyStringArray) + "]";
                case PropertyType.StringList:
                    for (int i = 0; i < propertyStringList.Count; i++)
                        propertyStringList[i] = StringUtilities.GetSafeChars(propertyStringList[i]);
                    return result + "[" + StringUtilities.ArrayToString(propertyStringList) + "]";
                case PropertyType.Float:
                    return result + propertyFloat;
                case PropertyType.FloatArray:
                    return result + "[" + StringUtilities.ArrayToString(propertyFloatArray) + "]";
                case PropertyType.ColorRGBA:
                    return result + "["+ propertyColor.r +","+ propertyColor.g + "," + propertyColor.b + "," + propertyColor.a + "]";
                case PropertyType.ColorRGB:
                    return result + "[" + propertyColor.r + "," + propertyColor.g + "," + propertyColor.b + "]";
                case PropertyType.Bool:
                    return result + propertyBool.ToString().ToLower();
                case PropertyType.ObjectProperty:
                    return result + "{" + propertyObject.GetPropertyGLTF() + "}";
                case PropertyType.Object:
                    return result + GetObjectProperties(propertyObjects,extensionObjects);
                case PropertyType.Array:
                    return result + "[" + GetObjectProperties(propertyObjects, extensionObjects,true) + "]";
                case PropertyType.Extra:
                    return result + GetObjectProperties(propertyExtra.extrasProperties);
                case PropertyType.Int:
                    return result + propertyInt;
                case PropertyType.GameObject:
                    return result + GetGameObjectIndex(propertyGameObject);
                case PropertyType.GameObjectArray:
                    return result + "[" + GetGameObjectArrayIndex() +"]";
                case PropertyType.GameObjectList:
                    return result + "[" + GetGameObjectListIndex() +"]";
                case PropertyType.IntArray:
                    return result + "[" + StringUtilities.ArrayToString(propertyIntArray) + "]";
                case PropertyType.IntList:
                    return result + "[" + StringUtilities.ArrayToString(propertyIntList) + "]";
                case PropertyType.Vector2:
                    return result + "["+ StringUtilities.Vector2ToString(propertyVector2,false) +"]";
                case PropertyType.Vector3:
                    return result + "[" + StringUtilities.Vector3ToString(propertyVector3, false) + "]";
                case PropertyType.Quaternion:
                    return result + "[" + StringUtilities.QuaternionToString(propertyQuaternion,false) + "]";
                case PropertyType.EmptyHolder:
                    result = GetExtensionPropertyGLTF(extensionObjects);
                    result = StringUtilities.RemoveCharacterFromString(result, 2, false);
                    return result;
                default:
                    return "";
            }
        }
        
        private string GetGameObjectArrayIndex()
        {
            string result = "";
            if (propertyGameObjectArray != null) {
                if (propertyGameObjectArray.Length == 0)
                    return "";
                else
                {
                    foreach (GameObject go in propertyGameObjectArray)
                    {
                        result += GetGameObjectIndex(go) + ",";
                    }
                    result = StringUtilities.RemoveCharacterFromString(result,1,false);
                }
            }
            return result;
        }
        private string GetGameObjectListIndex()
        {
            string result = "";
            if (propertyGameObjectList != null)
            {
                if (propertyGameObjectList.Count == 0)
                    return "";
                else
                {
                    foreach (GameObject go in propertyGameObjectList)
                    {
                        result += GetGameObjectIndex(go) + ",";
                    }
                    result = StringUtilities.RemoveCharacterFromString(result, 1, false);
                }
            }
            return result;
        }
        private string GetGameObjectIndex(GameObject tarObject)
        {
            if (tarObject != null)
            {
                ObjectNodeMono onm = tarObject.GetComponent<ObjectNodeMono>();
                if (onm != null)
                    return onm.node.ToString();
            }

            return "-1";
        }
        public static string GetExtensionPropertyGLTF(List<ObjectExtension> object_extensions)
        {
            string result = "";
            if (object_extensions != null)
            {
                if (object_extensions.Count > 0)
                {
                    result += "\"extensions\":{\n";
                    for (int i = 0; i < object_extensions.Count; i++)
                    {
                        result += "\"" + object_extensions[i].name + "\":";
                        result += GetObjectProperties(object_extensions[i].properties);
                        result += ",\n";
                    }
                    result = StringUtilities.RemoveCharacterFromString(result, 2, false);
                    result += "},\n";       // ADDED HERE TO PREVENT ERRORS LATER, THIS ONES GETS ALWAYS REMOVED, BUT MUST BE ADDED TO AVOID ERRORS AND GO WITH THE FLOW OF GETTING OBJECT PROPERTIES
                }
            }
            return result;
        }
        public static string GetObjectProperties(List<ObjectProperty> object_properties, List<ObjectExtension> object_extensions = null, bool isArray =false)
        {
            string result = "";
            if (object_properties != null)
            {

                if (object_properties.Count > 0)
                {
                    if (!isArray)
                        result += "{\n";
                    for (int i = 0; i < object_properties.Count; i++)
                    {
                        result += object_properties[i].GetPropertyGLTF();
                        result += ",\n";
                    }
                    result += GetExtensionPropertyGLTF(object_extensions);
                    result = StringUtilities.RemoveCharacterFromString(result, 2, false);
                    if (!isArray)
                        result += "\n}";

                    
                }
                else
                {
                    result += "{}\n";
                }
            }
            return result;
        }
        public static string GetObjectProperties(ObjectProperty object_property)
        {
            string result = "";
            if (object_property != null)
            {
                result += "{\n";
                result += object_property.GetPropertyGLTF();
                result += "\n}";
            }
            return result;
        }


        // PENDING TO SEE IF WE WILL USE IT
        //public string GetMaterialPropertyTHREEJS()
        //{
            
        //    string result = parameterName + ": ";
        //    switch (propertyType)
        //    {
        //        case PropertyType.Texture:
        //            return result + parameterTexture.exportName;
        //        case PropertyType.Float:
        //            return result + propertyFloat;
        //        case PropertyType.ColorRGBA:
        //            return result + "0x" + ColorUtility.ToHtmlStringRGBA(propertyColor);
        //        case PropertyType.ColorRGB:
        //            return result + "0x" + ColorUtility.ToHtmlStringRGB(propertyColor);
        //        case PropertyType.Bool:
        //            return result + propertyBool.ToString().ToLower();
        //        default:
        //            return "";
        //    }
        //}
    }
}
