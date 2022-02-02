using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace IK
{
    public class PlayerIK : MonoBehaviour
    {
        [SerializeField] Transform leftHandIK;
        [SerializeField] Transform leftHintIK;
        [SerializeField] TwoBoneIKConstraint leftArmConstraint;
        [SerializeField] Transform rightHandIK;
        [SerializeField] Transform rightHintIK;
        [SerializeField] TwoBoneIKConstraint rightArmConstraint;

        float weight;

        private void Awake()
        {

            leftArmConstraint.data.targetPositionWeight = 0;
            leftArmConstraint.data.targetRotationWeight = 0;
            leftArmConstraint.data.hintWeight = 0;
            rightArmConstraint.data.targetPositionWeight = 0;
            rightArmConstraint.data.targetRotationWeight = 0;
            rightArmConstraint.data.hintWeight = 0;
            weight = 0;
        }

        public System.Collections.IEnumerator Pose(IKSetting setting)
        {
            float t = 0;
            while (t < setting.TransitionTime)
            {
                t += Time.deltaTime;
                var p = t / setting.TransitionTime;
                weight = Mathf.Lerp(0.0f, 1.0f, p);
                yield return null;
            }
        }

        public void SetIKTarget(IKSetting setting)
        {
            if (setting.LeftHandTarget != null)
            {
                leftHandIK.position = setting.LeftHandTarget.position;
                leftHandIK.rotation = setting.LeftHandTarget.rotation;
                leftHintIK.position = setting.LeftHintTarget.position;
                leftHandIK.rotation = setting.LeftHandTarget.rotation;
            }
            if (setting.RightHandTarget != null)
            {
                rightHandIK.position = setting.RightHandTarget.position;
                rightHandIK.rotation = setting.RightHandTarget.rotation;
                rightHintIK.position = setting.RightHintTarget.position;
                rightHandIK.rotation = setting.RightHandTarget.rotation;
            }
            SetIKWeight(setting);
        }

        void SetIKWeight(IKSetting setting)
        {
            if (setting.LeftHandTarget != null)
            {
                leftArmConstraint.data.targetPositionWeight = weight;
                leftArmConstraint.data.targetRotationWeight = weight;
                leftArmConstraint.data.hintWeight = weight;
            }
            if (setting.RightHandTarget != null)
            {
                rightArmConstraint.data.targetPositionWeight = weight;
                rightArmConstraint.data.targetRotationWeight = weight;
                leftArmConstraint.data.hintWeight = weight;
            }
        }
    }

}
