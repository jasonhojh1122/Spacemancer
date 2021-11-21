
using System;
using System.Collections.Generic;

namespace Core {

    public class Dimension {

        [Flags] public enum Color
        {
            NONE    = 0,
            BLUE    = 1 << 0,
            GREEN   = 1 << 1,
            RED     = 1 << 2,
            CYAN    = GREEN | BLUE,
            MAGENTA = RED | BLUE,
            YELLOW  = RED | GREEN,
            WHITE   = RED | GREEN | BLUE,
            BLACK   = 0b_1000,
        }

        [Serializable]
        public struct ColorSetting
        {
            public Color colorTag;
            public UnityEngine.Color32 color32;
        }

        public static List<Color> BaseColor = new List<Color> {Color.RED, Color.GREEN, Color.BLUE};

        public static Dictionary<Dimension.Color, UnityEngine.Color> MaterialColor = new Dictionary<Dimension.Color, UnityEngine.Color> {
            //{Color.RED,     new UnityEngine.Color(0.588f, 0.196f, 0.196f, 1.0f)}, // #963232
            //{Color.GREEN,   new UnityEngine.Color(0.196f, 0.588f, 0.196f, 1.0f)}, // #329632
            //{Color.BLUE,    new UnityEngine.Color(0.196f, 0.196f, 0.588f, 1.0f)}, // #323296
            //{Color.CYAN,    new UnityEngine.Color(0.196f, 0.588f, 0.588f, 1.0f)}, // #329696
            //{Color.MAGENTA, new UnityEngine.Color(0.588f, 0.196f, 0.588f, 1.0f)}, // #963296
            //{Color.YELLOW,  new UnityEngine.Color(0.588f, 0.588f, 0.196f, 1.0f)}, // #969632
            //{Color.WHITE,   new UnityEngine.Color(0.882f, 0.882f, 0.882f, 1.0f)}, // #E1E1E1
            //{Color.BLACK,   new UnityEngine.Color(0.235f, 0.235f, 0.235f, 1.0f)}, // #3C3C3C
        };

        public static Color AddColor(Color col1, Color col2)
        {
            if ((col1 & col2) > 0)
            {
                return Color.BLACK;
            }
            Color newColor = col1 | col2;
            if (newColor > Color.WHITE)
            {
                newColor = Color.BLACK;
            }
            return newColor;
        }

        public static List<Color> SplitColor(Color color)
        {
            var ret = new List<Color>();
            if (color == Color.BLACK)
            {
                for (int i = 0; i < BaseColor.Count; i++)
                    ret.Add(Color.BLACK);
                return ret;
            }
            foreach (Color bc in BaseColor)
            {
                if ((bc & color) > 0)
                {
                    ret.Add(bc);
                }
            }
            return ret;
        }

        public static List<Color> MissingColor(Color color)
        {
            var ret = new List<Color>();
            if (color != Color.BLACK)
            {
                foreach (Color bc in BaseColor)
                {
                    if ((bc & color) == 0)
                    {
                        ret.Add(bc);
                    }
                }
            }
            return ret;
        }

    }

}