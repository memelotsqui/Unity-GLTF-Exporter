using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class ImageUtilities
    {
        public static float GetImageMaxPixelFloatValue(Texture2D target_texture, float max_possible_value = 20f,bool is_lightmap = true)
        {
            if (is_lightmap)
                ImageGenerator.MakeTextureTypeLightmap(target_texture);
            Color[] allColors = target_texture.GetPixels();
            float maxVal = 0;
            for (int i = 0; i < allColors.Length; i++)
            {
                if (allColors[i].r > maxVal)
                    maxVal = allColors[i].r;
                if (allColors[i].g > maxVal)
                    maxVal = allColors[i].g;
                if (allColors[i].b > maxVal)
                    maxVal = allColors[i].b;
            }
            if (maxVal > max_possible_value)
                return max_possible_value;
            return maxVal;
        }
    }
}
