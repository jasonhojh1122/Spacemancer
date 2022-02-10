
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Util
{
    public class Ray
    {
        public static IEnumerable<RaycastHit> OrderedHits(Vector3 origin, Vector3 direction)
        {
            RaycastHit[] hits = Physics.RaycastAll(origin, direction);
            IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);
            return orderedHits;
        }

    }
}