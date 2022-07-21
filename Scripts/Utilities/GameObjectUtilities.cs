using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class GameObjectUtilities
    {
        public static List<GameObject> GetTopHierarchyChilds(GameObject parentObject, List<GameObject> existingList = null, bool includeInactive = false)
        {

            List<GameObject> result = null;
            if (existingList != null)
                result = existingList;
            else
                result = new List<GameObject>();
            if (parentObject != null)
            {
                Transform[] childs = parentObject.GetComponentsInChildren<Transform>(includeInactive);
                foreach (Transform t in childs)
                {
                    if (t.parent == parentObject.transform)
                    {
                        result.Add(t.gameObject);
                    }
                }
            }
            return result;
        }
        public static List<GameObject> GetNonRepeatingObjectList(GameObject tarObject, List<GameObject> existingList = null)
        {
            List<GameObject> result = null;
            if (existingList != null)
                result = existingList;
            else
                result = new List<GameObject>();
            bool newObject = true;
            if (tarObject != null)
            {
                foreach (GameObject go in result)
                {
                    if (go == tarObject)
                    {
                        newObject = false;
                        break;
                    }
                }
            }
            if (newObject)
                result.Add(tarObject);

            return result;
        }
        public static GameObject[] CombineArraysNoRepeatingObjects(GameObject[] arr1, GameObject[] arr2)
        {

            if (arr1 == null)
            {
                arr1 = new GameObject[0];
            }
            if (arr2 == null)
            {
                return arr1;
            }
            List<GameObject> finalList = new List<GameObject>();
            foreach (GameObject go in arr1)
            {
                if (go != null)
                    finalList.Add(go);
            }
            foreach (GameObject go2 in arr2)
            {
                if (go2 != null)
                {
                    bool newObject = true;
                    foreach (GameObject go1 in arr1)
                    {
                        if (go2 == go1)
                        {
                            newObject = false;
                            break;
                        }
                    }
                    if (newObject) finalList.Add(go2);
                }
            }

            GameObject[] listArray = new GameObject[finalList.Count];
            for (int i = 0; i < finalList.Count; i++)
            {
                listArray[i] = finalList[i];
            }
            return listArray;
        }
        public static GameObject[] RemoveObjectsFromArray(GameObject[] target, GameObject[] remove)
        {
            if (target == null)
                return null;
            if (remove == null)
                return target;
            List<GameObject> finalList = new List<GameObject>();
            foreach (GameObject go1 in target)
            {
                bool originalObject = true;
                foreach (GameObject go2 in remove)
                {
                    if (go2 == go1)
                    {
                        originalObject = false;
                        break;
                    }
                }
                if (originalObject) finalList.Add(go1);

            }

            GameObject[] listArray = new GameObject[finalList.Count];
            for (int i = 0; i < finalList.Count; i++)
            {
                listArray[i] = finalList[i];
            }
            return listArray;
        }
        public static GameObject[] RemoveNullObjects(GameObject[] target)
        {
            List<GameObject> finalList = new List<GameObject>();
            foreach(GameObject go in target)
            {
                if (go != null)
                {
                    finalList.Add(go);
                }
            }
            if (finalList.Count == target.Length)
                return target;
            else
            {
                GameObject[] listArray = new GameObject[finalList.Count];
                for (int i = 0; i < finalList.Count; i++)
                {
                    listArray[i] = finalList[i];
                }
                return listArray;
            }
        }
        public static List<GameObject> RemoveNullObjects(List<GameObject> target)
        {
            List<GameObject> finalList = new List<GameObject>();
            foreach (GameObject go in target)
            {
                if (go != null)
                {
                    finalList.Add(go);
                }
            }
            return finalList;
        }
        public static GameObject[] RemoveObjectFromArray(GameObject[] target, GameObject remove)
        {
            if (remove == null)
                return target;
            List<GameObject> finalList = new List<GameObject>();
            foreach (GameObject go in target)
            {
                if (go != null)
                {
                    if (go != remove)
                    {
                        finalList.Add(go);
                    }
                }
            }
            GameObject[] listArray = new GameObject[finalList.Count];
            for (int i = 0; i < finalList.Count; i++)
            {
                listArray[i] = finalList[i];
            }
            return listArray;
        }
        public static GameObject[] AddObjectToArray(GameObject[] target,GameObject add, bool canRepeat = false)
        {
            if (add == null)
                return target;

            if (!canRepeat)
            {
                bool repeat = false;
                foreach (GameObject go in target)
                {
                    if (go != null)
                    {
                        if (go == add)
                        {
                            repeat = true;
                            break;
                        }
                    }
                }

                if (repeat)
                    return target;
            }

            GameObject[] listArray = new GameObject[target.Length+1];
            for (int i = 0; i < target.Length; i++)
            {
                listArray[i] = target[i];
            }
            listArray[target.Length] = add;
            return listArray;
        }
        public static GameObject InstantiateFromRelativeFolder(string relativePath,string name, Transform targetParent = null)
        {
            Debug.Log(relativePath);
            GameObject result = GameObject.Instantiate((GameObject) AssetDatabase.LoadAssetAtPath(relativePath, typeof(GameObject)));
            result.name = name;
            result.transform.parent = targetParent;
            return result;
        }
    }
}
