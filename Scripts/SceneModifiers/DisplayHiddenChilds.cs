using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
# endif



public class DisplayHiddenChilds : MonoBehaviour
{
    public GameObject[] hiddenChildsObjects;
    public bool toggleOn = false;
    public void SelectHiddenChildObjects()
    {

    }
    public void ToggleHiddenObjects()
    {
        foreach(GameObject t in hiddenChildsObjects)
        {
            t.SetActive(toggleOn);
            
        }
        toggleOn = !toggleOn;
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(DisplayHiddenChilds))]
public class DisplayHiddenChildsEditor : Editor
{
    DisplayHiddenChilds myScript;
    GameObject[] goArray;
    private void OnEnable()
    {
        
        myScript = (DisplayHiddenChilds)target;
       
        Transform[] allChilds = myScript.transform.GetComponentsInChildren<Transform>(true);
        List<GameObject> hiddenObjects = new List<GameObject>();
        

        foreach (Transform t in allChilds)
        {
            if (t.gameObject.activeInHierarchy == false) {
                hiddenObjects.Add(t.gameObject);
            }
        }

        myScript.hiddenChildsObjects = new GameObject[hiddenObjects.Count];

        for (int i =0;i< hiddenObjects.Count; i++)
        {
            myScript.hiddenChildsObjects[i] = hiddenObjects[i];
        }





    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        if (GUILayout.Button("Select Hidden"))
        {
            Selection.objects = myScript.hiddenChildsObjects;
        }
        string add = myScript.toggleOn? "on":"off";
        if (GUILayout.Button("Toggle Hidden " + add))

        {
            myScript.ToggleHiddenObjects();
        }
    }

}


# endif