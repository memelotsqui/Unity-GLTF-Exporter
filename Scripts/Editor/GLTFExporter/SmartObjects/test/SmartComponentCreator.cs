using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
// makes sure that the static constructor is always called in the editor.
namespace WEBGL_EXPORTER.GLTF
{
    public class MouseEvent : Editor
    {
        private void OnSceneGUI()
        {
            Debug.Log("asdkljn");
        }
    }
        [InitializeOnLoad]
    public class SmartComponentCreator : Editor
    {
        static SmartComponentCreator()
        {
            //EditorApplication.
            // Adds a callback for when the hierarchy window processes GUI events
            // for every GameObject in the heirarchy.
            //EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallbackExist;
            //EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;


            EditorApplication.update += te;

            //EditorApplication.
        }
        static void testFunction(GenericMenu menu, SerializedProperty property)
        {
            
            Debug.Log("ttttt");
        }
        void OnSceneGUI()
        {
            
            Debug.Log("tes new");
            //if (Event.current.type == EventType.MouseDown)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Event.current.mousePosition);
            //    RaycastHit hit = new RaycastHit();
            //    if (Physics.Raycast(ray, out hit, 1000.0f))
            //    {
            //        Debug.Log(Event.current.mousePosition);
            //        Vector3 newTilePosition = hit.point;
            //        Instantiate(newTile, newTilePosition, Quaternion.identity);
            //    }
            //}
        }
        static void te()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Debug.Log("kmk");
            //}
            //Debug.Log(Event.current);
            //Debug.Log("changa");
        }
        static void ProjectWindowItemCallback(string guid, Rect selectionRect)
        {
            //Debug.Log("ON PROJECT!");
            if (Event.current.type == EventType.MouseDrag)
            {
                //Debug.Log("yahoo");

                //TextAsset dragData = new CustomDragData();
                //dragData.originalIndex = somethingYouGotFromYourProperty;
                //dragData.originalList = this.targetList;
                TextAsset receivedDragData = new TextAsset();
                DragAndDrop.SetGenericData("text", receivedDragData);
                receivedDragData = DragAndDrop.GetGenericData("text") as TextAsset;
                Debug.Log(receivedDragData);
                DragAndDrop.AcceptDrag();
                //var selectedObjects = new List<TextAsset>();
                foreach (var objectRef in DragAndDrop.objectReferences)
                {
                    Debug.Log(objectRef);
                    // if the object is the particular asset type...
                    if (objectRef is TextAsset)
                    {
                        Debug.Log("test");
                        // we create a new GameObject using the asset's name.
                        var gameObject = new GameObject(objectRef.name);
                        // we attach component X, associated with asset X.
                        var componentX = gameObject.AddComponent<SmartObjectBehaviour>();
                        // we place asset X within component X.
                        componentX.javascript = objectRef as TextAsset;
                        // add to the list of selected objects.
                        //selectedObjects.Add(gameObject);
                    }
                }
            }
            if (Event.current.type == EventType.DragPerform)
            {
                Debug.Log("WOOO!");
            }
        }
        static void HierarchyWindowItemCallbackExist(int instanceID, Rect pRect)
        {

            // happens when an acceptable item is released over the GUI window
            if (Event.current.type == EventType.DragExited && pRect.Contains(Event.current.mousePosition))
            {
                // get all the drag and drop information ready for processing.
                DragAndDrop.AcceptDrag();

                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (gameObject == null) return;

                gameObject = PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance ? PrefabUtility.GetPrefabParent(gameObject) as GameObject : gameObject;


                if (gameObject != null)
                    // run through each object that was dragged in.
                    foreach (var objectRef in DragAndDrop.objectReferences)
                    {
                        // if the object is the particular asset type...
                        if (objectRef is TextAsset)
                        {
                            string p = AssetDatabase.GetAssetPath(objectRef);
                            if (p.EndsWith(".js"))
                            {
                                // we attach component X, associated with asset X.
                                var smartBehaviour = gameObject.AddComponent<SmartObjectBehaviour>();
                                // we place asset X within component X.
                                smartBehaviour.javascript = objectRef as TextAsset;
                            }

                        }


                    }


                Event.current.Use();


            }
        }
        static void HierarchyWindowItemCallback(int pID, Rect pRect)
        {
            // Debug.Log("DOIN SOMTHIN");
            // happens when an acceptable item is released over the GUI window
            if (Event.current.type == EventType.DragPerform)
            {
               
                DragAndDrop.AcceptDrag();
                foreach (var objectRef in DragAndDrop.objectReferences)
                {
                    if (objectRef is TextAsset)
                    {
                        Debug.Log("knjkj");
                        Debug.Log(pID);
                        Debug.Log("pus");
                        //// we create a new GameObject using the asset's name.
                        //var gameObject = new GameObject(objectRef.name);
                        //// we attach component X, associated with asset X.
                        //var componentX = gameObject.AddComponent<SmartObjectBehaviour>();
                        //// we place asset X within component X.
                        //componentX.javascript = objectRef as TextAsset;
                        //// add to the list of selected objects.
                        //selectedObjects.Add(gameObject);
                    }
                }
            }
            if (Event.current.type == EventType.DragPerform)
            {
                Debug.Log("DOIN SOMTHIN2");
                // get all the drag and drop information ready for processing.
                DragAndDrop.AcceptDrag();
                // used to emulate selection of new objects.
                var selectedObjects = new List<GameObject>();
                // run through each object that was dragged in.
                foreach (var objectRef in DragAndDrop.objectReferences)
                {
                    // if the object is the particular asset type...
                    if (objectRef is TextAsset)
                    {
                        Debug.Log("pus");
                        //// we create a new GameObject using the asset's name.
                        //var gameObject = new GameObject(objectRef.name);
                        //// we attach component X, associated with asset X.
                        //var componentX = gameObject.AddComponent<SmartObjectBehaviour>();
                        //// we place asset X within component X.
                        //componentX.javascript = objectRef as TextAsset;
                        //// add to the list of selected objects.
                        //selectedObjects.Add(gameObject);
                    }
                }
                // we didn't drag any assets of type AssetX, so do nothing.
                if (selectedObjects.Count == 0) return;
                // emulate selection of newly created objects.
                Selection.objects = selectedObjects.ToArray();

                // make sure this call is the only one that processes the event.
                Event.current.Use();
            }
        }
    }
}