
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace TimelineEvent.CenterLabAnimation
{
    public class CenterLabAnimationController : MonoBehaviour
    {
        [SerializeField] List<Core.ObjectColor> phase1FixedObjects;
        [SerializeField] List<Core.ObjectColor> phase2FixedObjects;
        [SerializeField] List<GameObject> phase1HideObjects;
        [SerializeField] List<GameObject> phase2HideObjects;

        private void Start()
        {
            switch (Saving.GameSaveManager.Instance.GameSave.phase)
            {
                case 0:
                    break;
                case 1:
                    foreach (var obj in phase1FixedObjects)
                        obj.Color = Core.Dimension.Color.WHITE;
                    foreach (var obj in phase1HideObjects)
                        obj.SetActive(false);
                    break;
                case 2:
                    foreach (var obj in phase1FixedObjects)
                        obj.Color = Core.Dimension.Color.WHITE;
                    foreach (var obj in phase2FixedObjects)
                        obj.Color = Core.Dimension.Color.WHITE;
                    foreach (var obj in phase1HideObjects)
                        obj.SetActive(false);
                    foreach (var obj in phase1HideObjects)
                        obj.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
