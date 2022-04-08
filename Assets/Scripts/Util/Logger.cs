using System.Diagnostics;
using UnityEngine;

namespace Util
{
    public static class Debug
    {
        [Conditional("ENABLE_LOG")]
        public static void Log(GameObject go, string s)
        {
            UnityEngine.Debug.Log(go.name + " " + go.transform.GetInstanceID() + " : " + s);
        }

        [Conditional("ENABLE_LOG")]
        public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color, float time)
        {
            Debug.DrawLine(start, end, color, time);
        }

        public static void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float t)
        {
            // create matrix
            Matrix4x4 m = new Matrix4x4();
            m.SetTRS(pos, rot, scale);

            var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
            var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
            var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
            var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

            var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
            var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
            var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
            var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

            UnityEngine.Debug.DrawLine(point1, point2, c, t);
            UnityEngine.Debug.DrawLine(point2, point3, c, t);
            UnityEngine.Debug.DrawLine(point3, point4, c, t);
            UnityEngine.Debug.DrawLine(point4, point1, c, t);

            UnityEngine.Debug.DrawLine(point5, point6, c, t);
            UnityEngine.Debug.DrawLine(point6, point7, c, t);
            UnityEngine.Debug.DrawLine(point7, point8, c, t);
            UnityEngine.Debug.DrawLine(point8, point5, c, t);

            UnityEngine.Debug.DrawLine(point1, point5, c, t);
            UnityEngine.Debug.DrawLine(point2, point6, c, t);
            UnityEngine.Debug.DrawLine(point3, point7, c, t);
            UnityEngine.Debug.DrawLine(point4, point8, c, t);

        }

    }
}
