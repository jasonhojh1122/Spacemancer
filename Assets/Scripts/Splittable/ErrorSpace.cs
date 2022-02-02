
using UnityEngine;
using System.Collections.Generic;

using Core;

namespace Splittable
{
    public class ErrorSpace : SplittableObject
    {
        public bool splitted;
        [SerializeField] GameObject particle;

        new void Awake() {
            base.Awake();
            splitted = false;
        }

        public override void Split()
        {
            if (!splitted)
            {
                splitted = true;
                for (int i = 0; i < World.Instance.Dimensions.Count; i++)
                    if (i != World.Instance.DimId[dimension.Color])
                        World.Instance.InstantiateNewObjectToDimension(this, i);
            }
            particle.SetActive(true);
            World.Instance.MoveToProcessed(this);
        }

        public override void Merge(SplittableObject parent)
        {
            IsMerged = true;
            World.Instance.MoveToProcessed(this);
            return;
        }

        public void Fix()
        {
            particle.SetActive(false);
        }
    }
}
