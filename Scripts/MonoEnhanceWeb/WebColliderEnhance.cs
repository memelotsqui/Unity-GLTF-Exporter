using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WEBGL_EXPORTER.GLTF
{
    [ExecuteAlways]
    public class WebColliderEnhance : MonoBehaviour
    {
        public bool setMassFromRigidBody = true;
        public float density = 1;
        public float gravityMultiplier = 1.0f;
        //public float restitution = 0;
        //public float friction = 0;

        Collider _collider;
        private void OnEnable()
        {
            _collider = transform.GetComponent<Collider>();
        }
        public float GetMass()
        {
            if (_collider != null)
            {
                Vector3 _bounds = new Vector3(_collider.bounds.extents.x * 2, _collider.bounds.extents.y * 2, _collider.bounds.extents.z * 2);
                float volume = _bounds.x * _bounds.y * _bounds.z;

                return volume * density;
            }
            else
            {
                return 1;
            }
        }
    }

    

#if UNITY_EDITOR
    [CustomEditor(typeof(WebColliderEnhance))]
    public class WebRigidbodyEnhanceEditor : Editor
    {
        private WebColliderEnhance myScript;
        private MonoScript script;
        
        private void OnEnable()
        {
            myScript = target as WebColliderEnhance;
            script = MonoScript.FromMonoBehaviour((WebColliderEnhance)target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false);
            //base.OnInspectorGUI();
            myScript.setMassFromRigidBody = EditorGUILayout.Toggle("Set Mass From RigidBody: ", myScript.setMassFromRigidBody);
            if (!myScript.setMassFromRigidBody)
            {
                myScript.density = EditorGUILayout.FloatField("Density: ", myScript.density);
                EditorGUILayout.LabelField("Calculated Mass:  " + myScript.GetMass() + "  kgs");
            }
            myScript.gravityMultiplier = EditorGUILayout.FloatField("Gravity Multiplier: ", myScript.gravityMultiplier);
            //myScript.friction = EditorGUILayout.FloatField("Friction: ", myScript.friction);
            //if (myScript.friction < 0)
            //    myScript.friction = 0;
            //myScript.restitution = EditorGUILayout.Slider("Restitution: ", myScript.restitution,0f,1f);

            
        }
    }
#endif
}
