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

        bool isMoving;

        void Awake()
        {
            so = GetComponent<SplittableObject>();
            startPosition = transform.TransformPoint(startPoint.localPosition);
            endPosition = transform.TransformPoint(endPoint.localPosition);
            Core.World.Instance.OnTransitionEnd.AddListener(OnDimensionChange);
            isMoving = false;
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
            if (!so.IsInCorrectDim() || isMoving) return;
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
            if (Util.Fuzzy.CloseVector3(transform.localPosition, startPosition))
            {
                isOn = false;
            }
            else
            {
                isOn = true;
            }
            isMoving = false;
        }

        public override void OnColorChange()
        {
            return;
        }

        IEnumerator Move(Vector3 target)
        {
            OnTurnOn.Invoke();
            isMoving = true;
            Input.InputManager.Instance.pause = true;
            Input.CameraController.Instance.FollowPlayer();
            DeathZone.pause = true;
            while (!Util.Fuzzy.CloseVector3(transform.localPosition, target))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
                Splittable.Character.Player.Instance.transform.position = transform.position;
                yield return null;
            }
            DeathZone.pause = false;
            Input.InputManager.Instance.pause = false;
            Input.CameraController.Instance.UnFollowPlayer();
            isMoving = false;
            OnTurnOff.Invoke();
        }

    }

}
