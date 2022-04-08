
using UnityEngine;

namespace IK
{
    public class IKSetting : MonoBehaviour
    {
        [SerializeField] Transform leftHandTarget;
        [SerializeField] Transform leftHintTarget;
        [SerializeField] Transform rightHandTarget;
        [SerializeField] Transform rightHintTarget;
        [SerializeField] float transitionTime = 0.1f;

        public Transform LeftHandTarget {
            get => leftHandTarget;
        }

        public Transform LeftHintTarget {
            get => leftHintTarget;
        }

        public Transform RightHandTarget {
            get => rightHandTarget;
        }

        public Transform RightHintTarget {
            get => rightHintTarget;
        }

        public float TransitionTime {
            get => transitionTime;
        }
    }
}
