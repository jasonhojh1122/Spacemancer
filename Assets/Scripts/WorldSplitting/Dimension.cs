
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core {

    public class Dimension : MonoBehaviour
    {

        /// <summary>
        /// The bit flag of object's color.
        /// </summary>
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

        /// <summary>
        /// The base color of dimensions.
        /// </summary>
        public static List<Color> BaseColor = new List<Color> {Color.RED, Color.GREEN, Color.BLUE};

        /// <summary>
        /// A list of all the valid color.
        /// </summary>
        public static List<Color> SplittedColor = new List<Color>
        {
            Color.RED, Color.GREEN, Color.BLUE, Color.YELLOW, Color.CYAN, Color.MAGENTA
        };

        /// <summary>
        /// A list of all the valid color.
        /// </summary>
        public static List<Color> ValidColor = new List<Color>
        {
            Color.RED, Color.GREEN, Color.BLUE, Color.YELLOW, Color.CYAN, Color.MAGENTA, Color.WHITE
        };

        /// <summary>
        /// A dictionary that maps the splitted color to the index that it locates in <c>SplittedColor</c>
        /// </summary>
        /// <value></value>
        public static Dictionary<Dimension.Color, int> ValidColorIndex = new Dictionary<Dimension.Color, int>
        {
            {Color.RED, 0}, {Color.GREEN, 1}, {Color.BLUE, 2}, {Color.YELLOW, 3}, {Color.CYAN, 4}, {Color.MAGENTA, 5}, {Color.WHITE, 6}
        };

        /// <summary>
        /// A <c>Dictionary</c> mapping from <c>Dimension.Color</c> to the <c>UnityEngine.Color</c> used
        /// in material.
        /// </summary>
        public static Dictionary<Dimension.Color, UnityEngine.Color> MaterialColor = new Dictionary<Dimension.Color, UnityEngine.Color> {};

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

        public static Color SubColor(Color col1, Color col2)
        {
            return col1 & (~col2);
        }

        /// <summary>
        /// Splits the given color into a list of <c>BaseColor</c>.
        /// </summary>
        /// <param name="color"> The color to be splitted.</param>
        /// <returns> A <c>List</c> of color. </returns>
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

        /// <summary>
        /// Splits the given color and return a list of missing color.
        /// </summary>
        /// <param name="color"> The color to be checked. </param>
        /// <returns> A <c>List</c> of missing color. </returns>
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

        public Color color;

    }

}