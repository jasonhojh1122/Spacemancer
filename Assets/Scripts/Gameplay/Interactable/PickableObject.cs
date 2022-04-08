using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class PickableObject : Interactable
    {
        [SerializeField] Vector3 pickUpOffset = new Vector3(0.0f, 0.2f, 0.26f);
        Splittable.SplittableObject so;
        bool picked;

        void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            picked = false;
        }

        public override void Interact()
        {
            if (!so.IsInCorrectDim())
                return;
            if (!picked)
                PickUp();
            else
                PutDown();
        }

        void PickUp()
        {
            picked = true;
            transform.SetParent(InteractionManager.Instance.transform);
            transform.localRotation = Quaternion.identity;
            InteractionManager.Instance.PlayerIK.OnPosed.AddListener(this.OnPosed);
            InteractionManager.Instance.PlayerIK.Pose(IKSetting);
        }

        public void OnPosed()
        {
            InteractionManager.Instance.PlayerIK.OnPosed.RemoveListener(this.OnPosed);
            StartCoroutine(MoveToPlayer());
        }

        IEnumerator MoveToPlayer()
        {
            Input.InputManager.Instance.pause = true;
            float t = 0.0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.parent.TransformPoint(pickUpOffset);
            while (t < IKSetting.TransitionTime)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, t / IKSetting.TransitionTime);
                yield return null;
            }
            transform.localPosition = pickUpOffset;
            Input.InputManager.Instance.pause = false;
        }

        void PutDown()
        {
            picked = false;
            transform.SetParent(InteractionManager.Instance.transform.parent);
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), GetY(), Mathf.RoundToInt(localPos.z));
            transform.localRotation = Quaternion.identity;
            InteractionManager.Instance.PlayerIK.EndPose();
        }

        public override void OnZoneExit(Collider other)
        {
            if(!picked && other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }

        public override bool IsInteracting()
        {
            return picked;
        }

        float GetY()
        {
            var orderedHits = Util.Ray.OrderedHits(transform.position, Vector3.down);
            foreach (var hit in orderedHits)
                if (!hit.collider.isTrigger)
                    return hit.point.y;
            return InteractionManager.Instance.transform.position.y;
        }
    }
}