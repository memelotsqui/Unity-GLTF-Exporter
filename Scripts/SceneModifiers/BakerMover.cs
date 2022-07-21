using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    [ExecuteAlways]
    public class BakerMover : MonoBehaviour
    {
        public GameObject targetObject;
        public GameObject[] turnoffObjects;
        public Vector3 globalPosition;
        public Quaternion globalRotation;
        public Vector3 globalTestPosition;
        public Quaternion globalTestRotation;
        [HideInInspector]
        public bool firstTime = true;

        // NOTE: CANT PLACE INITIAL VALUE ON ENABLE, WHEN IT DUPLICATES IT RESETS THE POSITION TO ITS CURRENT VALUE

        public void SetForExport(bool set = true)
        {
            if (set)
            {
                targetObject.SetActive(true);
                MoveToSavedPosition();
            }
            else
            {
                MoveToOutsidePosition();
            }
        }
        // Update is called once per frame
        public void GetObjectPosition()
        {
            if (targetObject == null)
                targetObject = gameObject;
            globalPosition = gameObject.transform.position;
            globalRotation = gameObject.transform.rotation;
        }
        public void GetObjectTestPosition()
        {
            if (targetObject == null)
                targetObject = gameObject;
            globalTestPosition = gameObject.transform.position;
            globalTestRotation = gameObject.transform.rotation;
        }
        public void MoveToSavedPosition()
        {
            if (targetObject != null)
            {
                targetObject.transform.position = globalPosition;
                targetObject.transform.rotation = globalRotation;
                foreach (GameObject go in turnoffObjects)
                {
                    if (go != null)
                        go.SetActive(false);
                }
            }
        }
        public void MoveToOutsidePosition()
        {
            if (targetObject != null)
            {
                targetObject.transform.position = globalTestPosition;
                targetObject.transform.rotation = globalTestRotation;
                foreach (GameObject go in turnoffObjects)
                {
                    if (go != null)
                        go.SetActive(true);
                }
            }
        }
    }
}
