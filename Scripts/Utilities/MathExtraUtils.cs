using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WEBGL_EXPORTER
{
    public class MathExtraUtils
    {
        public static float ClampRangeValue(float value, float min, float max)
        {

            float midVal = (min + max) / 2;                                 // GET MIDDLE VALUE BETWEEN THE 2 NUMBERS
            float differenceVal = ((midVal - min) + (max - midVal)) / 2;    // VALUE TO BE ADDED OR SUBSTRACTED TO THE MID VALUE, UNTIL NOW WE HAVENT USED "VALUE" PROVIDED BY USER
            float result = ((value - midVal) * differenceVal) / midVal;     // RULE OF 3, TO GET THE RANGE VALUE (FROM -DIFFERENCEVAL TO +DIFFERENCEVAL)
            result += midVal;
            if (result < min)
                result = min;
            if (result > max)
                result = max;
            return result;
        }
        public static float DegreeToRadians(float degree)
        {
            return (degree * Mathf.Deg2Rad);
        }

        public static float RoundFloat(float tarFloat, int decimals)
        {

            float multiplier = Mathf.Pow(10, decimals);
            Debug.Log(Mathf.Round(tarFloat * multiplier) * (1 / multiplier));
            return Mathf.Round(tarFloat * multiplier) * (1/multiplier);
        }
    }
}
