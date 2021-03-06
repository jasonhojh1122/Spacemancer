using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Core.ObjectColor))]
    public class DigitalLock : Interactable
    {

        [SerializeField] Electronic.ElectronicObject targetObject;
        [SerializeField] UnityEvent OnScanHand;
        [SerializeField] UnityEvent OnUnlock;
        [SerializeField] bool singleUse = false;

        Core.ObjectColor objectColor;

        private void Awake()
        {
            objectColor = GetComponent<Core.ObjectColor>();
        }

        public override void Interact()
        {
            if (objectColor.Color == Core.World.Instance.ActiveDimension.color)
            {
                InteractionManager.Instance.PlayerIK.OnPosed.AddListener(this.OnPosed);
                InteractionManager.Instance.PlayerIK.Pose(IKSetting);
            }
        }

        public void OnPosed()
        {
            InteractionManager.Instance.PlayerIK.OnPosed.RemoveListener(this.OnPosed);
            if (singleUse)
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
            StartCoroutine(ScanHand());
        }

        IEnumerator ScanHand()
        {
            OnScanHand.Invoke();
            yield return new WaitForSeconds(0.5f);
            Open();
            InteractionManager.Instance.PlayerIK.EndPose();
        }

        void Open()
        {
            if (targetObject != null)
            {
                foreach (var s in Core.World.Instance.ObjectPool.ActiveObjects[targetObject.gameObject.name])
                {
                    if (s.Dim.color == Splittable.Character.Player.Instance.Dim.color)
                    {
                        var electron = s.GetComponent<Electronic.ElectronicObject>();
                        electron.Toggle();
                    }
                }
            }
            OnUnlock.Invoke();
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }
}
