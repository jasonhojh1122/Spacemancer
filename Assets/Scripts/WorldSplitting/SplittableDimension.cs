
using UnityEngine;

namespace Core
{
    public class SplittableDimension : SplittableObject
    {

        public override void Split()
        {
            world.RemoveFromSet(this);
        }

        public override void Merge(SplittableObject parent)
        {
            world.RemoveFromSet(this);
        }

    }
}
