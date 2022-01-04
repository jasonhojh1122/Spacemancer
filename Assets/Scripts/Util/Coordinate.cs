
using UnityEngine;

namespace Util
{
    public class Coordinate
    {
        public static Vector3 RelativeLocalPos(Transform obj, Transform reference)
        {
            return reference.InverseTransformPoint(obj.position);
        }

        public static Vector3 RelativeLocalPos(Core.SplittableObject obj, Core.ObjectColor reference)
        {
            return reference.transform.InverseTransformPoint(obj.transform.position);
        }

    }
}

