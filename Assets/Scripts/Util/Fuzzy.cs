using UnityEngine;

public static class Fuzzy {

    public static bool CloseVector3(Vector3 a, Vector3 b) {
        return (a - b).magnitude < 0.01f;
    }

    public static bool CloseQuaternion(Quaternion a, Quaternion b) {
        return Quaternion.Angle(a, b) < 0.01f;
    }

    public static bool CloseFloat(float a, float b) {
        return Mathf.Abs(a-b) < 0.01f;
    }

}