using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splittable;

namespace Interaction
{
    public class MovingPlatform : Interactable
    {
        [SerializeField] Transform startPoint;
        [SerializeField] Transform endPoint;
        [SerializeField] bool isMovingToEnd = true;
        [SerializeField] float moveSpeed = 1.5f;

        bool isMoving;
        Vector3 startPosition;
        Vector3 endPosition;

        protected new void Awake()
        {
            base.Awake();
            startPosition = transform.TransformPoint(startPoint.localPosition);
            endPosition = transform.TransformPoint(endPoint.localPosition);
        }

        public override void Interact()
        {
            if (!so.IsInCorrectDim()) return;
            StartCoroutine(Move());
        }

        IEnumerator Move()
        {
            InputManager.Instance.pause = true;
            InteractionManager.Instance.transform.SetParent(this.transform);
            isMoving = true;
            Vector3 target = isMovingToEnd ? endPosition : startPosition;
            while (!Util.Fuzzy.CloseVector3(transform.position, target))
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            isMoving = false;
            isMovingToEnd = !isMovingToEnd;
            InteractionManager.Instance.transform.SetParent(this.transform.parent);
            InputManager.Instance.pause = false;
        }

        public void MoveToEnd()
        {
            if (!so.IsInCorrectDim()) return;
            isMovingToEnd = true;
            StartCoroutine(Move());
        }

        public void MoveToStart()
        {
            if (!so.IsInCorrectDim()) return;
            isMovingToEnd = false;
            StartCoroutine(Move());
        }

        public override bool IsInteracting()
        {
            return isMoving;
        }
    }

}
