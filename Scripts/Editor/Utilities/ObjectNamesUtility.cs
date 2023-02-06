using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;


public static class ObjectNamesUtility
{
    public static Dictionary<Type, string> GetInternalInspectorTitlesCache()
    {
        Type inspectorTitlesType = typeof(ObjectNames).GetNestedType("InspectorTitles", BindingFlags.Static | BindingFlags.NonPublic);
        var inspectorTitlesField = inspectorTitlesType.GetField("s_InspectorTitles", BindingFlags.Static | BindingFlags.NonPublic);
        return (Dictionary<Type, string>)inspectorTitlesField.GetValue(null);
    }
}
