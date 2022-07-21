using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class MaterialOptions : MonoBehaviour
    {
        public Material[] materialOptions;
        public MeshRenderer[] changeMesh;
        public int curMat = 0;
        public bool displayOptions = true;

        public bool canOffset = false;
        [HideInInspector]
        public int offsetColumns = 1;
        [HideInInspector]
        public int offsetRows = 1;
        [HideInInspector]
        public int curColumn = 1;
        [HideInInspector]
        public int curRow = 1;
        public void ChangeMaterial(bool nextMaterial)
        {
            if (nextMaterial)
            {
                curMat++;
                if (curMat >= materialOptions.Length)
                    curMat = 0;
            }
            else
            {
                curMat--;
                if (curMat < 0)
                    curMat = materialOptions.Length -1;
            }

            foreach (MeshRenderer mr in changeMesh)
            {
                mr.sharedMaterial = materialOptions[curMat];
            }
        }
        public void SelectOffset(int offsetInt)
        {
            //te
            switch (offsetInt)
            {
                case 0:                 //TOP
                    changeOffset(false, 1);
                    curRow--;
                    break;
                case 1:                 //LEFT
                    changeOffset(true, 3);
                    curColumn--;
                    break;
                case 2:                 //RIGHT
                    changeOffset(true, 5);
                    curColumn++;
                    break;
                case 3:                 //BOTTOM
                    changeOffset(false, 7);
                    curRow++;
                    break;
            }
            if (curRow > offsetRows)
                curRow = 1;
            if (curRow < 1)
                curRow = offsetRows;

            if (curColumn > offsetColumns)
                curColumn = 1;
            if (curColumn < 1)
                curColumn = offsetColumns;
        }
        private void changeOffset(bool changeColumn, int move)
        {
            
            foreach (MeshRenderer mr in changeMesh)
            {
                OffsetUVs ou = mr.transform.GetComponent<OffsetUVs>();
                if (ou == null)
                    ou = mr.gameObject.AddComponent<OffsetUVs>();

                if (changeColumn)
                    ou.offsetValue = (1f / (float)offsetColumns);
                else
                    ou.offsetValue = (1f / (float)offsetRows);

                ou.OffsetAllUVs(move);
            }
        }
    }
}
