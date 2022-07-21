using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Inspector_SmartObject_DragDropv2 : MonoBehaviour
{
    public class CustomDragData
    {
        public int originalIndex;
        public IList originalList;
    }
    private void OnGUI()
    {
        if (Event.current.type == EventType.MouseDrag)
        {
            // Clear out drag data
            DragAndDrop.PrepareStartDrag();
            Debug.Log("test");

            // Set up what we want to drag
            DragAndDrop.paths[0] = "/Users/joe/myPath.txt";

            // Start the actual drag
            DragAndDrop.StartDrag("Dragging title");

            // Make sure no one uses the event after us
            Event.current.Use();
        }
    }
    //protected void ExampleDragDropGUI(Rect dropArea, SerializedProperty property)
    //{
    //    // Cache References:
    //    Event currentEvent = Event.current;
    //    EventType currentEventType = currentEvent.type;

    //    // The DragExited event does not have the same mouse position data as the other events,
    //    // so it must be checked now:
    //    if (currentEventType == EventType.DragExited) DragAndDrop.PrepareStartDrag();// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)

    //    if (!dropArea.Contains(currentEvent.mousePosition)) return;

    //    switch (currentEventType)
    //    {
    //        case EventType.MouseDown:
    //            DragAndDrop.PrepareStartDrag();// reset data
    //            Debug.Log("start mouse drag");
    //            CustomDragData dragData = new CustomDragData();
    //            dragData.originalIndex = somethingYouGotFromYourProperty;
    //            dragData.originalList = this.targetList;

    //            DragAndDrop.SetGenericData(dragDropIdentifier, dragData);

    //            Object[] objectReferences = new Object[1] { property.objectReferenceValue };// Careful, null values cause exceptions in existing editor code.
    //            DragAndDrop.objectReferences = objectReferences;// Note: this object won't be 'get'-able until the next GUI event.

    //            currentEvent.Use();

    //            break;
    //        case EventType.MouseDrag:
    //            // If drag was started here:
    //            CustomDragData existingDragData = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;

    //            if (existingDragData != null)
    //            {
    //                DragAndDrop.StartDrag("Dragging List ELement");
    //                currentEvent.Use();
    //            }

    //            break;
    //        case EventType.DragUpdated:
    //            if (IsDragTargetValid()) DragAndDrop.visualMode = DragAndDropVisualMode.Link;
    //            else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

    //            currentEvent.Use();
    //            break;
    //        case EventType.Repaint:
    //            if (
    //            DragAndDrop.visualMode == DragAndDropVisualMode.None ||
    //            DragAndDrop.visualMode == DragAndDropVisualMode.Rejected) break;

    //            EditorGUI.DrawRect(dropArea, Color.grey);
    //            break;
    //        case EventType.DragPerform:
    //            DragAndDrop.AcceptDrag();

    //            CustomDragData receivedDragData = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;

    //            if (receivedDragData != null  receivedDragData.originalList == this.targetList) ReorderObject();
    //    else AddDraggedObjectsToList();

    //            currentEvent.Use();
    //            break;
    //        case EventType.MouseUp:
    //            // Clean up, in case MouseDrag never occurred:
    //            DragAndDrop.PrepareStartDrag();
    //            break;
    //    }

    //}
}
