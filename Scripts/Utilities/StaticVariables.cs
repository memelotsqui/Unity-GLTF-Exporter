using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WEBGL_EXPORTER
{
    public class StaticVariables
    {
        //pending to change, move to preferences core interprika


        public static bool exportHiddenInSceneObjects = true;
        public static Vector3 THREEPositionModifier = new Vector3(-100f, 100f, 100f); // MODIFIER TO GET THE SAME POSITION THREE JS USES
        public static float THREESizeModifier = 100f;
        public static int sliceLastCharacterCount = 5;
        public static bool combineGroups = true;
        public static string actionablesVarName = "actionables";
        public static string lightmapNonStaticTextureName = "lmbasic";

        public static float lightmapIntensityValue = 1f;
        public static string lightmapNonStaticVarName = "lmn";
        public static int groupParentDigits = 3;
        public static bool exportSimpleMaterial = false;

        // public static string clockDeltaName = "clockDelta";
        public static List<string> stringListTempHolder;

        //SETUP
        public static bool exportMayaCode = true;
        public static string mayapyLocation = "C:/Program Files/Autodesk/Maya2019/bin/mayapy.exe";
        public static string wwwLocalFolder = "C:/wamp64/www/";
        //public static string 

    }
}
