using UnityEngine;
using UnityEngine.Animations.Rigging;

using Gameplay.Interactable;
namespace IK
{
    public class PlayerDummyIK : MonoBehaviour
    {
        [SerializeField] Transform leftHandIK;
        [SerializeField] Transform leftHintIK;
        [SerializeField] TwoBoneIKConstraint leftArmConstraint;
        [SerializeField] Transform rightHandIK;
        [SerializeField] Transform rightHintIK;
        [SerializeField] TwoBoneIKConstraint rightArmConstraint;

        private void Awake()
        {

            leftArmConstraint.data.targetPositionWeight = 0;
            leftArmConstraint.data.targetRotationWeight = 0;
            leftArmConstraint.data.hintWeight = 0;
            rightArmConstraint.data.targetPositionWeight = 0;
            rightArmConstraint.data.targetRotationWeight = 0;
            rightArmConstraint.data.hintWeight = 0;
        }

        private void Update()
        {
            if (InteractionManager.Instance.PlayerIK.setting != null)
                SyncIKTargets();
        }

        /// <summary>
        /// Synchronizes the IK position.
        /// </summary>
        void SyncIKTargets()
        {
            if (InteractionManager.Instance.PlayerIK.setting.LeftHandTarget != null)
            {
                leftHandIK.localPosition = InteractionManager.Instance.PlayerIK.LeftHandIK.localPosition;
                leftHandIK.localRotation = InteractionManager.Instance.PlayerIK.LeftHandIK.localRotation;
                leftHintIK.localPosition = InteractionManager.Instance.PlayerIK.LeftHintIK.localPosition;
                leftHintIK.localRotation = InteractionManager.Instance.PlayerIK.LeftHintIK.localRotation;
            }
            if (InteractionManager.Instance.PlayerIK.setting.RightHandTarget != null)
            {
                rightHandIK.localPosition = InteractionManager.Instance.PlayerIK.RightHandIK.localPosition;
                rightHandIK.localRotation = InteractionManager.Instance.PlayerIK.RightHandIK.localRotation;
                rightHintIK.localPosition = InteractionManager.Instance.PlayerIK.RightHintIK.localPosition;
                rightHintIK.localRotation = InteractionManager.Instance.PlayerIK.RightHintIK.localRotation;
            }
            SetIKWeight();
        }

        /// <summary>
        /// Synchronizes the IK InteractionManager.Instance.PlayerIK.Weight.
        /// </summary>
        void SetIKWeight()
        {
            if (InteractionManager.Instance.PlayerIK.setting.LeftHandTarget != null)
            {
                leftArmConstraint.data.targetPositionWeight = InteractionManager.Instance.PlayerIK.Weight;
                leftArmConstraint.data.targetRotationWeight = InteractionManager.Instance.PlayerIK.Weight;
                leftArmConstraint.data.hintWeight = InteractionManager.Instance.PlayerIK.Weight;
            }
            if (InteractionManager.Instance.PlayerIK.setting.RightHandTarget != null)
            {
                rightArmConstraint.data.targetPositionWeight = InteractionManager.Instance.PlayerIK.Weight;
                rightArmConstraint.data.targetRotationWeight = InteractionManager.Instance.PlayerIK.Weight;
                leftArmConstraint.data.hintWeight = InteractionManager.Instance.PlayerIK.Weight;
            }
        }
    }

}
