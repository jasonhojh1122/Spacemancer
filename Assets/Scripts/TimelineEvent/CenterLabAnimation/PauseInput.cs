
using UnityEngine;

namespace TimelineEvent.CenterLabAnimation
{
    public class PauseInput : MonoBehaviour
    {
        private void OnEnable()
        {
            if (Input.InputManager.Instance != null)
                Input.InputManager.Instance.pause = true;
        }

        private void OnDisable()
        {
            if (Input.InputManager.Instance != null)
                Input.InputManager.Instance.pause = false;
        }
    }
}
