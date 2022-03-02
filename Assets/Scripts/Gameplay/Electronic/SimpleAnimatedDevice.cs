using UnityEngine;

namespace Gameplay.Electronic
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class SimpleAnimatedDevice : ElectronicObject
    {
        [SerializeField] Animator animator;
        [SerializeField] bool _isOpened = true;
        protected Splittable.SplittableObject so;

        protected bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;
                animator.SetBool("IsOpened", IsOpened);
            }
        }

        protected void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
            Core.World.Instance.OnTransitionEnd.AddListener(OnDimensionChange);
        }

        public override void TurnOn()
        {
            if (so.IsInCorrectDim())
                IsOpened = true;
            else
                IsOpened = false;
        }

        public override void TurnOff()
        {
            IsOpened = false;
        }

        public override void OnDimensionChange()
        {
            if (IsOpened && !so.IsInCorrectDim())
                IsOpened = false;
        }

        public override void OnColorChange()
        {
            if (IsOpened && !so.IsInCorrectDim())
                IsOpened = false;
        }
    }
}
