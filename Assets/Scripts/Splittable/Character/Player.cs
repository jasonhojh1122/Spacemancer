using UnityEngine;
using System.Collections.Generic;

using Core;
namespace Splittable.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : SplittableObject
    {
        [SerializeField] PlayerDummy dummyPrefab;

        [Header("IK")]
        [SerializeField] IK.PlayerIK playerIK;
        [SerializeField] IK.IKSetting machineIKSetting;
        [SerializeField] GameObject machine;

        static Player _instance;
        List<PlayerDummy> dummies;
        CharacterController characterController;

        public static Player Instance
        {
            get => _instance;
        }

        public CharacterController Controller
        {
            get => characterController;
        }

        new void Awake()
        {
            base.Awake();

            if (_instance != null)
                Debug.LogError("Multiple instances of Player created.");
            _instance = this;

            characterController = GetComponent<CharacterController>();

            dummies = new List<PlayerDummy>();
            for (int i = 0; i < World.Instance.Dimensions.Count; i++)
            {
                dummies.Add(GameObject.Instantiate<PlayerDummy>(dummyPrefab));
                dummies[i].transform.localPosition = transform.position;
                dummies[i].transform.localRotation = transform.rotation;
                World.Instance.MoveObjectToDimension(dummies[i].gameObject, i);
                dummies[i].gameObject.SetActive(false);
            }
            machine.SetActive(false);
        }

        public override void Split()
        {
            MoveToActiveDimension(true);
            World.Instance.MoveToProcessed(this);
        }

        public override void Merge(SplittableObject parent)
        {
            MoveToActiveDimension(false);
            World.Instance.MoveToProcessed(this);
        }

        public void UpdateDummyTransform()
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].transform.localPosition = transform.localPosition;
                dummies[i].transform.localRotation = transform.localRotation;
            }
        }

        public void SetDummyAnimatorController(RuntimeAnimatorController controller)
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].SetAnimatorController(controller);
            }
        }

        public void SetDummyAnimatorSpeed(float speed)
        {
            for (int i = 0; i < dummies.Count; i++)
            {
                if (!dummies[i].gameObject.activeSelf) continue;
                dummies[i].SetAnimatorSpeed(speed);
            }
        }

        void MoveToActiveDimension(bool enableDummies)
        {
            World.Instance.MoveObjectToDimension(this, World.Instance.ActiveDimension.color);
            for (int i = 0; i < dummies.Count; i++)
            {
                if (World.Instance.Splitted && enableDummies && i != World.Instance.ActiveDimId
                    && World.Instance.Dimensions[i].color != Dimension.Color.NONE)
                {
                    dummies[i].gameObject.SetActive(true);
                }
                else
                {
                    dummies[i].gameObject.SetActive(false);
                }
            }
            UpdateDummyTransform();
        }

        private void FixedUpdate()
        {
            if (World.Instance.Transitting) return;
            if (Dim.color != World.Instance.ActiveDimension.color)
                MoveToActiveDimension(World.Instance.Splitted);
        }

        public void TakeOutSpaceDevice()
        {
            playerIK.OnPosed.AddListener(OnPose);
            playerIK.Pose(machineIKSetting);
        }

        void OnPose()
        {
            machine.SetActive(true);
            playerIK.OnPosed.RemoveListener(OnPose);
        }

        public void PutAwaySpaceDevice()
        {
            machine.SetActive(false);
            playerIK.EndPose();
        }
    }

}