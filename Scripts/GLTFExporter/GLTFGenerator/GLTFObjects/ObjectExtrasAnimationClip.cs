using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtrasAnimationClip
    {
        //statics
        public static int globalIndex = -1;
        public static List<ObjectExtrasAnimationClip> allUniqueAnimationClips;

        //vars
        public int index = -1;

        public AnimationClip animationClip;
        public List<ObjectProperty> animationProperties;

        public List<string> trackNames;
        public List<ObjectAccessors> keyTimes;
        public List<ObjectAccessors> keyValues;
        public List<ObjectAccessors> keyWeights;
        public List<ObjectAccessors> keyTangents;
        public static void Reset()
        {
            globalIndex = 0;
            allUniqueAnimationClips = new List<ObjectExtrasAnimationClip>();
        }

        public static int GetAnimationIndex(AnimationClip animation_clip)
        {
            if (animation_clip == null)
                return -1;
            
            if (allUniqueAnimationClips == null)
            {
                allUniqueAnimationClips = new List<ObjectExtrasAnimationClip>();

                allUniqueAnimationClips.Add(new ObjectExtrasAnimationClip(animation_clip));

                return allUniqueAnimationClips.Count - 1;
            }
            else
            {
                foreach (ObjectExtrasAnimationClip oa in allUniqueAnimationClips)
                {
                    if (oa.isSameAnimationClip(animation_clip))
                    {
                        return oa.index;
                    }
                }       
                // IF NO EXISTING ANIMATION CLIP WAS FOUND CREATE A NEW ANIMATION CLIP

                allUniqueAnimationClips.Add(new ObjectExtrasAnimationClip(animation_clip));
                return allUniqueAnimationClips.Count - 1;
            }
        }
        private bool isSameAnimationClip(AnimationClip animation_clip)
        {
            if (animation_clip == animationClip)
                return true;
            return false;
        }

        public ObjectExtrasAnimationClip(AnimationClip animation_clip)
        {
            animationClip = animation_clip;
            index = globalIndex;

            animationProperties = new List<ObjectProperty>();//get data at the end
            trackNames = new List<string>();
            keyTimes = new List<ObjectAccessors>();
            keyValues = new List<ObjectAccessors>();
            keyWeights= new List<ObjectAccessors>();
            keyTangents = new List<ObjectAccessors>();

            float duration = animationClip.averageDuration;

            EditorCurveBinding[] animation_curves = AnimationUtility.GetCurveBindings(animation_clip);

            for (int i = 0; i < animation_curves.Length; i++)
            {

                EditorCurveBinding curve = animation_curves[i];

                string trackName = GetTrackName(curve);
                if (trackName != "")
                {
                    float mult = 1;
                    if (trackName.Contains(".rotation"))
                    {
                        mult = Mathf.Deg2Rad;
                        if (!trackName.Contains("[z]"))
                            mult *= -1;
                    }
                    if (trackName.Contains(".position[z]"))
                    {
                        mult *= -1;
                    }



                        Keyframe[] keys = AnimationUtility.GetEditorCurve(animation_clip, curve).keys;


                    int lastItemArray = 0;
                    int firstItemArray = 0;


                    // IF LAST KEYFRAME IS NOT ATE THE END OF THE OVERALL ANIMATION DURATION, ADD A KEYFRAME
                    if (keys[keys.Length - 1].time < duration)
                        lastItemArray = 1;

                    // IF FIRST KEY IS NOT IN TIME 0, ADD A KEYFRAME AT VALUE 0
                    if (keys[0].time != 0f)
                        firstItemArray = 1;



                    float[] key_times = new float[keys.Length + lastItemArray + firstItemArray];
                    float[] key_values = new float[keys.Length + lastItemArray + firstItemArray];
                    Vector2[] key_weight = new Vector2[keys.Length + lastItemArray + firstItemArray];
                    Vector2[] key_tangents = new Vector2[keys.Length + lastItemArray + firstItemArray];

                    for (int j = 0; j < keys.Length; j++)
                    {
                        
                        key_times[j + firstItemArray] = keys[j].time;
                        key_values[j + firstItemArray] = keys[j].value * mult;        // VALUES IN ROTATION MUST BE IN RAD
                        key_weight[j + firstItemArray] = new Vector2(keys[j].inWeight , keys[j].outWeight);             
                        key_tangents[j + firstItemArray] = new Vector2(keys[j].inTangent > 1000 ? 0: keys[j].inTangent * mult       // VALIDATE IS NOT INFINITY
                                                    , keys[j].outTangent > 1000 ? 0 : keys[j].outTangent * mult);   // VALIDATE IS NOT INFINITY
                    }

                    if (firstItemArray > 0)
                    {
                        key_times[0] = 0f;
                        key_values[0] = key_values[1];
                        key_weight[0] = new Vector2(0.3f, 0.3f);
                        key_tangents[0] = new Vector2(key_values[1], key_values[1]);
                    }

                    if (lastItemArray > 0)
                    {
                        key_times[key_times.Length - 1] = duration;
                        key_values[key_values.Length - 1] = key_values[key_values.Length - 2];
                        
                        key_weight[key_weight.Length - 1] = new Vector2(0.3f,0.3f);
                        key_tangents[key_tangents.Length - 1] = new Vector2(key_values[key_values.Length - 1], key_values[key_values.Length - 1]);

                    }

                    trackNames.Add(trackName);
                    
                    keyTimes.Add(new ObjectAccessors(key_times));
                    keyValues.Add(new ObjectAccessors(key_values));
                    keyWeights.Add(new ObjectAccessors(key_weight));
                    keyTangents.Add(new ObjectAccessors(key_tangents));
                }

            }
            index = globalIndex;
            globalIndex++;
        }
        private static string GetTrackName(EditorCurveBinding curve)
        {
            string result = "";
            bool saved = false;
            if (curve.type == typeof(Transform))
            {
                saved = true;
                result += curve.path;
                if (curve.propertyName.Contains("Position"))
                    result += ".position[" + curve.propertyName.Split('.')[1] + "]";
                if (curve.propertyName.Contains("Scale"))
                    result += ".scale[" + curve.propertyName.Split('.')[1] + "]";
                if (curve.propertyName.Contains("Euler"))
                    result += ".rotation[" + curve.propertyName.Split('.')[1] + "]";
            }
            if (!saved && curve.type == typeof(Material))
            {
                saved = true;
                result += curve.path;
                string[] matProps = curve.propertyName.Split('.');
                result += ".material." + matProps[0];
                if (matProps.Length > 0)
                    result += "[" + matProps[1] + "]";

            }
            if (!saved)
            {

            }

            return result;
        }
        public static void AddGLTFDataToExtras()
        {
            if (allUniqueAnimationClips.Count > 0)
            {
                if (ExportToGLTF.options.exportAnimations)
                {
                    List<ObjectProperty> _animationClips = new List<ObjectProperty>();
                    
                    foreach (ObjectExtrasAnimationClip oa in allUniqueAnimationClips)
                    {
                        List<ObjectProperty> _animationClip_single = new List<ObjectProperty>();
                        _animationClip_single.Add(new ObjectProperty("name", oa.animationClip.name));
                        _animationClip_single.Add(new ObjectProperty("loop", oa.animationClip.isLooping));


                        List<ObjectProperty> _keyframeTracks = new List<ObjectProperty>();
                        for (int i = 0; i < oa.keyTimes.Count; i++) {
                            List<ObjectProperty> _keyframeTracks_single = new List<ObjectProperty>();
                            _keyframeTracks_single.Add(new ObjectProperty("trackName", oa.trackNames[i]));
                            _keyframeTracks_single.Add(new ObjectProperty("TIME", oa.keyTimes[i].export_index));
                            _keyframeTracks_single.Add(new ObjectProperty("VALUE", oa.keyValues[i].export_index));
                            _keyframeTracks_single.Add(new ObjectProperty("WEIGHT", oa.keyWeights[i].export_index));
                            _keyframeTracks_single.Add(new ObjectProperty("TANGENT", oa.keyTangents[i].export_index));
                            _keyframeTracks.Add(new ObjectProperty("", _keyframeTracks_single));
                        }
                        _animationClip_single.Add(new ObjectProperty("keyframeTracks", _keyframeTracks,true));
                        _animationClips.Add(new ObjectProperty("", _animationClip_single));
                    }
                    ObjectMasterExtras.Add(new ObjectProperty("animationClips", _animationClips,true));
                }
            }
        }
        
    }
}





