using UnityEngine;
using System.Collections.Generic;

using Core;
namespace Character
{
    public class SplittablePlayer : SplittableObject
    {
        [SerializeField] PlayerDummy dummyPrefab;
        List<PlayerDummy> dummies;

        void Awake()
        {
            dummies = new List<PlayerDummy>();
            for (int i = 0; i < World.Instance.Dimensions.Count; i++)
            {
                dummies.Add(GameObject.Instantiate<PlayerDummy>(dummyPrefab));
                dummies[i].transform.localPosition = transform.position;
                dummies[i].transform.localRotation = transform.rotation;
                World.Instance.MoveObjectToDimension(dummies[i].gameObject, i);
                dummies[i].gameObject.SetActive(false);
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

        public void UpdateDummyTransform()
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].transform.localPosition = transform.localPosition;
                dummies[i].transform.localRotation = transform.localRotation;
            }
        }

        public void SetDummyAnimatorController(RuntimeAnimatorController controller)
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].SetAnimatorController(controller);
            }
        }

        public void SetDummyAnimatorSpeed(float speed)
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].SetAnimatorSpeed(speed);
            }
        }

        void MoveToActiveDimension(bool enableDummies)
        {
            World.Instance.MoveObjectToDimension(this, World.Instance.ActiveDimension.Color);
            for (int i = 0; i < dummies.Count; i++)
            {
                if (World.Instance.Splitted && enableDummies && i != World.Instance.ActiveDimId)
                {
                    dummies[i].gameObject.SetActive(true);
                }
                else
                {
                    dummies[i].gameObject.SetActive(false);
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