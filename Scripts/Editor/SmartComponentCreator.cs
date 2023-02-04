using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
// makes sure that the static constructor is always called in the editor.
namespace WEBGL_EXPORTER.GLTF
{
    [InitializeOnLoad]
    public class SmartComponentCreator : Editor
    {
        static SmartComponentCreator()
        {
            AddDragProjectHandler();
        }

        static DragAndDropVisualMode InspectorHandler(Object[] targets, bool perform)
        {
            if (perform)
                AddComponentToObject(targets, DragAndDrop.objectReferences, DragAndDrop.paths);

            if (isValidAsset(DragAndDrop.paths, targets))
                return DragAndDropVisualMode.Link;
            else
                return DragAndDropVisualMode.None;
        }

        static DragAndDropVisualMode HierarchyHandler(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            //Debug.Log(dropTargetInstanceID);
            if (perform)
                AddComponentToObject(EditorUtility.InstanceIDToObject(dropTargetInstanceID) as GameObject, DragAndDrop.objectReferences, DragAndDrop.paths);

            if (isValidAsset(DragAndDrop.paths) && dropTargetInstanceID > 0 && dropMode == HierarchyDropFlags.DropUpon)
                return DragAndDropVisualMode.Link;
            else
                return DragAndDropVisualMode.None;
        }

        static bool isValidAsset(string[] paths, Object[] targets = null)
        {
            if (targets != null)
            {
                foreach (Object obj in targets)
                {
                    if (obj.GetType() != typeof(GameObject))
                        return false;
                }
            }

            foreach (string st in paths)
            {
                Debug.Log(st);
                    if (!st.EndsWith(".js"))
                        return false;
            }
            return true;
        }

        public static void AddDragProjectHandler()
        {
            // Add the handler
            DragAndDrop.AddDropHandler(InspectorHandler);
            DragAndDrop.AddDropHandler(HierarchyHandler);
        }

        public static void RemoveProjectHandler()
        {
           
            // Remove the handler
            DragAndDrop.RemoveDropHandler(InspectorHandler);
            DragAndDrop.RemoveDropHandler(HierarchyHandler);
        }
        static void AddComponentToObject(GameObject target, Object[] objectReferences, string[] objectPaths)
        {
            if (!isValidAsset(objectPaths))
                return;

            Undo.RegisterCompleteObjectUndo(target, "Add smart object component to gameObject");
            foreach (Object ob in objectReferences)
            {
                // we attach component X, associated with asset X.
                var smartBehaviour = target.AddComponent<SmartObjectBehaviour>();
                // we place asset X within component X.
                smartBehaviour.javascript = ob as TextAsset;
            }
        }
        static void AddComponentToObject(Object[] targets, Object [] objectReferences, string [] objectPaths)
        {
            if (!isValidAsset(objectPaths, targets))
                return;
            
            foreach (GameObject go in targets)
            {
                Undo.RegisterCompleteObjectUndo(go, "Add smart object component to gameObject");
                foreach (Object ob in objectReferences)
                {
                    
                    // we attach component X, associated with asset X.
                    var smartBehaviour = go.AddComponent<SmartObjectBehaviour>();
                   // Undo.RegisterCreatedObjectUndo(smartBehaviour, "Add smart object component to gameObjects");
                   
                    // we place asset X within component X.
                    smartBehaviour.javascript = ob as TextAsset;
                    //Undo.CollapseUndoOperations(undoID);
                }
            }
        }
    }
}
