using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class WorldStatus : MonoBehaviour
    {
        [SerializeField] List<Image> indicatorImages;
        Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Core.World.Instance.OnTransitionStart.AddListener(StartAnim);
            Core.World.Instance.OnTransitionEnd.AddListener(UpdateColor);
            Core.World.Instance.OnActiveDimChange.AddListener(UpdateColor);
        }

        private void StartAnim()
        {
            animator.SetBool("IsSplitted", Core.World.Instance.Splitted);
        }

        private void UpdateColor()
        {
            foreach (var img in indicatorImages)
            {
                img.color = Core.Dimension.MaterialColor[Core.World.Instance.ActiveDimension.color];
            }
        }

    }
}