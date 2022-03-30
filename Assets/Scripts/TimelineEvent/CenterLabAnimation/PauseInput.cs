
using UnityEngine;

namespace TimelineEvent.CenterLabAnimation
{
    public class PauseInput : MonoBehaviour
    {
        private void OnEnable()
        {
            Input.InputManager.Instance.pause = true;
        }

        private void OnDisable()
        {
            Input.InputManager.Instance.pause = false;
        }
    }
}
