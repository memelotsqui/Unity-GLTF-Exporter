using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class ModelImportOptions : AssetPostprocessor
    {
        public void OnPreprocessModel()
        {
            ModelImporter modelImporter = (ModelImporter)assetImporter;

            modelImporter.generateSecondaryUV = true;
        }
    }
}