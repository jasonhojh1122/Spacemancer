
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
        [SerializeField] List<GameObject> phase1ShowObjects;
        [SerializeField] List<GameObject> phase2ShowObjects;

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
                    foreach (var obj in phase1ShowObjects)
                        obj.SetActive(true);
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
                    foreach (var obj in phase1ShowObjects)
                        obj.SetActive(true);
                    foreach (var obj in phase2ShowObjects)
                        obj.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
