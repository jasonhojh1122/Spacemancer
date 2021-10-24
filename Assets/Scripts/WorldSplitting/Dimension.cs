
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Dimension {

    [Flags] public enum Color {
        BLUE    = 0b_0001,
        GREEN   = 0b_0010,
        RED     = 0b_0100,
        CYAN    = GREEN | BLUE,
        MAGENTA = RED | BLUE,
        YELLOW  = RED | GREEN,
        WHITE   = RED | GREEN | BLUE,
        BLACK   = 0b_1000,
        NONE    = 0b_0000
    }

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

    [SerializeField] GameObject target;
    public Color color;

    public Vector3 position {
        get => target.transform.position;
    }
    public Quaternion rotation {
        get => target.transform.rotation;
    }

}
