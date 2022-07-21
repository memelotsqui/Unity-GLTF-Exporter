using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    //https://github.com/omigroup/gltf-extensions/pull/63/files
    public class ObjectExtensionOmiCollider
    {
        //statics 
        //public static int globalIndex = -1;
        //public static List<ObjectExtensionOmiCollider> allUniqueColliders;

        //vars
        //public int index = -1;      //the index of the audio emitter
        public List<ObjectProperty> colliderProperties;

        //public static void Reset()
        //{
        //    globalIndex = 0;
        //    allUniqueColliders = new List<ObjectExtensionOmiCollider>();
        //}
        //public static int GetColliderIndex(Collider collider_source)
        //{
        //    if (collider_source == null)
        //        return -1;

        //    allUniqueColliders.Add(new ObjectExtensionOmiCollider(collider_source));

        //    return allUniqueColliders.Count - 1;

        //}

        //public ObjectExtensionOmiCollider(Collider collider)
        //{
        //    index = globalIndex;

        //    colliderProperties = GetColliderProperties(collider);

        //    globalIndex++;
        //}

        public static List<ObjectProperty> GetColliderProperties(Collider collider, Rigidbody rigidBody)
        {
            if (collider == null)
                return null;
            List<ObjectProperty> _props = new List<ObjectProperty>();

            switch (collider.GetType().Name)
            {
                case "MeshCollider":
                    MeshRenderer mr = collider.transform.GetComponent<MeshRenderer>();
                    ObjectNodeMono node = collider.transform.GetComponent<ObjectNodeMono>();
                    if (mr != null)
                    {
                        int connectedMesh = node.connectedMesh;
                        MeshCollider mc = collider as MeshCollider;
                        if (mc.convex)
                        {
                            _props.Add(new ObjectProperty("type", "hull")); //hull and mesh will point towards the mesh, in the case of hull, every engine must be responsible for the convex hull creation
                            _props.Add(new ObjectProperty("mesh", connectedMesh)); 

                        }
                        else
                        {
                            _props.Add(new ObjectProperty("type", "mesh"));
                            _props.Add(new ObjectProperty("mesh", connectedMesh));
                        }
                    }
                    break;

                case "SphereCollider":
                    SphereCollider sc = collider as SphereCollider;
                    _props.Add(new ObjectProperty("type", "sphere"));
                    _props.Add(new ObjectProperty("radius", sc.radius));
                    _props.Add(new ObjectProperty("center", sc.center));    //check if negative z
                    break;

                case "BoxCollider":
                    BoxCollider bc = collider as BoxCollider;
                    _props.Add(new ObjectProperty("type", "box"));
                    _props.Add(new ObjectProperty("extents", bc.size,false));
                    _props.Add(new ObjectProperty("center", bc.center));

                    break;

                case "CapsuleCollider":
                    CapsuleCollider cc = collider as CapsuleCollider;
                    _props.Add(new ObjectProperty("type", "capsule"));
                    _props.Add(new ObjectProperty("radius", cc.radius));
                    _props.Add(new ObjectProperty("height", cc.height));
                    _props.Add(new ObjectProperty("center", cc.center));    //check if negative z
                    break;
                    //compound??
                    
            }
            if (collider.isTrigger)
                _props.Add(new ObjectProperty("sensor", true));

            if (collider.sharedMaterial != null){
                PhysicMaterial mat = collider.sharedMaterial;
                List<ObjectProperty> _material = new List<ObjectProperty>();
                _material.Add(new ObjectProperty("friction", mat.dynamicFriction));
                _material.Add(new ObjectProperty("restitution", mat.bounciness));
                _material.Add(new ObjectProperty("combineFriction", GetPhysicsMaterialCombineString(mat.frictionCombine)));
                _material.Add(new ObjectProperty("combineRestitution", GetPhysicsMaterialCombineString(mat.bounceCombine)));
                _props.Add(new ObjectProperty("material", _material));
            }
            
            

            //collider.bounds
            if (rigidBody != null)
            {
                WebColliderEnhance _enh = rigidBody.transform.GetComponent<WebColliderEnhance>();
                List<ObjectProperty> _rigidBodyProps = new List<ObjectProperty>();
                //save current mass
                Vector3 _bounds = new Vector3(collider.bounds.extents.x * 2, collider.bounds.extents.y * 2, collider.bounds.extents.z * 2);
                float _volume = _bounds.x * _bounds.y * _bounds.z;
                float _density = rigidBody.mass/_volume; //= calc volume / rigidBody.mass;
                float _gravity = rigidBody.useGravity == true ? 1 : 0;
                if (_enh != null)
                {
                    if (!_enh.setMassFromRigidBody) {
                        _density = _enh.density;
                    }
                    _gravity *= _enh.gravityMultiplier;
                    //if (_enh.friction != 0)
                    //    _rigidBodyProps.Add(new ObjectProperty("friction", _enh.friction));
                    //if (_enh.restitution != 0)
                    //    _rigidBodyProps.Add(new ObjectProperty("restitution", _enh.restitution));

                }

                if (rigidBody.isKinematic)
                    _rigidBodyProps.Add(new ObjectProperty("isKinematic", true));
                if (_gravity != 1)
                    _rigidBodyProps.Add(new ObjectProperty("gravity", _gravity));
                if (_density != 1)
                    _rigidBodyProps.Add(new ObjectProperty("density", _density));

                if (rigidBody.drag != 0)
                    _rigidBodyProps.Add(new ObjectProperty("damp", rigidBody.drag));
                if (rigidBody.angularDrag != 0)
                    _rigidBodyProps.Add(new ObjectProperty("angularDamp", rigidBody.angularDrag));


                bool[] _constraints = GetConstraints((int)rigidBody.constraints);

                if (_constraints[0] || _constraints[1] || _constraints[2])
                {
                    List<ObjectProperty> _positionLock = new List<ObjectProperty>();
                    _positionLock.Add(new ObjectProperty("x", _constraints[0]));
                    _positionLock.Add(new ObjectProperty("y", _constraints[1]));
                    _positionLock.Add(new ObjectProperty("z", _constraints[2]));
                    _rigidBodyProps.Add(new ObjectProperty("positionLock", _positionLock));
                }
                if (_constraints[0] || _constraints[1] || _constraints[2])
                {
                    List<ObjectProperty> _rotationLock = new List<ObjectProperty>();
                    _rotationLock.Add(new ObjectProperty("x", _constraints[3]));
                    _rotationLock.Add(new ObjectProperty("y", _constraints[4]));
                    _rotationLock.Add(new ObjectProperty("z", _constraints[5]));
                    _rigidBodyProps.Add(new ObjectProperty("rotationLock", _rotationLock));
                }
                
                if (rigidBody.collisionDetectionMode != CollisionDetectionMode.Discrete)
                {
                    _rigidBodyProps.Add(new ObjectProperty("ccd", true));
                }
                _props.Add(new ObjectProperty("rigidBody", _rigidBodyProps));
            }

            return _props;
        }
        private static string GetPhysicsMaterialCombineString(PhysicMaterialCombine matCombine)
        {
            switch (matCombine)
            {
                case PhysicMaterialCombine.Average:
                    return "average";
                case PhysicMaterialCombine.Minimum:
                    return "min";
                case PhysicMaterialCombine.Maximum:
                    return "max";
                case PhysicMaterialCombine.Multiply:
                    return "multiply";
            }
            return "";
        }
        private static bool[] GetConstraints(int enumVal)
        {
            bool[] _constraints = new bool[6];
            for (int i = 0; i < _constraints.Length; i++)
                _constraints[i] = false;
            if (enumVal == 0 || enumVal == 14 || enumVal == 112 || enumVal == 126)
            {
                switch (enumVal)
                {
                    case 0:
                        break;
                    case 14:
                        _constraints[0] = _constraints[1] = _constraints[2] = true;
                        break;
                    case 112:
                        _constraints[3] = _constraints[4] = _constraints[5] = true;
                        break;
                    case 126:
                        _constraints[0] = _constraints[1] = _constraints[2] = _constraints[3] = _constraints[4] = _constraints[5] = true;
                        break;
                }
            }
            else
            {

                getActiveConstrain(ref _constraints, enumVal);

            }

            return _constraints;
        }

        private static void getActiveConstrain(ref bool[] constrains, int val)
        {
            if (val >= 64)
            {
                constrains[5] = true;
                val -= 64;
            }

            if (val >= 32)
            {
                constrains[4] = true;
                val -= 32;
            }

            if (val >= 16)
            {
                constrains[3] = true;
                val -= 16;
            }

            if (val >= 8)
            {
                constrains[2] = true;
                val -= 8;
            }

            if (val >= 4)
            {
                constrains[1] = true;
                val -= 4;
            }

            if (val >= 2)
            {
                constrains[0] = true;
            }

        }
        //public static void AddGLTFDataToExtensions()    //global section of extension!
        // {
        //if (allUniqueColliders.Count > 0)
        //{
        //    List<ObjectProperty> _OMI_collider = new List<ObjectProperty>();

        //    List<ObjectProperty> _emitters = new List<ObjectProperty>();

        //    foreach (ObjectExtensionOmiCollider oc in allUniqueColliders)
        //    {
        //        _OMI_collider.Add(new ObjectProperty("",oc.colliderProperties));
        //    }

        //    ObjectExtension.Add(new ObjectProperty("OMI_collider", _OMI_collider));
        //}
        // }
    }
}
