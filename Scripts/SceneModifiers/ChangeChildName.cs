using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class ChangeChildName : MonoBehaviour
    {
        public string preWords = "";
        public string postWords = "";

        public int removeInitChars = 0;
        public int removeLastChars = 0;
        // Start is called before the first frame update
        public void ChangeFirstChildNames()
        {
            List < GameObject > childObjects = new List<GameObject>();
            childObjects = GameObjectUtilities.GetTopHierarchyChilds(gameObject,childObjects,true);
            foreach (GameObject go in childObjects)
            {
                ChangeObjecName(go);
            }
        }
        public void ChangeAllChildNames()
        {
            Transform[] gos = gameObject.GetComponentsInChildren<Transform>();
            foreach(Transform go in gos)
            {
                ChangeObjecName(go.gameObject);
            }
        }
        void ChangeObjecName(GameObject tarObject)
        {
            string centerName = tarObject.name;
            centerName = StringUtilities.RemoveCharacterFromString(centerName, removeInitChars, true);
            centerName = StringUtilities.RemoveCharacterFromString(centerName, removeLastChars, false);
            tarObject.name = preWords + centerName + postWords;
        }
    }
}
