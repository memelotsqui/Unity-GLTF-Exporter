using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WEBGL_EXPORTER
{
    public class CubemapExtra
    {

        public Texture2D posx;
        public Texture2D negx;

        public Texture2D posy;
        public Texture2D negy;

        public Texture2D posz;
        public Texture2D negz;


        public string name;
        //public bool reverse = false;

        public CubemapExtra(ReflectionProbe refProbe, string cubemapName = ""/*, bool _reverse = false*/)
        {
            if (refProbe != null)
            {
                Cubemap _cubemap = refProbe.bakedTexture as Cubemap;
                if (_cubemap != null)
                {
                    GetCubemapImages(_cubemap/*, _reverse*/);
                }
                else
                {
                    negx = posx = negy = posy = negz = posz = ImageGenerator.CreateSimpleTexture(new Color(0.2f, 0.2f, 0.2f));
                }

            }
            else
            {
                negx = posx = negy = posy = negz = posz = ImageGenerator.CreateSimpleTexture(new Color(0.2f, 0.2f, 0.2f));
            }
           // reverse = _reverse;
            name = cubemapName;
        }
        public CubemapExtra(Color _cubemap_color, string _cubemapName = "")
        {
            negx = posx = negy = posy = negz = posz = ImageGenerator.CreateSimpleTexture(_cubemap_color);
            name = _cubemapName;
        }
        public CubemapExtra(Cubemap _cubemap, string _cubemapName = ""/*, bool _reverse = false*/)
        {
            //reverse = _reverse;
            name = _cubemapName;
            if (_cubemap != null)
            {
                GetCubemapImages(_cubemap/*, _reverse*/);
            }
            else
            {
                negx = posx = negy = posy = negz = posz = ImageGenerator.CreateSimpleTexture(new Color(0.2f, 0.2f, 0.2f));
            }
        }

        private void GetCubemapImages(Cubemap _cubemap/*, bool _reverse = false*/)
        {
            ImageGenerator.MakeReadableCubemap(_cubemap);
            int textureSize = (int)(Mathf.Sqrt(_cubemap.GetPixels(CubemapFace.NegativeX).Length));

            negx = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeX), textureSize, textureSize, false, true);
            posx = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveX), textureSize, textureSize, false, true);

            negy = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeY), textureSize, textureSize, true, false);
            posy = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveY), textureSize, textureSize, true, false);

            negz = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeZ), textureSize, textureSize, false, true);
            posz = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveZ), textureSize, textureSize, false, true);
        }

        public Texture2D[] GetCubemapTextures()
        {
            Texture2D[] result = new Texture2D[6];
            result[0] = negx;       //negx and posx are inverted in three js
            result[1] = posx;

            result[2] = posy;
            result[3] = negy;

            result[4] = negz;       //negz and posz are inverted in three js
            result[5] = posz;

            return result;
        }

        public static Texture2D[] CreateCubemapTextures(Cubemap _cubemap)
        {
            if (_cubemap != null)
            {
                ImageGenerator.MakeReadableCubemap(_cubemap);
                int textureSize = (int)(Mathf.Sqrt(_cubemap.GetPixels(CubemapFace.NegativeX).Length));

                Texture2D[] result = new Texture2D[6];

                result[0] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeX), textureSize, textureSize, false, true);    //negx in three js
                result[1] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveX), textureSize, textureSize, false, true);    //posx in three js

                result[2] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveY), textureSize, textureSize, false, true);    //posy in three js
                result[3] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeY), textureSize, textureSize, false, true);    //negy in three js

                result[4] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.PositiveZ), textureSize, textureSize, false, true);    //posz in three js
                result[5] = ImageGenerator.GetTextureFromPixels(_cubemap.GetPixels(CubemapFace.NegativeZ), textureSize, textureSize, false, true);    //negz in three js

                return result;
            }
            return null;
        }
    }
}
