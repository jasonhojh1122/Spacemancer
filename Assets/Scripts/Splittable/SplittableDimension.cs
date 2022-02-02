
using UnityEngine;

namespace Splittable
{
    public class SplittableDimension : SplittableObject
    {

        public override void Split()
        {
            Core.World.Instance.RemoveFromSet(this);
        }

        public override void Merge(SplittableObject parent)
        {
            Core.World.Instance.RemoveFromSet(this);
        }

    }
}
