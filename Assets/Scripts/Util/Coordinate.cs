
using UnityEngine;

namespace Util
{
    public class Coordinate
    {
        public static Vector3 RelativeLocalPos(Transform obj, Transform reference)
        {
            return reference.InverseTransformPoint(obj.position);
        }

        public static Vector3 RelativeLocalPos(Splittable.SplittableObject obj, Core.ObjectColor reference)
        {
            return reference.transform.InverseTransformPoint(obj.transform.position);
        }

        /// <summary>
        /// Calculates the lattice point of the given coordinate.
        /// </summary>
        /// <param name="pos"> The coordinate to be calculated. </param>
        /// <returns> The lattice point. </returns>
        public static Vector3 Lattice(Vector3 pos)
        {
            var c = Mathf.Ceil(pos.x);
            var f = Mathf.Floor(pos.x);
            var x = (c - pos.x) < (pos.x - f) ? c : f;
            c = Mathf.Ceil(pos.y);
            f = Mathf.Floor(pos.y);
            var y = (c - pos.y) < (pos.y - f) ? c : f;
            c = Mathf.Ceil(pos.z);
            f = Mathf.Floor(pos.z);
            var z = (c - pos.z) < (pos.z - f) ? c : f;
            return new Vector3(x, y, z);
        }

    }
}

