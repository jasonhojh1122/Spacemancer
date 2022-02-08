using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class Door : GameplayObject
    {
        [SerializeField] Animator doorAnimation;
        Splittable.SplittableObject so;
        bool isClosed;

        private void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
            Core.World.Instance.OnDimensionChange.AddListener(OnDimensionChange);
            isClosed = true;
        }

        public void Open()
        {
            if (so.IsInCorrectDim() && isClosed)
                OpenDoor();
        }

        void OpenDoor()
        {
            doorAnimation.SetTrigger("Open");
            isClosed = false;
        }

        public void Close()
        {
            if (!isClosed)
                CloseDoor();
        }

        void CloseDoor()
        {
            doorAnimation.SetTrigger("Close");
            isClosed = true;
        }

        public override void OnDimensionChange()
        {
            if (!isClosed)
                CloseDoor();
        }

        public override void OnColorChange()
        {
            if (!so.IsInCorrectDim() && !isClosed)
                CloseDoor();
        }
    }
}
