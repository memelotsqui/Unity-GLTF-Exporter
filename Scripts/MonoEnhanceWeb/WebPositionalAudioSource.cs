using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    //[RequireComponent(typeof(AudioSource))]
    //[ExecuteAlways]
    public class WebPositionalAudioSource : MonoBehaviour
    {
        public enum Distance { linear = 0,inverse = 1, exponential = 2 }

        public string emitterName;
        public AudioClip clip;
        
        public float gain = 1.0f;   //volume
        public bool loop = true;
        public bool playing = true;

        public float speed = 1.0f; 
        public float pitch = 1.0f;

        public bool isGlobalAudio = false;

        public float coneInnerAngleDegree = 45f; 
        public float coneOuterAngleDegree = 45f; 
        public float coneOuterGain = 0f;
        public Distance distance = Distance.inverse;

        //only works when distance model is set to linear
        public float maxDistance = 10000f;
        public float refDistance = 1f;
        //whens set to linear max value is one, otherwise max value can go higher
        public float rollOffFactor = 1f;
        public float rollOffFactorLinear = 1f;



        private void Start()
        {
            // used to have it enabled or disabled
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position,"unity-to-threejs/WebPositionalAudioSource.png",true);
        }
#endif
    }

}
