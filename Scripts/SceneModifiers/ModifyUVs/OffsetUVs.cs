using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System;

namespace WEBGL_EXPORTER
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent (typeof(MeshFilter))]
    [RequireComponent(typeof(OffsetUVsOrigMesh))]
    public class OffsetUVs : MonoBehaviour
    {
        private string instancedFolderName = "mesh_instances";
        public float offsetValue = 0.125f;
        [HideInInspector]
        public MeshFilter meshFilter;
        [HideInInspector]
        public bool modifiedMesh = false;
        private string instanceDirectory;
        [HideInInspector]
        public Mesh editedMesh;
        [HideInInspector]
        public bool uvsFromLightmapUvs = false;

        //public OffsetUVsOrigMesh origMesh;



        //[HideInInspector]
        //public Mesh originalMesh;
        [HideInInspector]
        public Mesh baseMesh;
        //[HideInInspector]
        //public OffsetUVsOrigMesh om;

        public Vector2 offsetUVs;
        [HideInInspector]
        public Vector2 savedOffsetUVs;

        public Vector2 scaleUVS = Vector2.one;
        [HideInInspector]
        public Vector2 savedScaleUVs = Vector2.one;

        [HideInInspector]
        public int objectID;

        [HideInInspector]
        public bool AutoChange = true;

/*        public int targetSubmesh = 0;
        public Vector2[][] uvsArray;
        public int[][] triangleIndices;*/

        
        private void OnEnable()
        {
            //origMesh = GetComponent<OffsetUVsOrigMesh>();
            if (objectID != gameObject.GetInstanceID())
            {
                objectID = gameObject.GetInstanceID();
                GetOriginalMesh();
                editedMesh = Instantiate(GetComponent<OffsetUVsOrigMesh>().originalMesh);
            }
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                OffsetUVsOrigMesh origMesh = GetComponent<OffsetUVsOrigMesh>();
                if (origMesh != null)
                    DestroyImmediate(origMesh);
                DestroyImmediate(this);
                //DestroyImmediate
            }
            if (meshFilter.sharedMesh == null)
                OffsetTargetUVs(Vector2.zero);

/*            if (meshFilter.sharedMesh.subMeshCount > 1)
            {
                GetSubmeshArray();
            }*/
        }
        //private void GetSubmeshArray()
        //{
