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

        public Transform LeftHandIK {
            get => leftHandIK;
        }
        public Transform LeftHintIK {
            get => leftHintIK;
        }
        public Transform RightHandIK {
            get => rightHandIK;
        }
        public Transform RightHintIK {
            get => rightHintIK;
        }

        public UnityEngine.Events.UnityEvent OnPosed;
        public IKSetting setting;

        float weight;

        public float Weight {
            get => weight;
        }

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

        private void Update()
        {
            if (setting != null)
                SyncIKTargets();
        }

        /// <summary>
        /// Starts the animation of posing towards the <c>IKSetting</c>.
        /// </summary>
        /// <param name="setting"> The <c>IKSetting</c> of the target pose. </param>
        public void Pose(IKSetting setting)
        {
            this.setting = setting;
            StartCoroutine(PoseAnim());
        }

        System.Collections.IEnumerator PoseAnim()
        {
            float t = 0;
            while (t < setting.TransitionTime)
            {
                t += Time.deltaTime;
                var p = t / setting.TransitionTime;
                weight = Mathf.Lerp(0.0f, 1.0f, p);
                yield return null;
            }
            weight = 1.0f;
            OnPosed.Invoke();
        }

        /// <summary>
        /// Starts the animation of ending the current IK pose.
        /// </summary>
        public void EndPose()
        {
            StartCoroutine(EndPoseAnim());
        }

        System.Collections.IEnumerator EndPoseAnim()
        {
            float t = 0;
            while (t < setting.TransitionTime)
            {
                t += Time.deltaTime;
                var p = t / setting.TransitionTime;
                weight = Mathf.Lerp(weight, 0.0f, p);
                yield return null;
            }
        }

        /// <summary>
        /// Synchronizes the IK position.
        /// </summary>
        void SyncIKTargets()
        {
            if (setting.LeftHandTarget != null)
            {
                leftHandIK.position = setting.LeftHandTarget.position;
                leftHandIK.rotation = setting.LeftHandTarget.rotation;
                leftHintIK.position = setting.LeftHintTarget.position;
                leftHintIK.rotation = setting.LeftHintTarget.rotation;
            }
            if (setting.RightHandTarget != null)
            {
                rightHandIK.position = setting.RightHandTarget.position;
                rightHandIK.rotation = setting.RightHandTarget.rotation;
                rightHintIK.position = setting.RightHintTarget.position;
                rightHintIK.rotation = setting.RightHintTarget.rotation;
            }
            SetIKWeight();
        }

        /// <summary>
        /// Synchronizes the IK weight.
        /// </summary>
        void SetIKWeight()
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
