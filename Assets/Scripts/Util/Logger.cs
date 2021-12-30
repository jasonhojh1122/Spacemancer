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

    }
}
