using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;

namespace Gameplay.Tutorial
{

    [RequireComponent(typeof(UI.CanvasGroupFader), typeof(VideoPlayer))]
    public class VideoTutorial : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Button closeButton;
        static EventSystem eventSystem;
        UI.CanvasGroupFader fader;
        VideoPlayer videoPlayer;

        private void Awake()
        {
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
            fader = GetComponent<UI.CanvasGroupFader>();
            videoPlayer = GetComponent<VideoPlayer>();
            closeButton.onClick.AddListener(OnClose);
        }

        public void Show()
        {
            InputManager.Instance.pause = true;
            fader.FadeIn();
            eventSystem.SetSelectedGameObject(closeButton.gameObject);
            videoPlayer.Play();
        }

        public void OnClose()
        {
            fader.FadeOut();
            InputManager.Instance.pause = false;
            videoPlayer.Stop();
        }
    }
}