/*            //if (uvsArray == null)
            //{
                
                int submeshCount = meshFilter.sharedMesh.subMeshCount;
                uvsArray = new Vector2[submeshCount][];
                triangleIndices = new int[submeshCount][];

                for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
                {
                //triangleIndices[i] = meshFilter.sharedMesh.GetTriangles(i).Length / 3;
                //Debug.Log(triangleIndices[i]);
                    triangleIndices[i] = meshFilter.sharedMesh.GetTriangles(i);
                    foreach (int fi in triangleIndices[i])
                    {
                        Debug.Log("index: " + fi);
                    }
                }
            // step 1: Get the all the triangle indices
            // Step 2: Use the triangle indices to fetch a vertex list
            // Step 3: Use this list
            Debug.Log(meshFilter.sharedMesh.uv.Length);
            Debug.Log(meshFilter.sharedMesh.vertices.Length);
            int maxVal = 0;
                for (int i = 0; i < submeshCount; i++)
                {
                    int startVal = maxVal;              // STARTS FROM 0, AND WITH EACH CYCLE IT GROWS
                    //maxVal += triangleIndices[i];       // SETS THE LIMIT

                    //Debug.Log("from: " + startVal + "  to: " + maxVal);
                    //Debug.Log(meshFilter.sharedMesh.uv.Length);
                    //Debug.Log(meshFilter.sharedMesh.vertices.Length);
                    Vector2[] resultVector2 = new Vector2[maxVal - startVal];
                    for (int j = startVal; j < maxVal; j++)
                    {
                        resultVector2[j-startVal] = meshFilter.sharedMesh.uv[j];
                    }
                    uvsArray[i] = resultVector2;
                }
            //}

            if (uvsArray != null)
            {
                foreach (Vector2[] v2 in uvsArray)
                {
                    Debug.Log(v2.Length);
                }
            }

            //if ()*/
        //}
        public void SetOffsetUvs()
        {
            if (objectID != gameObject.GetInstanceID())
            {
                objectID = gameObject.GetInstanceID();
                GetOriginalMesh();
                OffsetTargetUVs(Vector2.zero);
            }
            if (scaleUVS != savedScaleUVs || offsetUVs != savedOffsetUVs)
                OffsetTargetUVs(Vector2.zero);
        }
        public static void SetAllToOriginalMesh(bool origMesh)
        {
            OffsetUVs[] allOffsetUVs = FindObjectsOfType<OffsetUVs>();
            foreach (OffsetUVs ou in allOffsetUVs)
            {
                ou.SetOriginalMesh(origMesh);
            }
        }
        public void SetLightmapUvsAsMainUvs(bool set = true)
        {
            if (set)
            {
                uvsFromLightmapUvs = true;
                OffsetTargetUVs(Vector2.zero);       // WE OFFSET THE VALUES TO MAKE SURE WE ALREADY HAVE AN INSTANCE PF THE UVS
                List<Vector2> lightmapUvs = new List<Vector2>();
                editedMesh.GetUVs(1, lightmapUvs);
                editedMesh.SetUVs(0, lightmapUvs);
                OffsetTargetUVs(Vector2.zero);       // AND UPDATE A NEW VALUE
            }
            else
            {
                uvsFromLightmapUvs = false;
                
                List<Vector2> origUvs = new List<Vector2>();
                GetComponent<OffsetUVsOrigMesh>().originalMesh.GetUVs(0, origUvs);
                editedMesh.SetUVs(0, origUvs);
                OffsetTargetUVs(Vector2.zero);       // UPDATE NEW VALUE
            }
        }
        private void OnValidate()
        {
            if (AutoChange)
            {
                //Undo.RecordObject(editedMesh, "offset uvs");
                SetOffsetUvs();
            }
        }
        public void ResetOriginalMesh()
        {
            meshFilter.sharedMesh = GetComponent<OffsetUVsOrigMesh>().originalMesh;
            editedMesh = Instantiate(GetComponent<OffsetUVsOrigMesh>().originalMesh);
            OffsetTargetUVs(Vector2.zero);

        }
        public void GetOriginalMesh()
        {
            meshFilter = GetComponent<MeshFilter>();
            OffsetUVsOrigMesh om = GetComponent<OffsetUVsOrigMesh>();

            if (om.originalMesh != null)
            {
                if (meshFilter.sharedMesh == null)
                {
                    meshFilter.sharedMesh = om.originalMesh;
                }

                editedMesh = Instantiate(om.originalMesh);

                if (scaleUVS != savedScaleUVs || offsetUVs != savedOffsetUVs)
                {
                    OffsetTargetUVs(Vector2.zero);
                }
            }
            else
            {
                Debug.LogWarning(om.transform.name + " object, does not have mesh filter, remove object or delete component");
            }
        }
        public void SetOriginalMesh(bool setOrig = true)
        {
            AutoChange = !setOrig;
            if (setOrig)
            {
                GetComponent<MeshFilter>().sharedMesh = GetComponent<OffsetUVsOrigMesh>().originalMesh;
            }
            else
            {
                OffsetTargetUVs(Vector2.zero);
            }
        }
        public void DebugClick()
        {
            Debug.Log(GetComponent<OffsetUVsOrigMesh>().originalMesh.uv2.Length);
            Debug.Log(GetComponent<MeshFilter>().sharedMesh.uv2.Length);

            Debug.Log(GetComponent<OffsetUVsOrigMesh>().originalMesh.uv.Length);
            Debug.Log(GetComponent<MeshFilter>().sharedMesh.uv.Length);
        }
        public void OffsetAllUVs(int move)
        {
            modifiedMesh = true;
            switch (move)
            {
                case 0:
                    OffsetTargetUVs(new Vector2(-offsetValue, offsetValue));
                    break;
                case 1:
                    OffsetTargetUVs(new Vector2(0f, offsetValue));
                    break;
                case 2:
                    OffsetTargetUVs(new Vector2(offsetValue, offsetValue));
                    break;

                case 3:
                    OffsetTargetUVs(new Vector2(-offsetValue, 0f));
                    break;
                case 4:
                    //OffsetTargetUVs(new Vector2(0f, 0f));
                    break;
                case 5:
                    OffsetTargetUVs(new Vector2(offsetValue, 0f));
                    break;

                case 6:
                    OffsetTargetUVs(new Vector2(-offsetValue, -offsetValue));
                    break;
                case 7:
                    OffsetTargetUVs(new Vector2(0f, -offsetValue));
                    break;
                case 8:
                    OffsetTargetUVs(new Vector2(offsetValue, -offsetValue));
                    break;


                default:
                    Debug.Log("non valid int");
                    break;
            }


        }

        public void OffsetTargetUVs(Vector2 addValues)
        {
            savedScaleUVs = scaleUVS;
            savedOffsetUVs = offsetUVs;
            OffsetUVsOrigMesh om = GetComponent<OffsetUVsOrigMesh>();

            if (om.originalMesh != null)    // added
            {
                if (this.enabled)
                {
                    offsetUVs = new Vector2(offsetUVs.x + addValues.x, offsetUVs.y + addValues.y);

                    List<Vector2> newUvs = new List<Vector2>();
                    if (!uvsFromLightmapUvs)
                    {
                        for (int i = 0; i < om.originalMesh.uv.Length; i++)
                        {
                            newUvs.Add(new Vector2((om.originalMesh.uv[i].x * scaleUVS.x) + offsetUVs.x, (om.originalMesh.uv[i].y * scaleUVS.y) + offsetUVs.y));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < om.originalMesh.uv2.Length; i++)
                        {
                            newUvs.Add(new Vector2((om.originalMesh.uv2[i].x * scaleUVS.x) + offsetUVs.x, (om.originalMesh.uv2[i].y * scaleUVS.y) + offsetUVs.y));
                        }
                    }

                    editedMesh.SetUVs(0, newUvs);
                    meshFilter.mesh = editedMesh;
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }

        }


        public string GetFolderLocation()
        {
            string[] tempString = SceneManager.GetActiveScene().path.Split('/');
            string sceneDirectory = "";
            for (int i = 0; i < tempString.Length - 1; i++)             // we want to remove the name of the scene, we only need the folder path;
                sceneDirectory += tempString[i] + "/";
            instanceDirectory = sceneDirectory + instancedFolderName;

            if (!AssetDatabase.IsValidFolder(instanceDirectory))        // if the folder doesnt exist, create it.
            {
                AssetDatabase.CreateFolder(StringUtilities.RemoveCharacterFromString(sceneDirectory, 1, false), instancedFolderName);
            }

            return (StringUtilities.RemoveCharacterFromString(Application.dataPath, 6, false) + instanceDirectory + "/");
        }
  /*      public void CreateInstance(bool removeOld = true)
        {


            meshFilter = GetComponent<MeshFilter>();


            GameObject dupObject = GameObject.Instantiate(meshFilter.gameObject);

            List<GameObject> firstChildObjects = GameObjectUtilities.GetTopHierarchyChilds(dupObject, null, true);
            foreach (GameObject go in firstChildObjects)    //THIS WAY WE MAKE SURE WE ONLY EXPORT THIS GAME OBJECT, AND NOT AL THE CHILDS
                DestroyImmediate(go);

            string objectName = "(inst)_" + UnityEngine.Random.Range(0, 9999999).ToString() + System.DateTime.Now.Hour.ToString("D2") + System.DateTime.Now.Minute.ToString("D2") + System.DateTime.Now.Second.ToString("D2");
            FileModelExporter.ExportToFBX(dupObject, objectName, GetFolderLocation());
            DestroyImmediate(dupObject);

            GameObject newObject = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath(instanceDirectory + "/" + objectName + ".fbx", typeof(GameObject)) as GameObject);



            newObject.name = objectName;
            newObject.transform.position = meshFilter.transform.position;
            newObject.transform.rotation = meshFilter.transform.rotation;
            newObject.transform.parent = meshFilter.transform.parent;
            newObject.transform.localScale = meshFilter.transform.localScale;
            newObject.GetComponent<MeshRenderer>().materials = meshFilter.GetComponent<MeshRenderer>().sharedMaterials;
            newObject.AddComponent<OffsetUVs>();

            if (removeOld)              // THIS SECTION WILL GET THE CHILD OBJECTS OF THE ORIGINAL MESH, AND ASSIGN THEM TO THE NEW INSTANCE AS CHILDS
            {
                dupObject = GameObject.Instantiate(meshFilter.gameObject);
                dupObject.transform.position = meshFilter.gameObject.transform.position;
                dupObject.transform.rotation = meshFilter.gameObject.transform.rotation;
                dupObject.transform.parent = meshFilter.gameObject.transform.parent;
                dupObject.transform.localScale = meshFilter.gameObject.transform.localScale;

                firstChildObjects = GameObjectUtilities.GetTopHierarchyChilds(dupObject, null, true);
                foreach (GameObject go in firstChildObjects)
                {
                    go.transform.parent = newObject.transform;
                }
                DestroyImmediate(dupObject);
            }
            newObject.transform.SetSiblingIndex(meshFilter.transform.GetSiblingIndex());
            if (removeOld)
                DestroyImmediate(meshFilter.gameObject);
            Selection.activeGameObject = newObject.gameObject;

        }*/
/*        public void SaveChanges()
        {
            string[] splitString = transform.name.Split(' ');
            GameObject dupObject = GameObject.Instantiate(meshFilter.gameObject);

            List<GameObject> firstChildObjects = GameObjectUtilities.GetTopHierarchyChilds(dupObject, null, true);
            foreach (GameObject go in firstChildObjects)    //THIS WAY WE MAKE SURE WE ONLY EXPORT THIS GAME OBJECT, AND NOT AL THE CHILDS
                DestroyImmediate(go);

            FileExporter.ExportToFBX(dupObject, splitString[0], GetFolderLocation());
            DestroyImmediate(dupObject);
            //Debug.Log(splitString[0]);
        }*/
    }
}
