
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

using Core;

namespace Splittable
{
    public class ErrorSpace : SplittableObject
    {
        public bool splitted;
        [SerializeField] VisualEffect vf;
        bool playing;

        new void Awake() {
            base.Awake();
            splitted = false;
            playing = true;
            ReEnableVF();
        }

        public override void Split()
        {
            if (!splitted)
            {
                splitted = true;
                for (int i = 0; i < World.Instance.Dimensions.Count; i++)
                    if (i != World.Instance.DimId[dimension.color])
                        World.Instance.InstantiateNewObjectToDimension(this, i);
            }
            ReEnableVF();
            World.Instance.MoveToProcessed(this);
        }

        public override void Merge(SplittableObject parent)
        {
            IsMerged = true;
            World.Instance.MoveToProcessed(this);
            ReEnableVF();
        }

        public void Fix()
        {
            vf.Stop();
            col.enabled = false;
            playing = false;
        }

        void ReEnableVF()
        {
            if (playing) return;
            vf.Play();
            col.enabled = true;
            playing = true;
            Debug.Log("Play ve");
        }
    }
}
