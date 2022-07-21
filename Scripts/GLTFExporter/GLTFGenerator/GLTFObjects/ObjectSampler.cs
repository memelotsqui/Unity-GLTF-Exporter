using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectSampler
    {
        //statics 
        public static int globalIndex = -1;
        public static List<ObjectSampler> allUniqueSamplers;

        //vars
        public int index = -1;

        public List<ObjectProperty> samplerProperties;

        public TextureWrapMode _wrapModeU;
        public TextureWrapMode _wrapModeV;

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueSamplers = new List<ObjectSampler>();
        }

        public static int GeSamplerIndex(Texture _texture)
        {
            if (allUniqueSamplers == null)
            {
                allUniqueSamplers = new List<ObjectSampler>();
                allUniqueSamplers.Add(new ObjectSampler(_texture));

                return allUniqueSamplers.Count - 1;
            }
            else
            {
                foreach (ObjectSampler os in allUniqueSamplers)
                {
                    if (os.isSameSampler(_texture))
                    {
                        return os.index;
                    }
                }
                allUniqueSamplers.Add(new ObjectSampler(_texture));
                return allUniqueSamplers.Count - 1;
            }
        }
        public bool isSameSampler(Texture _texture)
        {
            //missing filter mode
            TextureWrapMode wrap_u = _texture.wrapModeU;
            TextureWrapMode wrap_v = _texture.wrapModeV;

            if (wrap_u == TextureWrapMode.MirrorOnce)
                wrap_u = TextureWrapMode.Mirror;
            if (wrap_v == TextureWrapMode.MirrorOnce)
                wrap_v = TextureWrapMode.Mirror;

            if (wrap_u == _wrapModeU && wrap_v == _wrapModeV)
            {
                return true;
            }

            return false;
        }

        public ObjectSampler(Texture _texture)
        {
            _wrapModeU = _texture.wrapModeU;
            if (_wrapModeU == TextureWrapMode.MirrorOnce)
                _wrapModeU = TextureWrapMode.Mirror;

            _wrapModeV = _texture.wrapModeV;
            if (_wrapModeV == TextureWrapMode.MirrorOnce)
                _wrapModeV = TextureWrapMode.Mirror;

            index = globalIndex;
            samplerProperties = GetSampleProperties(_wrapModeU, _wrapModeV);

            globalIndex++;
        }



        //NOTES:
        //WE WILL BE USING DEFAULT VALUES FOR MAG FILTER AND MIN FILTER, THE ONE THAT MAY CHANGE IS  WRAPS WRAPT

        //MAG FILTER: POSSIBLE VALUES:
        //9728 - "NEAREST", 9279 - "LINEAR"(DEFAULT)

        //MIN FILTER
        //9728 - "NEAREST", 9279 - "LINEAR", 9984 - "NEAREST_MIPMAP_NEAREST"
        //9986 - 9985 - "LINEAR_MIPMAP_NEAREST", 9986 - "NEAREST_MIPMAP_LINEAR" (default), 9987 - "LINEAR_MIPMAP_LINEAR"

        //WRAPS / WRAPT
        //33071 - "CLAMP_TO_EDGE", 33648 - "MIRRORED_REPEAT", 10497 - "REPEAT" (default)
        private List<ObjectProperty> GetSampleProperties(TextureWrapMode wrap_u, TextureWrapMode wrap_v)
        {
            List<ObjectProperty> sampler_Properties = new List<ObjectProperty>();
            //FOR NOW WE WILL SET DEFAULT VALUES, THE SAME DEFAULTS AS GLOTF FORMAT
            int _magFilter = 9729;          // LINEAR
            int _minFilter = 9986;          // NEAREST_MIPMAP_LINEAR
            int _wrapS = GetWrapModeInt(wrap_u);
            int _wrapT = GetWrapModeInt(wrap_v);

            sampler_Properties.Add(new ObjectProperty("magFilter", _magFilter)); 
            sampler_Properties.Add(new ObjectProperty("minFilter", _minFilter));
            sampler_Properties.Add(new ObjectProperty("wrapS",_wrapS));
            sampler_Properties.Add(new ObjectProperty("wrapT", _wrapT));

            return sampler_Properties;
        }

        private int GetWrapModeInt(TextureWrapMode wrap)
        {
            switch (wrap)
            {
                case TextureWrapMode.Repeat:
                    return 10497;
                case TextureWrapMode.Mirror:
                    return 33648;
                case TextureWrapMode.Clamp:
                    return 33071;
            }
            return 10497;
        }

        public static string GetGLTFData(bool add_end_comma = true)
        {
            string result = "";
            if (allUniqueSamplers.Count > 0)
            {
                result += "\"samplers\" : [\n";
                for (int i = 0; i < allUniqueSamplers.Count; i++)
                {
                    result += ObjectProperty.GetObjectProperties(allUniqueSamplers[i].samplerProperties);
                    if (i < allUniqueSamplers.Count - 1)
                        result += ",\n";
                }
                result += "]";
                if (add_end_comma)
                    result += ",\n";
            }
            return result;
        }
    }
}
