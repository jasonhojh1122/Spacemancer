
using UnityEngine;

namespace TimelineEvent.CenterLabAnimation
{
    public class PlayerWalking : MonoBehaviour
    {
        [SerializeField] Input.PlayerController playerController;

        private void OnEnable()
        {
            playerController.OverrideControl = true;
            playerController.OverrideWalk();
        }

        private void OnDisable()
        {
            playerController.OverrideIdle();
            playerController.OverrideControl = false;
        }
    }
}
