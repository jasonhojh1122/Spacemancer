
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace TimelineEvent.CenterLabAnimation
{
    public class CenterLabAnimationController : MonoBehaviour
    {
        [SerializeField] List<Core.ObjectColor> phase1FixedObjects;
        [SerializeField] List<Core.ObjectColor> phase2FixedObjects;

        private void Start()
        {
            switch (Saving.GameSaveManager.Instance.GameSave.phase)
            {
                case 0:
                    break;
                case 1:
                    foreach (var obj in phase1FixedObjects)
                        obj.Color = Core.Dimension.Color.WHITE;
                    break;
                case 2:
                    foreach (var obj in phase2FixedObjects)
                        obj.Color = Core.Dimension.Color.WHITE;
                    break;
                default:
                    break;
            }
        }
    }
}
