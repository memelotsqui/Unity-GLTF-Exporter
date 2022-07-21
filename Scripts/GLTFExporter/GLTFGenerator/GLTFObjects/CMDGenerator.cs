using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class CMDGenerator
    {
        string command = "";
        string endCommand = "";
        public CMDGenerator()
        {

        }
        public void AddLine(string code)
        {
            command += code;
        }
        public void AddEndLine(string code)
        {
            endCommand += code;
        }
        public string GetCommand(bool delete_after = true, string explorer_location = "", string local_page_location = "", string disc_route = "", string model_id = "")
        {
            string result = command + endCommand;
           
            if (explorer_location != "")
            {
                result += disc_route + "\n";
                result += "cd \"" + StringUtilities.GetFolderNameFromFile(explorer_location) + "\"\n";
                result += StringUtilities.GetFileNameFromPath(explorer_location) + " " + local_page_location + "?id=" + model_id + "\n";
            }
            if (delete_after)
                result += "(goto) 2>nul & del \"%~f0\"\n";

            return result;
        }
    }
}
