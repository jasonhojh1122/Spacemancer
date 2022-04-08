
using UnityEngine;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Splittable.ErrorSpace))]
    public class ErrorSpaceFixer : Interactable
    {
        [SerializeField] float poseTime = 0.2f;
        static SpaceDevice.SpaceFragmentContainer container;
        Splittable.ErrorSpace errorSpace;
        Collider col;

        void Awake()
        {
            errorSpace = GetComponent<Splittable.ErrorSpace>();
            if (container == null)
                container = FindObjectOfType<SpaceDevice.SpaceFragmentContainer>();
        }

        public override void Interact()
        {
            if (container == null || !container.IsSufficient()) return;
            var dir = transform.position - InteractionManager.Instance.transform.position;
            IKSetting.transform.forward = dir;
            InteractionManager.Instance.PlayerIK.OnPosed.AddListener(OnPose);
            InteractionManager.Instance.PlayerIK.Pose(IKSetting);
        }

        void OnPose()
        {
            InteractionManager.Instance.PlayerIK.OnPosed.RemoveListener(OnPose);
            StartCoroutine(Fix());
        }

        System.Collections.IEnumerator Fix()
        {
            yield return new WaitForSeconds(poseTime);
            InteractionManager.Instance.PlayerIK.EndPose();
            errorSpace.Fix();
            container.Lose();
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }

}