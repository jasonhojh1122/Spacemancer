
using UnityEngine;
using Core;

namespace Splittable
{
    public class VoidSplittable : SplittableObject
    {

        public override void Split()
        {
            MoveToDimension();
            Core.World.Instance.MoveToProcessed(this);
        }

        public override void Merge(SplittableObject parent)
        {
            MoveToDimension();
            IsMerged = true;
            Core.World.Instance.MoveToProcessed(this);
        }

        void MoveToDimension()
        {
            if (World.Instance.DimId[Color] >= 0)
                World.Instance.MoveObjectToDimension(this, Color);
            else
                World.Instance.MoveObjectToDimension(this, Dimension.Color.BLACK);
        }

    }
}
