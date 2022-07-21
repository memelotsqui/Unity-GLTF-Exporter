using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    [ExecuteAlways]
    public class TurnChildGameObjectVisibility : MonoBehaviour
    {
        public List<GameObject> tarObjects;
        public int curObject = 0;

        public void GetFirstChildObjects()
        {
            tarObjects = GameObjectUtilities.GetTopHierarchyChilds(gameObject,null,true);
        }
        public void TurnNextObject(bool nextObj)
        {
            if (nextObj)
            {
                curObject++;
                if (curObject >= tarObjects.Count)
                    curObject = 0;
            }
            else {
                if (curObject <= 0)
                    curObject = tarObjects.Count - 1;
                else
                    curObject--;
            }

            foreach(GameObject go in tarObjects)
            {
                go.SetActive(false);
            }

            tarObjects[curObject].SetActive(true);
        }
    }
}
