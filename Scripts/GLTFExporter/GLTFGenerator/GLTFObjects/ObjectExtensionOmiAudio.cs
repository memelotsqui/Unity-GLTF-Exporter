using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtensionOmiAudio
    {
        //consts
        public const string exportPreName = "aud_";

        //statics 
        public static int globalIndex = -1;
        public static List<ObjectExtensionOmiAudio> allUniqueAudioEmitter;
        public static List<AudioClip> allUniqueAudioClips;

        //vars
        public int index = -1;      //the index of the audio emitter
        public List<ObjectProperty> emitterProperties;
        

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueAudioEmitter = new List<ObjectExtensionOmiAudio>();
            allUniqueAudioClips = new List<AudioClip>();
        }
        public static int GetAudioEmitterIndex(AudioSource audio_source)
        {
            if (audio_source.clip == null)
                return -1;

            foreach(ObjectExtensionOmiAudio emitter in allUniqueAudioEmitter)
            {
                if (isSameAudioEmitter(audio_source))
                {
                    return emitter.index;
                }
            }
            allUniqueAudioEmitter.Add( new ObjectExtensionOmiAudio(audio_source));
            return allUniqueAudioEmitter.Count - 1;
            //

        }
        public static int GetAudioEmitterIndex(WebPositionalAudioSource web_audio_source)
        {
            if (web_audio_source.clip == null)
                return -1;

            foreach (ObjectExtensionOmiAudio emitter in allUniqueAudioEmitter)
            {
                if (isSameAudioEmitter(web_audio_source))
                {
                    return emitter.index;
                }
            }
            allUniqueAudioEmitter.Add(new ObjectExtensionOmiAudio(web_audio_source));
            return allUniqueAudioEmitter.Count - 1;
            //

        }
        private static bool isSameAudioEmitter(AudioSource audio_source)
        {
            // FOR NOW WE WILL ALWAYS CREATE NEW AUDIO EMITTERS
            return false;
        }
        private static bool isSameAudioEmitter(WebPositionalAudioSource web_audio_source)
        {
            // FOR NOW WE WILL ALWAYS CREATE NEW AUDIO EMITTERS
            return false;
        }

        public ObjectExtensionOmiAudio(AudioSource audio_source)
        {
            index = globalIndex;

            emitterProperties = GetEmitterProperties(audio_source);

            globalIndex++;
        }

        public ObjectExtensionOmiAudio(WebPositionalAudioSource web_audio_source)
        {
            index = globalIndex;

            emitterProperties = GetEmitterProperties(web_audio_source);

            globalIndex++;
        }

        private List<ObjectProperty> GetEmitterProperties(AudioSource audio_source)
        {
            List<ObjectProperty> props = new List<ObjectProperty>();
            props.Add(new ObjectProperty("name", audio_source.clip.name));
            // is this one required?
            props.Add(new ObjectProperty("type", "global"));
            props.Add(new ObjectProperty("gain", audio_source.volume));
            props.Add(new ObjectProperty("loop", audio_source.loop));
            props.Add(new ObjectProperty("playing", audio_source.playOnAwake));
            props.Add(new ObjectProperty("source", GetAudioClipindex(audio_source.clip)));

            // add pitch in extras (detune in three js)
            // playback rate (speed)
            // offset start


            return props;
        }
        private List<ObjectProperty> GetEmitterProperties(WebPositionalAudioSource web_audio_source)
        {
            //missing check defaults

            List<ObjectProperty> props = new List<ObjectProperty>();
            if (web_audio_source.name != "")
                props.Add(new ObjectProperty("name", web_audio_source.name));
            props.Add(new ObjectProperty("source", GetAudioClipindex(web_audio_source.clip)));
            
            bool isGlobalAudio = web_audio_source.isGlobalAudio;
            // is this one required?
            props.Add(new ObjectProperty("type", isGlobalAudio == true ? "global":"positional"));
            props.Add(new ObjectProperty("gain", web_audio_source.gain));
            // props.Add(new ObjectProperty("speed", web_audio_source.speed));
            // props.Add(new ObjectProperty("pitch", web_audio_source.pitch));
            // offset start property missing
            props.Add(new ObjectProperty("loop", web_audio_source.loop));
            props.Add(new ObjectProperty("playing", web_audio_source.playing));
           

            if (!isGlobalAudio)
            {
                List<ObjectProperty> _positional = new List<ObjectProperty>();

                _positional.Add(new ObjectProperty("coneInnerAngle", Mathf.Deg2Rad * web_audio_source.coneInnerAngleDegree));
                _positional.Add(new ObjectProperty("coneOuterAngle", Mathf.Deg2Rad * web_audio_source.coneOuterAngleDegree));
                _positional.Add(new ObjectProperty("coneOuterGain", web_audio_source.coneOuterGain));
                _positional.Add(new ObjectProperty("distanceModel", web_audio_source.distance.ToString()));

                _positional.Add(new ObjectProperty("refDistance", web_audio_source.refDistance));
                if (web_audio_source.distance == WebPositionalAudioSource.Distance.linear)
                {
                    _positional.Add(new ObjectProperty("maxDistance", web_audio_source.maxDistance));
                    _positional.Add(new ObjectProperty("rolloffFactor", web_audio_source.rollOffFactorLinear));
                }
                else
                {
                    _positional.Add(new ObjectProperty("rolloffFactor", web_audio_source.rollOffFactor));
                }

                if (_positional.Count > 0)
                {
                    props.Add(new ObjectProperty("positional", _positional));
                }
            }

            

            return props;
        }


        private static int GetAudioClipindex(AudioClip audio_clip)
        {
            foreach (AudioClip clip in allUniqueAudioClips)
            {
                if (audio_clip == clip)
                {
                    return allUniqueAudioClips.IndexOf(clip);
                }
            }
            // IF NO COINCIDENCE WAS FOUND ADD THE NEW AUDIO TO THE LIST AND RETURN THE INDEX OF IT
            allUniqueAudioClips.Add(audio_clip);
            return allUniqueAudioClips.Count - 1;
        }

        public static void AddGLTFDataToExtensions()
        {
            if (allUniqueAudioEmitter.Count > 0)
            {
                List<ObjectProperty> _KHR_audio = new List<ObjectProperty>();

                List<ObjectProperty> _emitters = new List<ObjectProperty>();
                List<ObjectProperty> _sources = new List<ObjectProperty>();

                foreach (ObjectExtensionOmiAudio ae in allUniqueAudioEmitter)
                {
                    _emitters.Add(new ObjectProperty("", ae.emitterProperties));
                }
                foreach (AudioClip clip in allUniqueAudioClips)
                {
                    List<ObjectProperty> _sources_single = new List<ObjectProperty>();
                    _sources_single.Add(new ObjectProperty("name", clip.name));
                    _sources_single.Add(new ObjectProperty("uri", "aud_"+ allUniqueAudioClips.IndexOf(clip) + "." +StringUtilities.GetExtensionFromObjectAsset(clip)));

                    _sources.Add(new ObjectProperty("", _sources_single));
                }

                _KHR_audio.Add(new ObjectProperty("sources", _sources, true));
                _KHR_audio.Add(new ObjectProperty("emitters", _emitters,true));

                ObjectExtension.Add(new ObjectProperty("KHR_audio", _KHR_audio));
            }
            
        }
        public static void ExportAllAudios(string location)
        {
            for (int i = 0; i < allUniqueAudioClips.Count; i++)
            {
                FileExporter.CopyFileFromAsset(allUniqueAudioClips[i],location,exportPreName + i);
            }
        }

    }
}
