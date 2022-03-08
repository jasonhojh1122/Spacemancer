using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splittable;

using Gameplay.Interactable;

namespace Gameplay.Electronic
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class MovingPlatform : ElectronicObject
    {
        [SerializeField] Transform startPoint;
        [SerializeField] Transform endPoint;
        [SerializeField] float moveSpeed = 1.5f;

        Splittable.SplittableObject so;
        Vector3 startPosition;
        Vector3 endPosition;

        void Awake()
        {
            so = GetComponent<SplittableObject>();
            startPosition = transform.TransformPoint(startPoint.localPosition);
            endPosition = transform.TransformPoint(endPoint.localPosition);
        }

        public override void TurnOn()
        {
            if (!so.IsInCorrectDim()) return;
            StartCoroutine(Move(endPosition));
        }

        public override void TurnOff()
        {
            if (!so.IsInCorrectDim()) return;
            StartCoroutine(Move(startPosition));
        }

        public override void Toggle()
        {
            if (!so.IsInCorrectDim()) return;
            if (isOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
            isOn = !isOn;
        }

        public override void OnDimensionChange()
        {
            if (!so.IsInCorrectDim()) return;
            StartCoroutine(Move(startPosition));
        }

        public override void OnColorChange()
        {
            if (!so.IsInCorrectDim()) return;
            StartCoroutine(Move(startPosition));
        }

        IEnumerator Move(Vector3 target)
        {
            Input.InputManager.Instance.pause = true;
            Input.CameraController.Instance.FollowPlayer();
            // Splittable.Character.Player.Instance.transform.SetParent(this.transform);
            while (!Util.Fuzzy.CloseVector3(transform.localPosition, target))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
                Splittable.Character.Player.Instance.transform.position = transform.position;
                yield return null;
            }
            Input.CameraController.Instance.UnFollowPlayer();
            // Splittable.Character.Player.Instance.transform.SetParent(this.transform.parent);
            Input.InputManager.Instance.pause = false;
        }

    }

}
