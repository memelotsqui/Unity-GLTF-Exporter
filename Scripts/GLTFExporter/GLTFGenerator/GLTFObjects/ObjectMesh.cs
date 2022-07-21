using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectMesh
    {
        //consts
        public const string exportPreName = "msh_";
        //public const bool offsetExtension = false;      // IF WE HAVE AN EXTENSION THAT CAN OFFSET THE UVS TAKEN FROM THE ORIGINAL MAPS, THEN WE ONLY NEED TO SAVE UVS ONCE, OTHERWISE WE MUST SAVE THE UVS AGAIN

        //statics
        public static int globalIndex = -1;
        public static List<ObjectMesh> allUniqueMeshes;

        //vars
        public int index = -1;
        public MeshHolder meshHolder;
        public UVVariant uv1var;     // LOCAL VALUE FROM MESH HOLDER TO KNOW WHICH VALUE OF THE LIST IS CONNECTED TO
        public UVVariant uv2var;
        public List<Material> materials;
        public List<ObjectProperty> meshProperties;
        public List<ObjectProperty> primitivesExtraProperties;
        public List<ObjectProperty> meshExtraProperties;

        public List<int> materialsInt;      //USED TO KNOW THE CONNECTION OF THE MATERIAL;

        public int lightmapIndex = -1;  //necessary to compare values with other materials
        //public List<ObjectProperty> materialProperties;

        public string exportName;

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueMeshes = new List<ObjectMesh>();
        }
        public static int GetMeshIndex(SkinnedMeshRenderer skinned_mesh, OffsetUVs offset_uvs)
        {
            if (skinned_mesh != null)
            {
                MeshHolder _meshHolder = MeshHolder.GetMeshHolder(skinned_mesh.sharedMesh,true);

                foreach (ObjectMesh om in allUniqueMeshes)
                {
                    if (om.IsSameMesh(_meshHolder, offset_uvs, skinned_mesh))
                    {
                        return om.index;
                    }
                }
                allUniqueMeshes.Add(new ObjectMesh(_meshHolder, offset_uvs, skinned_mesh));
                return allUniqueMeshes.Count - 1;
            }
            return -1;  // WILL RETURN -1, MEANS IT HAS NO MESH
        }
        public static int GetMeshIndex(Mesh mesh, OffsetUVs offset_uvs, MeshRenderer mesh_renderer)
        {
            if (mesh != null)
            {
                MeshHolder _meshHolder = MeshHolder.GetMeshHolder(mesh);

                foreach (ObjectMesh om in allUniqueMeshes)
                {
                    if (om.IsSameMesh(_meshHolder, offset_uvs, mesh_renderer))
                    {
                        return om.index;
                    }
                }
                allUniqueMeshes.Add(new ObjectMesh(_meshHolder, offset_uvs, mesh_renderer));
                return allUniqueMeshes.Count - 1;
            }
            return -1;  // WILL RETURN -1, MEANS IT HAS NO MESH
        }

        private bool IsSameMesh(MeshHolder mesh_holder, OffsetUVs offset_uvs, Renderer mesh_renderer)
        {
            if (mesh_renderer == null)
            {
                return false;
            }
            if (lightmapIndex != -1 || mesh_renderer.lightmapIndex != -1)   //IF THE MESH WE ARE COMPARING TO, OR RHIS MESH, HAS LIGHTMAP, THE MESH IS UNIQUE, SO RETURN FALSE
            {
                return false;
            }
            if (meshHolder == mesh_holder)
            {
                if (uv1var != null && offset_uvs != null)                     // CHECK FOR UVS 1
                {
                    if (!MeshHolder.IsSameUVs(uv1var, offset_uvs.offsetUVs, offset_uvs.scaleUVS))
                    {
                        return false;
                    }
                }
                // ALSO CHECK MATERIALS
                if (!HasSameMaterials(mesh_renderer.sharedMaterials, meshHolder.subMeshCount))
                {
                    return false;
                }

                return true;        // IF UVS ARE THE SAME, AND LIGHTMAP IS NOT NEEDED, THEN WE HAVE THE SAME VALUES
            }
            return false;
        }
        private bool HasSameMaterials(Material[] mesh_materials, int submesh_count)
        {
            if (meshHolder.subMeshCount != submesh_count)
                return false;

            for (int i =0; i < submesh_count; i++)      // MUST CHECK ONLY FOR THE QUANTITY OF SUBMESHES, NOT THE QUANTITIY OF MATERIALS
            {
                if (i < mesh_materials.Length)          // IF MESH RENDERER HAD MORE MATERIALS THAN NEEDED IT WILL STOP TO GET ONLY THE ONES NEEDED
                {

                    if (mesh_materials[i] != materials[i])
                        return false;
                }
                else                                // IF WERE OUT OF RANGE, COMPARE A NULL VALUE
                {
                    if (null != materials[i])
                        return false;
                }
            }
            return true;
        }
        public ObjectMesh(MeshHolder mesh_holder, OffsetUVs offset_uvs, Renderer mesh_renderer)
        {
            meshHolder = mesh_holder;
            primitivesExtraProperties = new List<ObjectProperty>();
            meshExtraProperties = new List<ObjectProperty>();
            meshProperties = new List<ObjectProperty>();

            materials = new List<Material>();
            for (int i = 0; i< meshHolder.subMeshCount; i++)
            {
                if (mesh_renderer == null)
                {
                    materials.Add(null);
                }
                else
                {
                    if (i < mesh_renderer.sharedMaterials.Length)           // ADD THE MATERIALS BASED ON SUBMESHES, NO ON HOW MANY MATERIALS THERE ARE
                    {
                        materials.Add(mesh_renderer.sharedMaterials[i]);
                    }
                    else                                                    // IF WERE OUT OF RANGE, ADD A NULL VALUES (PLACEHOLDER, A DEFAULT MATERIAL WILL BE USED INT NULLS)
                    {
                        materials.Add(null);
                    }
                }
            }

            // GET MATERIALS
            materialsInt = new List<int>();
            foreach (Material mat in materials)
            {
                materialsInt.Add(ObjectMaterial.GetMaterialIndex(mat, mesh_renderer));
            }

            // GET UVS
            Vector2 _offsetCompareValue = new Vector2(0, 0);
            Vector2 _scaleCompareValue = new Vector2(1, 1);
            if (offset_uvs != null)
            {
                _offsetCompareValue = offset_uvs.offsetUVs;
                _scaleCompareValue = offset_uvs.scaleUVS;
            }
            uv1var = meshHolder.GetUVsLocalIndex(_offsetCompareValue, _scaleCompareValue);

            lightmapIndex = -1;
            if (mesh_renderer != null) lightmapIndex = mesh_renderer.lightmapIndex;

            if (lightmapIndex != -1)
            {
                if (offset_uvs == null)
                    uv2var = meshHolder.AddUVsAuto(new Vector2(mesh_renderer.lightmapScaleOffset.z, mesh_renderer.lightmapScaleOffset.w), new Vector2 (mesh_renderer.lightmapScaleOffset.x, mesh_renderer.lightmapScaleOffset.y));
                else
                    uv2var = meshHolder.AddUVsAuto(new Vector2(mesh_renderer.lightmapScaleOffset.z, mesh_renderer.lightmapScaleOffset.w), new Vector2 (mesh_renderer.lightmapScaleOffset.x, mesh_renderer.lightmapScaleOffset.y), offset_uvs.offsetUVs, offset_uvs.scaleUVS);
            }

            index = globalIndex;
            globalIndex++;
        }





        // FINAL STEP: GET DATA
        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueMeshes.Count > 0)
            {
                result += "\"meshes\":[";
                foreach (ObjectMesh om in allUniqueMeshes)
                {
                    //result += "{\n";
                    //"\"primitives\" : [\n";
                    List<ObjectProperty> _primitives = new List<ObjectProperty>();
                    //SUBMESH GLTF SEPARATED (CLASSIC MODE)
                    if (!ExportToGLTF.options.exportSubmeshesInExtra)
                    {
                        for (int i = 0; i < om.meshHolder.accessorIndices.Count; i++)
                        {
                            
                            List<ObjectProperty> _attributes = new List<ObjectProperty>();
                            _attributes.Add(new ObjectProperty("POSITION", om.meshHolder.accessorVertices.export_index));
                            if (om.meshHolder.accessorNormals != null) _attributes.Add(new ObjectProperty("NORMAL", om.meshHolder.accessorNormals.export_index));

                            if (om.meshHolder.accessorWeights != null)
                            {
                                _attributes.Add(new ObjectProperty("WEIGHTS_0", om.meshHolder.accessorWeights.export_index));
                                _attributes.Add(new ObjectProperty("JOINTS_0", om.meshHolder.accessorJoints.export_index));
                            }
                            if (!ExportToGLTF.options.createUVOffsetExtras)
                            {
                                if (om.uv1var != null) _attributes.Add(new ObjectProperty("TEXCOORD_0", om.uv1var.GetObjectAccessorExportInt()));
                                if (om.uv2var != null) _attributes.Add(new ObjectProperty("TEXCOORD_1", om.uv2var.GetObjectAccessorExportInt()));
                            }
                            else
                            {
                                // IF WE CAN OFFSET WITH CODE, WE ONLY NEED TO GET OM.MESHHOLDER.ACCESSORUVS1 AND MULTIPLY IT BY OM.UV1VAR OFFSET/SCALE
                                if (om.meshHolder.accessorUVs1 != null) _attributes.Add(new ObjectProperty("TEXCOORD_0", om.meshHolder.accessorUVs1.export_index));
                                if (om.meshHolder.accessorUVs2 != null) _attributes.Add(new ObjectProperty("TEXCOORD_1", om.meshHolder.accessorUVs2.export_index));
                                // SCALE OFFSET WILL BE ADDED IN GETMESHEXTRAS()
                            }
                            List<ObjectProperty> _primi = new List<ObjectProperty>();
                            _primi.Add(new ObjectProperty("attributes", _attributes));
                            _primi.Add(new ObjectProperty("indices", om.meshHolder.accessorIndices[i].export_index));
                            if (om.materialsInt[i] != -1) _primi.Add(new ObjectProperty("material", om.materialsInt[i]));
                            _primitives.Add(new ObjectProperty("", _primi));

                            // RENDER MODE CAN GO HERE
                        }
                    }
                    //END SUBMESH GLTF SEPARATED (CLASSIC MODE)
                    //SUBMESH COMBINED GLTF (CUSTOM EXTRA MODE)
                    else
                    {
                        List<ObjectProperty> _attributes = new List<ObjectProperty>();
                        _attributes.Add(new ObjectProperty("POSITION", om.meshHolder.accessorVertices.export_index));
                        if (om.meshHolder.accessorNormals != null) _attributes.Add(new ObjectProperty("NORMAL", om.meshHolder.accessorNormals.export_index));
                        if (om.meshHolder.accessorWeights != null)
                        {
                            _attributes.Add(new ObjectProperty("WEIGHTS_0", om.meshHolder.accessorWeights.export_index));
                            _attributes.Add(new ObjectProperty("JOINTS_0", om.meshHolder.accessorJoints.export_index));
                        }

                        if (!ExportToGLTF.options.createUVOffsetExtras)
                        {
                            if (om.uv1var != null) _attributes.Add(new ObjectProperty("TEXCOORD_0",om.uv1var.GetObjectAccessorExportInt()));
                            if (om.uv2var != null) _attributes.Add(new ObjectProperty("TEXCOORD_1",om.uv2var.GetObjectAccessorExportInt()));
                        }
                        else
                        {
                            // IF WE CAN OFFSET WITH CODE, WE ONLY NEED TO GET OM.MESHHOLDER.ACCESSORUVS1 AND MULTIPLY IT BY OM.UV1VAR OFFSET/SCALE
                            if (om.meshHolder.accessorUVs1 != null) _attributes.Add(new ObjectProperty("TEXCOORD_0", om.meshHolder.accessorUVs1.export_index));
                            if (om.meshHolder.accessorUVs2 != null) _attributes.Add(new ObjectProperty("TEXCOORD_1", om.meshHolder.accessorUVs2.export_index));
                            // SCALE OFFSET WILL BE ADDED IN GETMESHEXTRAS()
                        }
                        if (om.meshHolder.subMeshCount > 1)
                        {
                            // THIS VALUES WILL BE ADDED LATER IN EXTRAS!
                            List<ObjectProperty> _submeshes = new List<ObjectProperty>();
                            for (int i = 0; i < om.meshHolder.subMeshCount; i++)
                            {
                                List<ObjectProperty> _submesh = new List<ObjectProperty>();
                                _submesh.Add(new ObjectProperty("start", om.meshHolder.subMeshStart[i]));
                                _submesh.Add(new ObjectProperty("count", om.meshHolder.subMeshSize[i]));
                                _submesh.Add(new ObjectProperty("material", om.materialsInt[om.meshHolder.subMeshCount - i -1]));   // INDICES ARE INVERTED AND SO ARE MATERIALS
                                _submeshes.Add(new ObjectProperty("",_submesh));
                            }
                            om.meshExtraProperties.Add(new ObjectProperty("submeshes", _submeshes,true));
                        }
                        List<ObjectProperty> _primi = new List<ObjectProperty>();
                        _primi.Add(new ObjectProperty("attributes", _attributes));
                        _primi.Add(new ObjectProperty("indices", om.meshHolder.accessorIndices[0].export_index));
                        if (om.materialsInt[0] != -1) _primi.Add(new ObjectProperty("material", om.materialsInt[0]));
                        _primitives.Add(new ObjectProperty("",_primi));

                    }
                    om.meshProperties.Add(new ObjectProperty("primitives",_primitives, true));
                    ObjectProperty _extras = om.GetExtras();
                    if (_extras != null)
                        om.meshProperties.Add(_extras);

                    //END SUBMESH COMBINED GLTF (CUSTOM EXTRA MODE)
                    result += ObjectProperty.GetObjectProperties(om.meshProperties) + ",\n";
                    //result += "]";
                    
                    //result += om.GetMeshExtrasProperties();
                    //result += "},\n";

                }

                //if (ExportToGLTF.options.exportNavMesh)
                //{

                //}
                
                if (allUniqueMeshes.Count > 0)
                    result = StringUtilities.RemoveCharacterFromString(result, 2, false);
                result += "]";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }

        public ObjectProperty GetExtras()
        {
            if (lightmapIndex != -1)
                meshExtraProperties.Add(new ObjectProperty("lightmap_index", lightmapIndex));

            if (meshExtraProperties.Count > 0)
            {
                ObjectExtraProperties _extra = new ObjectExtraProperties(meshExtraProperties);
                return new ObjectProperty(_extra);
            }
            return null;
        }
        //public string GetMeshExtrasProperties(bool add_comma = false)
        //{
        //    string result = "";
        //    if (lightmapIndex != -1)
        //        meshExtraProperties.Add(new ObjectProperty("lightmap_index", lightmapIndex));

        //    if (meshExtraProperties.Count > 0)
        //    {
        //        ObjectExtraProperties _extra = new ObjectExtraProperties(meshExtraProperties);
        //        ObjectProperty final_extras = new ObjectProperty(_extra);
        //        result += final_extras.GetPropertyGLTF();
        //    }
        //    if (result != "" && add_comma)
        //        result += ",\n";
        //    if (result != "")
        //        result = ",\n" + result;
        //    return result;
        //}
        public string GetPrimitivesExtrasProperties(bool add_comma = false)
        {
            string result = "";

            if (!ExportToGLTF.options.createUVOffsetExtras)
            {
                if (uv1var != null)
                {
                    if (uv1var.optionalObjectAccessor != null)
                    {
                        List<ObjectProperty> texcoord_0_props = new List<ObjectProperty>();
                        if (uv1var.optionalObjectAccessor.isQuantized)
                        {
                            //if (ValidateFloatArrayUse(uv1var.optionalObjectAccessor.quantization_scale, 1))
                                //texcoord_0_props.Add(new ObjectProperty("scale", uv1var.optionalObjectAccessor.quantization_scale)); //WRONG WAY. BUT SAVED, THIS VALUE IS NOT IMPORTANT, BUIT ITS IMPORTANT TO KNOW THE VALUE IS QUANTIZED
                            //if (ValidateFloatArrayUse(uv1var.optionalObjectAccessor.quantization_offset, 0))
                                //texcoord_0_props.Add(new ObjectProperty("offset", uv1var.optionalObjectAccessor.quantization_offset));//WRONG
                        }
                        if (texcoord_0_props.Count > 0)
                            primitivesExtraProperties.Add(new ObjectProperty("TEXCOORD_0", texcoord_0_props));
                    }
                }
                if (uv2var != null)
                {
                    List<ObjectProperty> texcoord_1_props = new List<ObjectProperty>();
                    if (uv2var.optionalObjectAccessor.isQuantized)
                    {
                        //if (ValidateFloatArrayUse(uv2var.optionalObjectAccessor.quantization_scale, 1))
                            //texcoord_1_props.Add(new ObjectProperty("scale", uv2var.optionalObjectAccessor.quantization_scale));  //WRONG, TAKE VALUE FROM OFFSET_UVS CODE TRANSFORMED TO QUANTIZED VALUE AND UNITIZED
                        //if (ValidateFloatArrayUse(uv2var.optionalObjectAccessor.quantization_offset, 0))
                            //texcoord_1_props.Add(new ObjectProperty("offset", uv2var.optionalObjectAccessor.quantization_offset));
                    }
                    if (texcoord_1_props.Count > 0)
                        primitivesExtraProperties.Add(new ObjectProperty("TEXCOORD_1", texcoord_1_props));
                }
            }
            if (primitivesExtraProperties.Count > 0)
            {
                ObjectExtraProperties _extra = new ObjectExtraProperties(primitivesExtraProperties);
                ObjectProperty final_extras = new ObjectProperty(_extra);
                result += final_extras.GetPropertyGLTF();
            }
            if (result != "" && add_comma)
                result += ",\n";

            //if (result != "")
            //{
            //    result = StringUtilities.RemoveCharacterFromString(result, 2, false) + "\n";
            //    result = "\"extras\":{\n" +
            //        "\"mesh_transform\":{\n" +
            //        result +
            //        "}\n" +
            //        "},\n";
            //}

            return result;

        }

        private bool ValidateFloatArrayUse(float[] _floats, float compare_value)
        {
            if (_floats == null)            // WE DONT NEED THE ARRAY SINCE THERE IS NO ARRAY
                return false;
            foreach(float f in _floats)
            {
                if (f != compare_value)
                    return true;            // IF THERES A DIFFERENTE VALUE, IT MEANS WE REQUIRE THEM
            }
            return false;                   // IF ALL VALUES ARE SAME AS COMPARE VALUE, WE DONT NEED THE ARRAY
        }
    }
}
