using UnityEngine;

namespace Util
{
    public static class Fuzzy
    {
        public static float amount = 0.01f;
        public static Vector3 amountVec3 = new Vector3(amount, amount, amount);

        public static bool CloseVector3(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude < amount;
        }

        public static bool CloseVector3(Vector3 a, float x, float y, float z) {
            return (a - new Vector3(x, y, z)).magnitude < amount;
        }

        public static bool CloseQuaternion(Quaternion a, Quaternion b)
        {
            return Quaternion.Angle(a, b) < amount;
        }

        public static bool CloseFloat(float a, float b)
        {
            return Mathf.Abs(a-b) < amount;
        }

    }

}