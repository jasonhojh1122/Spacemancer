using UnityEngine;
using System.Collections.Generic;

namespace Splittable
{
    public class SplittableGroup : SplittableObject
    {
        [SerializeField] List<Core.ObjectColor> group;

        private void Start()
        {
            UpdateGroupDimension();
        }

        new public void Split()
        {
            base.Split();
            UpdateGroupDimension();
        }

        new public void Merge(SplittableObject parent)
        {
            base.Merge(parent);
            UpdateGroupDimension();
        }
        public void UpdateGroupDimension()
        {
            Debug.Log("Update");
            foreach (var oc in group)
                oc.DimensionColor = dimension.color;
        }


    }
}
