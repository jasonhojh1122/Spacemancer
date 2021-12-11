
using UnityEngine;

namespace Core
{
    public class SplittableDimension : SplittableObject
    {

        public override void Split()
        {
            World.Instance.RemoveFromSet(this);
        }

        public override void Merge(SplittableObject parent)
        {
            World.Instance.RemoveFromSet(this);
        }

    }
}
