
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Dimension : MonoBehaviour {

    [Flags] public enum Color {
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

    public static List<Color> BaseColor = new List<Color> {Color.RED, Color.GREEN, Color.BLUE};

    public static Color AddColor(Color col1, Color col2) {
        if ((col1 & col2) > 0) {
            return Color.BLACK;
        }
        Color newColor = col1 | col2;
        if (newColor > Color.WHITE) {
            newColor = Color.BLACK;
        }
        return newColor;
    }

    public static List<Color> SplitColor(Color color) {
        var ret = new List<Color>();
        if (color == Color.BLACK) {
            for (int i = 0; i < BaseColor.Count; i++)
                ret.Add(Color.BLACK);
            return ret;
        }
        foreach (Color bc in BaseColor) {
            if ((bc & color) > 0) {
                ret.Add(bc);
            }
        }
        return ret;
    }

    public static List<Color> MissingColor(Color color) {
        var ret = new List<Color>();
        if (color != Color.BLACK) {
            foreach (Color bc in BaseColor) {
                if ((bc & color) == 0) {
                    ret.Add(bc);
                }
            }
        }
        return ret;
    }

    [SerializeField] GameObject target;
    [SerializeField] Color color;

    public Vector3 targetPosition {
        get => target.transform.position;
    }
    public Quaternion targetRotation {
        get => target.transform.rotation;
    }
    public Color GetColor() {
        return color;
    }

}
