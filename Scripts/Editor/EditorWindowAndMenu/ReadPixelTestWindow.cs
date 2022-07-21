using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class ReadPixelTestWindow:EditorWindow
    {
        Texture2D tarTextureToRead;
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ReadPixelTestWindow));
        }

        private void OnGUI()
        {
            tarTextureToRead = (Texture2D)EditorGUILayout.ObjectField("Target Texture", tarTextureToRead, (typeof(Object)), false);
            if (tarTextureToRead != null)
            {
                if (GUILayout.Button("ReadTexture", GUILayout.Height(80f)))
                {
                    ReadPixelsFromTexture(tarTextureToRead);
                }
            }
        }
        static void ReadPixelsFromTexture(Texture2D tarTexture)
        {

            FileExporter.ExportToEXR(tarTexture, "testing_nocompression_4_2048", "L:/Archivos usuario/Documentos_L/Images",Texture2D.EXRFlags.CompressZIP);
            

        }
    }
}
