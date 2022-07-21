using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class MeshHolder
    {
        // consts
        public bool supportsTransformExtension = false;

        // statics
        public static List <MeshHolder> allMeshHolders;

        // vars
        public Mesh baseMesh;               
        public List<UVVariant> uv1Variant;  // THE VARIANTS FOR THIS MESH, STORED IN A CUSTOM CLASS
        public List<UVVariant> uv2Variant;

        public bool hasUVs1;                // DOES IT HAS PRIMARY UVS?
        public bool hasUVs2;                // DOES IT HAS LIGHTMAP UVS?

        public Vector2[] uvs;               // PRIMARY UVS INFORMATION IN CASE THEY EXIST
        public Vector2[] uvs2;              // LIGHTMAP UVS INFORMATION IN CASE THEY EXIST

        public int subMeshCount;            //HOW MANY SUBMESHES THIS MESH HAS?

        // FOR CUSTOM EXTRAS (COMBINED MESHES)
        public int[] subMeshStart;
        public int[] subMeshSize;


        // BASE INFORMATION OF THE MESH, THIS INFORMATION FOR NOW, IS CONSTANT
        public List<ObjectAccessors> accessorIndices;
        public ObjectAccessors accessorNormals;
        public ObjectAccessors accessorVertices;

        // FOR SKINNED MESH ONLY
        public ObjectAccessors accessorWeights;
        public ObjectAccessors accessorJoints;

        // SAVE ONLY IF EXTENSION IS SUPPORTED
        public ObjectAccessors accessorUVs1;
        public ObjectAccessors accessorUVs2;


        public static void Reset() 
        {
            allMeshHolders = new List<MeshHolder>();
        }
        public static MeshHolder GetMeshHolder(Mesh mesh, bool is_skinned = false)
        {
            foreach (MeshHolder mh in allMeshHolders)
            {
                if (mh.baseMesh == mesh)
                    return mh;
            }
            // IF NO MESH HOLDER WAS FOUND, ADD A NEW ONE TO THE LIST
            MeshHolder _newMeshHolder = new MeshHolder(mesh, is_skinned);
            allMeshHolders.Add(_newMeshHolder);
            return _newMeshHolder;
        }

        public MeshHolder(Mesh mesh, bool is_skinned)
        {
            subMeshCount = mesh.subMeshCount;
            baseMesh = mesh;
            // IMPORTANT, THESE LISTS ARE STORED IN OBJECT MESH WITH GETUVSLOCALINDEX() ~ MAIN UVS OR ADDUVSAUTO()~LIGHTMAP UVS
            uv1Variant = new List<UVVariant>();
            uv2Variant = new List<UVVariant>();

            accessorIndices = new List<ObjectAccessors>();
            //SUBMESH GLTF SEPARATED (CLASSIC MODE)
            if (!ExportToGLTF.options.exportSubmeshesInExtra)
            {
                for (int i = 0; i < baseMesh.subMeshCount; i++)
                {
                    int[] _indices = baseMesh.GetIndices(i);
                    int[] _inv_indices = new int[_indices.Length];
                    for (int j = 0; j < _inv_indices.Length; j++)
                    {
                        _inv_indices[j] = _indices[_inv_indices.Length - j - 1];
                    }
                    accessorIndices.Add(new ObjectAccessors(_inv_indices, true, true, "indices"));
                }
            }
            //END SUBMESH GLTF SEPARATED (CLASSIC MODE)
            //SUBMESH COMBINED GLTF (CUSTOM EXTRA MODE)
            else
            {
                if (baseMesh.subMeshCount > 1)
                {
                    subMeshStart = new int[baseMesh.subMeshCount];
                    subMeshSize = new int[baseMesh.subMeshCount];
                }
                
                List<int> _indices = new List<int>();
                int counter = 0;
                for (int i = 0; i < baseMesh.subMeshCount; i++)
                {
                    if (baseMesh.subMeshCount > 1)      //SINCE WE ARE INVERTING INDICES, THIS ONE HAS TO BE ALSO INVERTED
                    {
                        subMeshStart[i] = counter;
                        int submesh_size = baseMesh.GetIndices(baseMesh.subMeshCount - 1 - i).Length;
                        subMeshSize[i] = submesh_size;
                        counter += submesh_size;
                    }
                    _indices.AddRange(baseMesh.GetIndices(i)); 
                    
                }
                int[] _inv_indices = new int[_indices.Count];
                for (int i = 0; i < _inv_indices.Length; i++)
                {
                    _inv_indices[i] = _indices[_inv_indices.Length - i - 1];
                }
                accessorIndices.Add(new ObjectAccessors(_inv_indices, true, true, "indices"));
            }
            //END SUBMESH COMBINED GLTF (CUSTOM EXTRA MODE)
            if (is_skinned)
            {
                accessorJoints = new ObjectAccessors(GetBoneIndexArray(baseMesh.boneWeights),"joints");
                accessorWeights = new ObjectAccessors(GetBoneWeightArray(baseMesh.boneWeights), new Vector4(1f, 1f, 1f, 1f), false, "weights",ComponentType.FLOAT,false);
            }
            if (!ExportToGLTF.options.quantizeGLTF)
            {
                accessorVertices = new ObjectAccessors(baseMesh.vertices, new Vector3(1,1,-1), true, "vertices");

                if (baseMesh.normals.Length > 0 && ExportToGLTF.options.exportNormals)
                    accessorNormals = new ObjectAccessors(baseMesh.normals, new Vector3(1, 1, -1), false, "normals");
            }
            else
            {
                accessorVertices = new ObjectAccessors(baseMesh.vertices, new Vector3(1, 1, -1), true, "vertices", ExportGLTFOptions.GetComponentType(ExportToGLTF.options.quantizeVerticesTo), true);

                if (baseMesh.normals.Length > 0 && ExportToGLTF.options.exportNormals)
                    accessorNormals = new ObjectAccessors(baseMesh.normals, new Vector3(1, 1, -1), false, "normals", ExportGLTFOptions.GetComponentType(ExportToGLTF.options.quantizeNormalsTo,true), false);
            }

            if (mesh.uv.Length > 0)
            {
                hasUVs1 = true;
                uvs = mesh.uv;
                if (ExportToGLTF.options.createUVOffsetExtras)
                    accessorUVs1 = CreateUVsBasicAccessor(uvs,"uvs_1");
                    //accessorUVs1 = new ObjectAccessors(uvs,false,false,"uvs_1");
            }
            else
            {
                Debug.Log("has no uvs");
                hasUVs1 = false;
            }

            if (mesh.uv2.Length > 0)
            {
                hasUVs2 = true;
                uvs2 = mesh.uv2;
                if (ExportToGLTF.options.createUVOffsetExtras)
                    accessorUVs2 = CreateUVsBasicAccessor(uvs2, "uvs_2");
                //accessorUVs2 = new ObjectAccessors(uvs2,false,false,"uvs_2");
            }
            else
            {
                hasUVs2 = false;
            }
        }

        private Vector4[] GetBoneIndexArray(BoneWeight[] bones_weights)
        {
            Vector4[] result = new Vector4[bones_weights.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector4(bones_weights[i].boneIndex0,
                                        bones_weights[i].boneIndex1,
                                        bones_weights[i].boneIndex2,
                                        bones_weights[i].boneIndex3);
            }
            return result;
        }
        private Vector4[] GetBoneWeightArray(BoneWeight[] bones_weights)
        {
            Vector4[] result = new Vector4[bones_weights.Length];

            for (int i =0; i < result.Length; i++)
            {
                //Debug.Log("new");
                result[i] = new Vector4(bones_weights[i].weight0,
                                        bones_weights[i].weight1,
                                        bones_weights[i].weight2,
                                        bones_weights[i].weight3);
            }
            return result;
        }

        public ObjectAccessors CreateUVsBasicAccessor(Vector2[] uvs, string optional_identifier = "")    //FUNCTION WILL BE USED WHEN MESH TRANSFORM EXSTRAS IS SUPPORTED
        {

            if (!ExportToGLTF.options.quantizeGLTF)
            {
                return new ObjectAccessors(uvs, false, optional_identifier);
            }
            else
            {
                if (optional_identifier == "uvs_2")
                    return new ObjectAccessors(uvs, false, optional_identifier, ExportGLTFOptions.GetComponentType(ExportToGLTF.options.quantizeLightUVsTo), false);
                return new ObjectAccessors(uvs, false, optional_identifier, ExportGLTFOptions.GetComponentType(ExportToGLTF.options.quantizeMainUVsTo) , false);
            }
        }

        public UVVariant GetUVsLocalIndex(Vector2 offsetUVs, Vector2 scale_uvs)   // FOR MAIN UVS ONLY
        {
            if (hasUVs1)    // CHECK IF WE HAVE MAIN UVS!, IF NOT RETURN NULL!!
            {
                for (int i = 0; i < uv1Variant.Count; i++)
                {
                    if (uv1Variant[i].offsetUVs == offsetUVs && uv1Variant[i].scaleUVs == scale_uvs)
                    {
                        return uv1Variant[i];
                    }
                }
                // IF NO SAME UVVARIANT WAS FOUND, RETURN A NEW UVVARIANT
                UVVariant newVariant = new UVVariant(offsetUVs, scale_uvs);
                uv1Variant.Add(newVariant);
                if (!ExportToGLTF.options.createUVOffsetExtras && hasUVs1)             // IF WE CANT TRANSFORM VERTICES WITH CODE IN GLTF, WE MUST STORE NEW ACCESSORS
                    newVariant.CreateObjectAccessor(uvs, true, "uvs_1");         // IF NO UVS ARE FOUND, WE DONT SAVE THE ACCESSOR

                return newVariant;
            }
            else
            {
                return null;
            }
        }
        public UVVariant AddUVsAuto(Vector2 offsetUVs, Vector2 scale_uvs, Vector2? offset_orig = null, Vector2? scale_orig = null) // FOR LIGHTMAPS ONLY
        {

            if (!hasUVs2 && hasUVs1 && offset_orig != null && scale_orig != null) 
            {
                //Debug.Log(offset_orig.Value.x);
                //offsetUVs = new Vector2((offsetUVs.x - offset_orig.Value.x),(offsetUVs.y - offset_orig.Value.y));
                //scale_uvs = new Vector2((scale_uvs.x * scale_orig.Value.x), (scale_uvs.y * scale_orig.Value.y));
            }
            UVVariant newVariant = new UVVariant(offsetUVs, scale_uvs);
            uv2Variant.Add(newVariant);
            if (!ExportToGLTF.options.createUVOffsetExtras)                        // IF WE CANT TRANSFORM VERTICES WITH CODE IN GLTF, WE MUST STORE NEW ACCESSORS
            {
                if (hasUVs2)
                {
                    newVariant.CreateObjectAccessor(uvs2, true, "uvs_2");
                }
                else if (hasUVs1)
                {
                    Debug.Log("no uvs 2 taking uvs 1");
                    newVariant.CreateObjectAccessor(uvs, true, "uvs_2", offset_orig, scale_orig);     //IF WE DONT HAVE SECONDARY UVS, UNITY USES THE MAIN UVS AS LIGHTMAP UVS
                }
            }

            return newVariant;    
        }
        public static bool IsSameUVs(UVVariant uvvar, Vector2 offset_uvs, Vector2 scale_uvs)
        {
            if (uvvar.offsetUVs == offset_uvs && uvvar.scaleUVs == scale_uvs)
                return true;
            return false;
        }
        

    }
}
