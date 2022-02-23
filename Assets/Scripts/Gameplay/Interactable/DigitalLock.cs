using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Core.ObjectColor))]
    public class DigitalLock : Interactable
    {

        [SerializeField] Electronic.ElectronicObject targetObject;
        [SerializeField] UnityEvent OnUnlock;

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
            StartCoroutine(ScanHand());
        }

        IEnumerator ScanHand()
        {
            yield return new WaitForSeconds(0.5f);
            Open();
            InteractionManager.Instance.PlayerIK.EndPose();
        }

        void Open()
        {
            foreach (var s in Core.World.Instance.ObjectPool.ActiveObjects[targetObject.gameObject.name])
            {
                var electron = s.GetComponent<Electronic.ElectronicObject>();
                electron.TurnOn();
            }
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }
}
