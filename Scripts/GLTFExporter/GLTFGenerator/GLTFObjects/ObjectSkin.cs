using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectSkin
    {
        //statics 
        public static int globalIndex = -1;
        public static List<ObjectSkin> allUniqueSkins;

        //vars
        public int index = -1;

        public ObjectAccessors accessorInverseBindMatrix;
        public Transform [] bones;
        public SkinnedMeshRenderer skinMesh;
        //public int[] connectedJointIndices;
        public static void Reset()
        {
            globalIndex = 0;
            allUniqueSkins = new List<ObjectSkin>();
        }

        public static int GetSkinIndex(SkinnedMeshRenderer _smr)
        {
            //if (_bones == null)
            //Debug.Log("NULL BONES");
            if (_smr.rootBone == null)
                return -1;
            if (allUniqueSkins == null)
            {
                allUniqueSkins = new List<ObjectSkin>();
                allUniqueSkins.Add(new ObjectSkin(_smr));

                return allUniqueSkins.Count - 1;
            }
            else
            {
                foreach (ObjectSkin os in allUniqueSkins)
                {
                    if (os.isSameSkin(_smr.bones))
                    {
                        return os.index;
                    }
                }
                
                allUniqueSkins.Add(new ObjectSkin( _smr));
                return allUniqueSkins.Count - 1;
            }
        }
        public bool isSameSkin(Transform[] _bones)
        {
            if (_bones == null)
            {
                Debug.LogError("null bones");
                return false;
            }
            if (_bones.Length != bones.Length)
                return false;
            else
            {
                for (int i = 0; i < _bones.Length; i++)
                {
                    if (bones[i] != _bones[i])
                        return false;
                }
            }
            return true;
        }
        public ObjectSkin(SkinnedMeshRenderer _smr)
        {
            index = globalIndex;
            skinMesh = _smr;
            bones = _smr.bones;
            Debug.LogWarning("called here");
            Debug.Log(_smr);
            for (int i =0; i < _smr.bones.Length; i++)
            {
                Debug.LogWarning("new!!");
                Debug.Log(_smr.sharedMesh.bindposes[i]);
                Debug.Log("compare below");
                Debug.Log(_smr.bones[i].localToWorldMatrix);
                Debug.Log(_smr.bones[i].localToWorldMatrix.inverse);
            }
            
            
            accessorInverseBindMatrix = new ObjectAccessors(GetInverseBonesMatrix4x4(_smr));
            globalIndex++;
        }
        public Matrix4x4[] GetInverseBonesMatrix4x4(SkinnedMeshRenderer _smr)
        {
            Matrix4x4[] result = new Matrix4x4[_smr.bones.Length];

            for (int i =0; i < _smr.bones.Length; i++)
            {

                result[i] = TransformUtilities.GetGLTFMatrix(_smr.sharedMesh.bindposes[i]);
            }

            return result;
        }

        private int[] GetTransformIndices()
        {
            int[] result = new int[bones.Length];
            for (int i =0; i < bones.Length; i++)
            {
                result[i] = bones[i].GetComponent<ObjectNodeMono>().node;
            }
            return result;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueSkins.Count > 0)
            {
                result += "\"skins\" : [\n";
                //List<ObjectProperty> _skins = new List<ObjectProperty>();
                foreach (ObjectSkin os in allUniqueSkins) { 
                    List<ObjectProperty> _skin_single = new List<ObjectProperty>();
                    _skin_single.Add(new ObjectProperty("inverseBindMatrices", os.accessorInverseBindMatrix.export_index));
                    _skin_single.Add(new ObjectProperty("joints", os.GetTransformIndices()));
                    result += ObjectProperty.GetObjectProperties(_skin_single) + ",\n";

                }
                if (allUniqueSkins.Count > 0)
                    result = StringUtilities.RemoveCharacterFromString(result, 2, false);
                result += "]";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }
    }
}


