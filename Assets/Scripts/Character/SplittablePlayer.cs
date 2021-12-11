using UnityEngine;
using System.Collections.Generic;

using Core;
namespace Character
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(KinematicObject))]
    public class SplittablePlayer : SplittableObject
    {
        [SerializeField] PlayerDummy dummyPrefab;
        Dictionary<Dimension.Color, PlayerDummy> dummies;

        void Start()
        {
            dummies = new Dictionary<Dimension.Color, PlayerDummy>();
            foreach (Dimension.Color bc in Dimension.BaseColor)
            {
                dummies[bc] = GameObject.Instantiate<PlayerDummy>(dummyPrefab);
                World.Instance.MoveObjectToDimension(dummies[bc].gameObject, bc);
                dummies[bc].gameObject.SetActive(false);
            }
        }

        public void UpdateDummyTransform()
        {
            foreach (Dimension.Color bc in Dimension.BaseColor)
            {
                if (!dummies[bc].gameObject.activeSelf) continue;
                dummies[bc].transform.localPosition = transform.localPosition;
                dummies[bc].transform.localRotation = transform.localRotation;
            }
        }

        public void SetDummyAnimatorController(RuntimeAnimatorController controller)
        {
            foreach (Dimension.Color bc in Dimension.BaseColor)
            {
                if (!dummies[bc].gameObject.activeSelf) continue;
                dummies[bc].SetAnimatorController(controller);
            }
        }

        public void SetDummyAnimatorSpeed(float speed)
        {
            foreach (Dimension.Color bc in Dimension.BaseColor)
            {
                dummies[bc].SetAnimatorSpeed(speed);
            }
        }

        public override void Split()
        {
            MoveToActiveDimension(true);
            World.Instance.MoveToProcessed(this);
        }

        public override void Merge(SplittableObject parent)
        {
            MoveToActiveDimension(false);
            World.Instance.MoveToProcessed(this);
        }

        void MoveToActiveDimension(bool enableDummies)
        {
            World.Instance.MoveObjectToDimension(this, World.Instance.ActiveDimension.Color);
            foreach (Dimension.Color sc in Dimension.BaseColor)
            {
                if (World.Instance.Splitted && enableDummies && sc != World.Instance.ActiveDimension.Color)
                {
                    dummies[sc].gameObject.SetActive(true);
                }
                else
                {
                    dummies[sc].gameObject.SetActive(false);
                }
            }
        }

        private void FixedUpdate()
        {
            if (World.Instance.Transitting) return;
            MoveToActiveDimension(World.Instance.Splitted);
        }
    }

}