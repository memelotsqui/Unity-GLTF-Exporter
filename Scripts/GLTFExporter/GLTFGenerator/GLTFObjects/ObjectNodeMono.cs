using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectNodeMono : MonoBehaviour
    {
        //statics
        public static int nodeCounter = -1;
        public static List<ObjectNodeMono> allUniqueNodes;
        public static List<int> sceneNodes;

        public static Mesh navMesh;
        public static int navMeshNode = -1;
        public static int navMeshConnectedMesh = -1;

        public static int totalMeshes = -1;
        //public static int totalSkinnedMeshes = -1;

        //vars
        public int connectedMesh = -1;  // MESH HAS THE MATERIAL CONNECTIONS!

        public int connectedSkinnedMesh = -1;
        public int connectedSkin = -1;  //IN CASE ITS A SKINNED MESH
        public int connectedCamera = -1; // IN CASE WE HAVE A CONNECTED CAMERA TO THIS NODE

        //public bool batching = false;
        public int node = -1;
        public List<int> nodeChilds;    // all node childs ints
        public int nodeParent = -1;     // node parent -1 means is in scene
        public List<ObjectProperty> extraProperties;
        public ObjectProperty extensions;

        public static void ResetValues()
        {
            nodeCounter = 0;
            allUniqueNodes = new List<ObjectNodeMono>();
            sceneNodes = new List<int>();

            navMesh = null;
            navMeshNode = -1;
            navMeshConnectedMesh = -1;


            totalMeshes = 0;
            //totalSkinnedMeshes = 0;
        }
        public static void SetupNavMesh(Transform parent)
        {
            // NAV MESH NODE
            
            NavMeshTriangulation nav = NavMesh.CalculateTriangulation();
            if (nav.indices.Length > 0)
            {
                navMesh = new Mesh();
                navMesh.SetVertices(nav.vertices);
                navMesh.SetIndices(nav.indices,MeshTopology.Triangles,0);
                navMeshNode = nodeCounter + totalMeshes;
 
                navMeshConnectedMesh = ObjectMesh.GetMeshIndex(navMesh, null, null);

                ObjectNodeMono node_parent = parent.GetComponent<ObjectNodeMono>();
                node_parent.nodeChilds.Add(navMeshNode);

                totalMeshes++;
                nodeCounter++;
            }

        }
        private void ResetInstance()
        {
            connectedMesh = -1; 
            connectedSkin = -1; 
            connectedCamera = -1;
            connectedSkinnedMesh = -1;

        }
        public void SetupNode(bool is_active = true)
        {
            // RESET ALSO THIS NODE IN CASE IT ALREADY EXISTED
            ResetInstance();
            GetConnectedMesh();
            GetConnectedCamera(); 

            allUniqueNodes.Add(this);
            node = nodeCounter;
            nodeChilds = new List<int>();
            extraProperties = new List<ObjectProperty>();
            extensions = new ObjectProperty();

            GetConnectedAnimator();

            GetConnectedAudio();

            GetConnectedColliders();

            if (ExportToGLTF.options != null) {
                if (!is_active)
                {
                    extraProperties.Add(new ObjectProperty("visible",false));
                }
                if (ExportToGLTF.options.exportGameObjectsTag) 
                {
                    if (transform.tag != "Untagged")
                        extraProperties.Add(new ObjectProperty("tag", transform.tag));
                }
                if (ExportToGLTF.options.exportGameObjectsLayer)
                {
                    if (gameObject.layer != 0)
                        extraProperties.Add(new ObjectProperty("layer", gameObject.layer));
                }
                if (ExportToGLTF.options.exportBatching)
                {
#if UNITY_EDITOR
                    if (GameObjectUtility.GetStaticEditorFlags(gameObject).HasFlag(StaticEditorFlags.BatchingStatic))
                    {
                        extraProperties.Add(new ObjectProperty("batching", true));
                    }
                        #endif
                }
            }

            Transform parent = transform.parent;
            if (nodeCounter == 0)
            {
                ObjectScene.AddNodeToScene(node);
            }
            else
            {
                ObjectNodeMono node_parent = transform.parent.GetComponent<ObjectNodeMono>();
                nodeParent = node_parent.node;
                node_parent.nodeChilds.Add(node);

            }
            nodeCounter++;

        }
        public void GetConnectedCamera()
        {
            if (ExportToGLTF.options.exportCameras)
            {
                Camera _cam = transform.GetComponent<Camera>();
                string _tag = "";
                if (transform.tag == "MainCamera")
                    _tag = transform.tag;

                if (_cam != null)
                {
                    if (!_cam.orthographic)
                        connectedCamera = ObjectCamera.GetCameraIndex(1.0f, _cam.fieldOfView * Mathf.Deg2Rad, _cam.farClipPlane, _cam.nearClipPlane, _tag, _cam.cullingMask);
                    else
                        connectedCamera = ObjectCamera.GetCameraIndex(new Vector2(_cam.orthographicSize, _cam.orthographicSize), _cam.farClipPlane, _cam.nearClipPlane, _tag, _cam.cullingMask);
                }
            }
            else
            {
                connectedCamera = -1;
            }
        }
        public void GetConnectedMesh()
        {
            MeshFilter _mf= transform.GetComponent<MeshFilter>();
            MeshRenderer _mr = transform.GetComponent<MeshRenderer>();

            //new
            SkinnedMeshRenderer _smr = transform.GetComponent<SkinnedMeshRenderer>();

            if (_smr != null)
            {
                if (_smr.sharedMesh != null)
                {
                    if (_smr.sharedMesh.GetIndices(0).Length != 0)
                    {
                        //ObjectNodeMono targetRootJoint = _smr.rootBone.GetComponent<ObjectNodeMono>();
                        //Mesh _mesh = _smr.sharedMesh;

                        OffsetUVs _ou = null;
                        //pending
                        //OffsetUVs _ou = transform.GetComponent<OffsetUVs>();

                        //if (_ou != null)
                        //_mesh = transform.GetComponent<OffsetUVsOrigMesh>().originalMesh;

                        connectedSkinnedMesh = ObjectMesh.GetMeshIndex(_smr,_ou);
                        Debug.LogWarning("connectedSkinnedMesh: " + connectedSkinnedMesh);
                        connectedSkin = ObjectSkin.GetSkinIndex(_smr);
                        totalMeshes++;
                        //rootNodeSkin = _smr.rootBone;

                        // WE WILL NOT SAVE IT IN THE OBJECT THAT HOLDS THE COMPONENT, BUT IN THE ROOT OF THE JOINS
                        //if (targetRootJoint.connectedMesh == -1)
                        //{
                        //    targetRootJoint.connectedSkinnedMesh = ObjectMesh.GetMeshIndex(_smr, _ou);
                        //    targetRootJoint.connectedSkin = ObjectSkin.GetSkinIndex(_smr.bones, _smr.rootBone);
                        //    totalSkinnedMeshes++;  
                        //}
                        //else
                        //{
                        //    Debug.LogWarning("MORE THAN 2 MESHES IN ROOT JOINT IN SKIN");
                        //}
                    }
                }
                else
                {
                    Debug.LogWarning("missing skinned mesh in " + _smr.gameObject.name);
                }
            }
            if (_mf!= null && _mr != null)
            {
                if (_mf.sharedMesh != null)
                {
                    if (_mf.sharedMesh.GetIndices(0).Length != 0)
                    {

                        Mesh _mesh = _mf.sharedMesh;
                        OffsetUVs _ou = transform.GetComponent<OffsetUVs>();

                        if (_ou != null)
                            _mesh = transform.GetComponent<OffsetUVsOrigMesh>().originalMesh;

                        connectedMesh = ObjectMesh.GetMeshIndex(_mesh, _ou, _mr);

                        totalMeshes++;
                    }
                }
                else
                {
                    Debug.LogWarning("missing mesh in " + _mf.gameObject.name);
                }
            }
        }
        public void GetConnectedAnimator()
        {
            Animator _animator = GetComponent<Animator>();
            if (_animator != null)
            {
                ObjectExtrasAnimationController.GetAnimatorIndex(_animator, node);
            }
        }

        public void GetConnectedAudio()
        {
            WebPositionalAudioSource _webAudioSource = GetComponent<WebPositionalAudioSource>();
            if (_webAudioSource != null)
            {
                int index = ObjectExtensionOmiAudio.GetAudioEmitterIndex(_webAudioSource);
                extensions.AddExtensionObject(CreateKHR_audio(index));
            }
            else
            {
                AudioSource _audioSource = GetComponent<AudioSource>();
                if (_audioSource != null)
                {
                    int index = ObjectExtensionOmiAudio.GetAudioEmitterIndex(_webAudioSource);
                    extensions.AddExtensionObject(CreateKHR_audio(index));
                }
            }
        }

        public void GetConnectedColliders()
        {
            
            Collider _collider = GetComponent<Collider>();
            if (_collider!= null)
            {
                Rigidbody _rigidBody = GetComponent<Rigidbody>();
                List<ObjectProperty> properties = ObjectExtensionOmiCollider.GetColliderProperties(_collider, _rigidBody);
                extensions.AddExtensionObject(CreateOMI_collider(properties));
                //int index = ObjectExtensionOmiCollider.GetColliderIndex(_collider);
            }
        }
        public static ObjectExtension CreateKHR_audio(int index)
        {
            List<ObjectProperty> _properties = new List<ObjectProperty>();
            if (index >= 0)
            {
                _properties.Add(new ObjectProperty("emitter", index));
                ObjectExtension khr_audio = new ObjectExtension("KHR_audio", _properties, false);
                return khr_audio;
            }
            return null;
        }
        public static ObjectExtension CreateOMI_collider(List<ObjectProperty> properties)
        {
            
            if (properties.Count > 0)
            {
                ObjectExtension omi_collider = new ObjectExtension("OMI_collider", properties, false);
                return omi_collider;
            }
            return null;
        }
        public static int leafNodesCounter;
        public static string GetGLTFData(bool add_end_comma = true)
        {
            // FOR QUANTIZATION
            leafNodesCounter = allUniqueNodes.Count;
            List<int>  leaf_node_meshes = new List<int>();
            List<string> leaf_node_names = new List<string>();

            List<int> root_skin_nodes = new List<int>();

            Quaternion defaultQuaternion = new Quaternion(0, 0, 0, 1);

            string result = "\"nodes\":[";
            List<ObjectProperty> _allNodes = new List<ObjectProperty>();
            foreach (ObjectNodeMono onm in allUniqueNodes)
            {
                //DECLARATION
                List<ObjectProperty> _node = new List<ObjectProperty>();
                if (ExportToGLTF.options.exportGameObjectName)
                    _node.Add(new ObjectProperty("name", StringUtilities.GetStringJsonLegal(onm.gameObject.name)));

                //CAMERA
                if (onm.connectedCamera != -1) _node.Add(new ObjectProperty("camera", onm.connectedCamera));

                //CHILDS
                if (onm.connectedMesh != -1)
                {

                    if (ExportToGLTF.options.blockLeafNodesCreation == false)
                    {
                        onm.nodeChilds.Add(leafNodesCounter);
                        leafNodesCounter++;
                        leaf_node_meshes.Add(onm.connectedMesh);
                        leaf_node_names.Add(StringUtilities.GetStringJsonLegal(onm.gameObject.name));
                        root_skin_nodes.Add(-1);
                    }
                    else
                    {
                        _node.Add(new ObjectProperty("mesh", onm.connectedMesh));
                        // place the information here
                    }
                   
                }
                if (onm.connectedSkinnedMesh != -1)
                {
                    //root.Add(leafNodesCounter);
                    Debug.Log("jahsbd");
                    Debug.Log(onm.connectedSkin);
                    if (ExportToGLTF.options.blockLeafNodesCreation == false)
                    {
                        leafNodesCounter++;
                        leaf_node_meshes.Add(onm.connectedSkinnedMesh);
                        leaf_node_names.Add(StringUtilities.GetStringJsonLegal(onm.gameObject.name));
                        root_skin_nodes.Add(onm.connectedSkin);
                    }
                    else
                    {
                        _node.Add(new ObjectProperty("mesh", onm.connectedSkinnedMesh));
                        ObjectScene.AddNodeToScene(onm.connectedSkin);
                        _node.Add(new ObjectProperty("skin", onm.connectedSkin));
                    }
                    
                    //Debug.LogWarning("skinned mesh is " + onm.connectedSkinnedMesh);
                    //_node.Add(new ObjectProperty("mesh", onm.connectedSkinnedMesh));
                    //_node.Add(new ObjectProperty("skin", onm.connectedSkin));

                }

                if (onm.nodeChilds.Count > 0) _node.Add(new ObjectProperty("children", onm.nodeChilds));

                //TRANSFORM
                Quaternion final_rotation = onm.transform.localRotation;
                final_rotation = Quaternion.Euler(-onm.transform.localEulerAngles.x, -onm.transform.localEulerAngles.y, onm.transform.localEulerAngles.z);
                if (onm.transform.localPosition != Vector3.zero) 
                    _node.Add(new ObjectProperty("translation", onm.transform.localPosition)); ;
                if (final_rotation != defaultQuaternion) 
                    _node.Add(new ObjectProperty("rotation", final_rotation));
                if (onm.transform.localScale != Vector3.one) 
                    _node.Add(new ObjectProperty("scale", onm.transform.localScale,false));

                //EXTENSIONS
                if (onm.extensions.extensionObjects != null)
                    if (onm.extensions.extensionObjects.Count > 0)
                        _node.Add(onm.extensions);

                //EXTRAS
                ObjectProperty extras = onm.GetNodeExtraObjectGLTFData();
                if (extras != null)
                    _node.Add(extras);

                _allNodes.Add(new ObjectProperty("",_node));
            }

            // LEAF NODES
            for (int i = 0; i < leaf_node_meshes.Count; i++)
            {
                //result += CreateLeafNode(leaf_node_names[i], leaf_node_meshes[i]);
                //Debug.LogWarning(nodeCounter);
                if (ExportToGLTF.options.blockLeafNodesCreation == false)
                {
                    List<ObjectProperty> _node = CreateLeafNode(leaf_node_names[i], leaf_node_meshes[i], root_skin_nodes[i], nodeCounter + i);
                    _allNodes.Add(new ObjectProperty("", _node));
                }
            }

            //// NAV MESH NODE
            if (navMeshNode != -1)
            {
                List<ObjectProperty> _node = new List<ObjectProperty>();
                if (ExportToGLTF.options.exportGameObjectName)
                    _node.Add(new ObjectProperty("name", "_navMesh_"));
                _node.Add(new ObjectProperty("mesh", navMeshConnectedMesh));

                List<ObjectProperty> extraProperties = new List<ObjectProperty>();
                extraProperties.Add(new ObjectProperty("visible", false));

                ObjectExtraProperties _extras = new ObjectExtraProperties(extraProperties);
                _node.Add(new ObjectProperty(_extras));

                _allNodes.Add(new ObjectProperty("", _node));

            }
            result += ObjectProperty.GetObjectProperties(_allNodes,null,true);

            //result = StringUtilities.RemoveCharacterFromString(result,2,false);
            result += "]";
            if (add_end_comma)
                result += ",\n";

            return result;
        }


        private static List<ObjectProperty> CreateLeafNode(string _name,int mesh_int, int skin_int, int node_id)
        {

            List<ObjectProperty> _node = new List<ObjectProperty>();
            if (ExportToGLTF.options.exportGameObjectName)
                _node.Add(new ObjectProperty("name", "_geom_"+_name));
            _node.Add(new ObjectProperty("mesh",mesh_int));
            if (skin_int != -1) 
            {
                ObjectScene.AddNodeToScene(node_id);
                _node.Add(new ObjectProperty("skin", skin_int)); 
            }
            if (ExportToGLTF.options.quantizeGLTF && ExportToGLTF.options.blockLeafNodesCreation == false)
            {
                if (ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_offset != null)
                {
                    float[] _offset = new float[3];
                    _offset[0] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_offset[0];
                    _offset[1] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_offset[1];
                    _offset[2] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_offset[2];

                    _node.Add(new ObjectProperty("translation", new Vector3(_offset[0], _offset[1], _offset[2])));
                }
                if (ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_scale != null)
                {
                    float[] _scale = new float[3];

                    _scale[0] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_scale[0];
                    _scale[1] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_scale[1];
                    _scale[2] = ObjectMesh.allUniqueMeshes[mesh_int].meshHolder.accessorVertices.quantization_scale[2];

                    _node.Add(new ObjectProperty("scale", new Vector3(_scale[0], _scale[1], _scale[2])));
                }
            }
            return _node;

        }
     

        private ObjectProperty GetNodeExtraObjectGLTFData()
        {

            ObjectNodeUserExtrasMono[] user_extras = transform.GetComponents<ObjectNodeUserExtrasMono>();
            foreach (ObjectNodeUserExtrasMono ue in user_extras)
            {
                ue.optionalIdentifier = -1;
            }
            for (int i = 0; i < user_extras.Length; i++)
            {
                if (user_extras[i].extrasName != "")
                {
                    bool saved = false;
                    
                    //user_extras[i].SetGLTFLastData();
                    if (user_extras[i].computedProperties != null)
                    {
                        if (user_extras[i].computedProperties.Count > 0)
                        {
                            extraProperties.Add(new ObjectProperty(user_extras[i].extrasName, user_extras[i].computedProperties));
                            saved = true;
                        }
                    }
                    if (!saved)
                        extraProperties.Add(new ObjectProperty(user_extras[i].extrasName));
                }
            }
            
            if (extraProperties.Count > 0)
            {
                ObjectExtraProperties _extra = new ObjectExtraProperties(extraProperties);
                return new ObjectProperty(_extra);
            }
            return null;

        }

    }
}
