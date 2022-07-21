using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF { 

    [CustomEditor(typeof(WebPositionalAudioSource))]
    public class WebPositionalAudioSource_Editor : Editor
    {
        WebPositionalAudioSource myScript;
        MonoScript script;

        string emitterName;
        float gain;   //volume
        bool loop;
        bool playing;

        //bool editPitchSpeedSeparatedly;
        float speed;  //pitch
        float pitch;         //check if its not the same as speed?

        float coneInnerAngleDegree;
        float coneOuterAngleDegree;
        float coneOuterGain;
        WebPositionalAudioSource.Distance distance;

        //only works when distance model is set to linear
        float maxDistance;
        float refDistance;
        //whens set to linear max value is one, otherwise max value can go higher
        float rollOffFactor = 1f;
        float rollOffFactorLinear = 1f;

        AudioClip audioClip;

        private void OnEnable()
        {
            myScript = (WebPositionalAudioSource)target;
            script = MonoScript.FromMonoBehaviour((WebPositionalAudioSource)target);

            //if (!myScript.setupCreated)
            //{
            //    myScript.setupCreated = true;
            //    UnityEditorInternal.ComponentUtility.MoveComponentUp(myScript);
            //}
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script: ", script, typeof(MonoScript), false);
            //myScript.audioSource = (AudioSource)EditorGUILayout.ObjectField("Audio Clip: ", audioSource, typeof(AudioClip), false);
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();

            emitterName = EditorGUILayout.TextField("Emitter Name (optional): ", myScript.emitterName);
            audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip: ", myScript.clip, typeof(AudioClip), false);
            gain = EditorGUILayout.FloatField("Gain: ", myScript.gain);

            EditorGUILayout.Space();
            speed = EditorGUILayout.FloatField("Speed: ", myScript.speed);
            pitch = EditorGUILayout.FloatField("Pitch: ", myScript.pitch);

            EditorGUILayout.BeginHorizontal();
            loop = EditorGUILayout.Toggle("Loop: ", myScript.loop);
            playing = EditorGUILayout.Toggle("Playing: ", myScript.playing);
            EditorGUILayout.EndHorizontal();

            
            if (!myScript.isGlobalAudio)
            {
                if (GUILayout.Button("Change To Global Audio", GUILayout.Height(40f)))
                {
                    myScript.isGlobalAudio = true;
                }
                GuiLayoutExtras.CenterLabel("Cone angles in degrees", 20);
                EditorGUILayout.Space();
                coneInnerAngleDegree = EditorGUILayout.FloatField("Cone Inner Angle (red): ", myScript.coneInnerAngleDegree);
                coneOuterAngleDegree = EditorGUILayout.FloatField("Cone Outer Angle (blue): ", myScript.coneOuterAngleDegree);
                coneOuterGain = EditorGUILayout.FloatField("Cone Outer Gain: ", myScript.coneOuterGain);
                distance = (WebPositionalAudioSource.Distance)EditorGUILayout.EnumPopup("Distance: ", myScript.distance);

                EditorGUILayout.Space();
                if (myScript.distance == WebPositionalAudioSource.Distance.linear)
                {
                    maxDistance = EditorGUILayout.FloatField("Max Distance: ", myScript.maxDistance);
                }
                else
                {
                    maxDistance = myScript.maxDistance;
                }


                refDistance = EditorGUILayout.FloatField("Ref Distance: ", myScript.refDistance);
                if (myScript.distance == WebPositionalAudioSource.Distance.linear)
                {
                    rollOffFactorLinear = EditorGUILayout.Slider("Roll Off Factor: ", myScript.rollOffFactorLinear, 0f, 1f);
                    rollOffFactor = myScript.rollOffFactor;
                }
                else
                {
                    rollOffFactor = EditorGUILayout.FloatField("Roll Off Factor: ", myScript.rollOffFactor);
                    rollOffFactorLinear = myScript.rollOffFactorLinear;
                }
            }
            else
            {
                if (GUILayout.Button("Change To Positional Audio", GUILayout.Height(40f)))
                {
                    myScript.isGlobalAudio = false;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myScript,"edit web audio");
                SaveChanges();
            }
            //base.OnInspectorGUI();
        }

        private void SaveChanges()
        {
            if (myScript.clip != audioClip) 
            {
                Debug.Log("clip changed");
                string extension = StringUtilities.GetExtensionFromObjectAsset(audioClip).ToLower();
                if (extension == "mp3" || extension == "wav")
                {
                    myScript.clip = audioClip;
                }
                else
                {
                    Debug.LogError("Only .mp3 or .wav format is supported. Skipping");
                }
            }
            myScript.emitterName = emitterName;
            myScript.gain = gain;

            myScript.speed = speed;
            myScript.loop = loop;
            myScript.playing = playing;
            myScript.pitch = pitch;

            myScript.coneInnerAngleDegree = coneInnerAngleDegree;
            myScript.coneOuterAngleDegree = coneOuterAngleDegree;
            myScript.coneOuterGain = coneOuterGain;
            myScript.distance = distance;

            myScript.maxDistance = maxDistance;
            myScript.refDistance = refDistance;
            myScript.rollOffFactor = rollOffFactor;
            myScript.rollOffFactorLinear = rollOffFactorLinear;
        }

        private void OnSceneGUI()
        {
            if (myScript.enabled)
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(myScript.transform.position, myScript.transform.position + (myScript.transform.TransformDirection(Vector3.back) * 3f));

                Handles.color = Color.red;
                Vector3 inner_1 = Quaternion.Euler(0, myScript.coneInnerAngleDegree / 2, 0) * myScript.transform.TransformDirection(Vector3.back);
                Vector3 inner_2 = Quaternion.Euler(0, -myScript.coneInnerAngleDegree / 2, 0) * myScript.transform.TransformDirection(Vector3.back);
                Handles.DrawLine(myScript.transform.position, myScript.transform.position + (inner_1 * 3f));
                Handles.DrawLine(myScript.transform.position, myScript.transform.position + (inner_2 * 3f));

                Handles.color = Color.blue;
                Vector3 outer_1 = Quaternion.Euler(0, myScript.coneOuterAngleDegree / 2, 0) * myScript.transform.TransformDirection(Vector3.back);
                Vector3 outer_2 = Quaternion.Euler(0, -myScript.coneOuterAngleDegree / 2, 0) * myScript.transform.TransformDirection(Vector3.back);
                Handles.DrawLine(myScript.transform.position, myScript.transform.position + (outer_1 * 3f));
                Handles.DrawLine(myScript.transform.position, myScript.transform.position + (outer_2 * 3f));
            }
        }
    }
}
