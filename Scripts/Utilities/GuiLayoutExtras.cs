using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WEBGL_EXPORTER
{
    public class GuiLayoutExtras
    {
        public static GUIStyle buttonGuiStyle;
        private static float currentHorizontalMargin = 0;
        private static int buttonsQty = 1;
        public static void CenterLabel(string label, int height)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("", GUILayout.Height(height / 2 - 7));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, GUILayout.Height(height / 2 + 7));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static float GetCenteredButtonWidth() 
        {
            return ((Screen.width * GetDPI()) / buttonsQty) - ((currentHorizontalMargin*2* GetDPI()) / buttonsQty);
        }
        public static float GetCenteredLabelWidth(float margin = 0f)
        {
            return (Screen.width * GetDPI()) - margin;
        }
        public static float RealDPIFloatSize(float value)
        {
            return value * GetDPI();
        }
        public static float GetRealScreenWidth()
        {
            return (Screen.width * GetDPI());
        }
        public static float GetRealScreenHeight()
        {
            return (Screen.height * GetDPI());
        }
        private static float GetDPI()
        {
            float dpi = Screen.dpi;
            if (dpi == 0)
                dpi = 96;
            dpi = 96 / dpi;
            return dpi;
        }
        public static void BeginAlignedHorizontal(float margin, int buttons_qty)
        {
            GUILayout.BeginHorizontal();
            buttonsQty = buttons_qty;
            GUILayout.Box("", GUIStyle.none, GUILayout.Width((margin/1.4f)*GetDPI()));
            currentHorizontalMargin = margin;
        }
        public static void EndAlignedHorizontal()
        {
            //GUILayout.Box("", GUIStyle.none, GUILayout.Width((currentHorizontalMargin) * GetDPI()));
            currentHorizontalMargin = 0;
            buttonsQty = 1;
            GUILayout.EndHorizontal();
        }
/*        public static void BeginCenteredButtons()
        {
            GUILayout.BeginArea(new Rect(0f, 100f, 100f, 80f));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        }
        public static void EndCenteredButtons()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }*/
    }
}
