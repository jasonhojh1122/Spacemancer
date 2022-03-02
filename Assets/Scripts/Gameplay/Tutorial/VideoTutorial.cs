using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;

namespace Gameplay.Tutorial
{

    [RequireComponent(typeof(UI.CanvasGroupFader))]
    public class VideoTutorial : MonoBehaviour
    {
        [SerializeField] VideoPlayer videoPlayer;
        static EventSystem eventSystem;
        UI.CanvasGroupFader fader;

        private void Awake()
        {
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
            fader = GetComponent<UI.CanvasGroupFader>();
        }

        public void Show()
        {
            Input.InputManager.Instance.pause = true;
            fader.FadeIn();
            videoPlayer.Play();
        }

        public void OnClose()
        {
            fader.FadeOut();
            Input.InputManager.Instance.pause = false;
            videoPlayer.Stop();
        }
    }
}