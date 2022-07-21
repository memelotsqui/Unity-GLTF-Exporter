using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class ArrayListsUtilities
    {
        public static List <MeshRenderer> GetWorkingMeshRenderersListFromChilds(Transform targetParent)
        {
            List<MeshRenderer> meshRendChildsResult = new List<MeshRenderer>();
            MeshRenderer[] allChildMeshrenderers = targetParent.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in allChildMeshrenderers)
            {
                MeshFilter mf = mr.gameObject.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    if (mf.sharedMesh != null)
                    {
                        meshRendChildsResult.Add(mr);
                    }
                }
            }
            return meshRendChildsResult;
        }
        public static MeshRenderer[] GetWorkingMeshRenderersArrayFromChilds(Transform targetParent)
        {
            List<MeshRenderer> meshRendChildsResult = GetWorkingMeshRenderersListFromChilds(targetParent);
            MeshRenderer[] result = new MeshRenderer[meshRendChildsResult.Count];
            for (int i = 0; i < result.Length;i++)
            {
                result[i] = meshRendChildsResult[i];
            }
            return result;
        }
        public static string[] ChangeArraySize(string[] targetArray, int newSize)
        {
            string[] newArray = new string[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }
        public static long[] ChangeArraySize(long[] targetArray, int newSize)
        {
            long[] newArray = new long[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }

        public static int[] ChangeArraySize(int[] targetArray, int newSize)
        {
            int[] newArray = new int[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }
        public static GameObject[] ChangeArraySize(GameObject[] targetArray, int newSize)
        {
            GameObject[] newArray = new GameObject[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }
        public static Material[] ChangeArraySize(Material[] targetArray, int newSize)
        {
            Material[] newArray = new Material[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }
        public static GameObjectArray[] ChangeArraySize(GameObjectArray[] targetArray, int newSize)
        {
            GameObjectArray[] newArray = new GameObjectArray[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (targetArray.Length == i)
                    break;
                else
                    newArray[i] = targetArray[i];
            }
            return newArray;
        }
    }
}
