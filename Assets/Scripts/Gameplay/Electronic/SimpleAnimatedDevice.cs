using UnityEngine;

namespace Gameplay.Electronic
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class SimpleAnimatedDevice : ElectronicObject
    {
        [SerializeField] Animator doorAnimation;
        [SerializeField] bool _isOpened = true;
        Splittable.SplittableObject so;

        bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;
                doorAnimation.SetBool("IsOpened", IsOpened);
            }
        }

        private void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
            Core.World.Instance.OnDimensionChange.AddListener(OnDimensionChange);
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
