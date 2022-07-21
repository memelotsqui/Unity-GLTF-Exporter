using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class CreateUXMLFile
    {
        public static void CreateUSSFromStyles(int buttonHeight, int buttonWidth, int buttonMargin, int labelSize, string buttonUpLocalLocation, string buttonHoverLocalLocation, string buttonActiveLocalLocation, string fullSaveLocation)
        {
            string result = "";
            result += ".row {\n"+
                            "display: flex;\n"+
                            "flex-direction: row;\n"+
                            "justify-content: center;\n"+
                        "}\n"+
                        ".header{\n"+
                            "font-size: 15px;\n"+
                            "color: #ffd800;\n"+
                        "}\n"+
                        ".prefab-button{\n"+
                            "height: " + buttonHeight + "px;\n"+
                            "width: " + buttonWidth + "px;\n"+
                            "margin: " + buttonMargin + "px;\n"+
                            "justify-content:flex-end;\n"+
                        "}\n"+
                        ".prefab-button__icon{\n"+
                            "pointer-events:none;\n"+
                        "}\n"+
                        ".unity-button {\n"+
                            "background-image: url(\"/"+ buttonUpLocalLocation + "\");\n"+
                        "}\n"+
                        ".unity-button:hover {\n"+
                            "background-image: url(\"/"+ buttonHoverLocalLocation + "\");\n"+
                        "}\n"+
                        ".unity-button:active {\n"+
                            "background-image: url(\"/"+ buttonActiveLocalLocation + "\");\n"+
                        "}\n"+
                        "Label{\n"+
                            "font-size:"+labelSize+"px;\n"+
                        "}\n";
            FileExporter.ExportToText(result, "button_styles_uss", fullSaveLocation, "uss");
            AssetDatabase.Refresh();
        }

        public static void CreateUXMLCodesFromSubfolders(int rowSize, int maxRowsQty, string targetLocalFolder,string extension,bool getAllSubfolders=false)
        {
            string [] subfolders = StringUtilities.GetSubfoldersPathsFromFolder(targetLocalFolder,true,true,getAllSubfolders);
            string[] result = new string[subfolders.Length];
            for (int i = 0; i < subfolders.Length; i++)
            {
                Debug.Log(subfolders[i]);
                result[i] = CreateUXMLCode(rowSize, maxRowsQty, subfolders[i], extension);
                if (result[i] != "")
                {
                    Debug.Log("exporting");
                    FileExporter.ExportToText(result[i], "buttons_uxml", subfolders[i], "uxml");
                }
            }
            AssetDatabase.Refresh();
            
            //return result;
        }
        public static string CreateUXMLCode(int rowSize, int maxRowQty, string targetLocalFolder/*,bool firstFileExtension = true*/, string extension)
        {
/*            if (firstFileExtension) { 
                string [] filestemp = StringUtilities.GetFilesPathFromFolder(targetLocalFolder, true);
                if (filestemp.Length > 0)
                    extension = StringUtilities.GetExtensionFromFile(filestemp[0]);
            }*/
            string [] files = StringUtilities.GetFilesPathFromFolder(targetLocalFolder, true, extension,true);
            

            string result = "";
            if (files != null)
            {
                if (files.Length > 0)
                {
                    result += "<UXML xmlns=\"UnityEngine.UIElements\">\n";
                    int rowCounter = 0;
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (rowCounter == 0)
                        {
                            result += "<VisualElement class =\"row\">\n";
                        }
                        result += CreatePrefabButton(StringUtilities.GetFileNameFromPath(files[i]), files[i]);
                        rowCounter += 1;
                        if (rowCounter == rowSize)
                        {
                            rowCounter = 0;
                            result += "</VisualElement>\n";
                        }
                    }
                    if (rowCounter < rowSize)
                        result += "</VisualElement>\n";
                    result += "</UXML>\n";
                }
            }
            return result;
        }
        public static string CreatePrefabButton(string labelName, string location, string className = "prefab-button")
        {
            string result = "";

            result += "<Button name = \"" + location + "\" class=\"" + className + "\">\n" +
            "<VisualElement name = \"Icon\" class=\"" + className + "__icon\" />\n" +
            "<Label text = \"" + labelName + "\" class=\"" + className + "__label\"/>\n" +
            "</Button>\n";

            return result;
        } 
    }
}
