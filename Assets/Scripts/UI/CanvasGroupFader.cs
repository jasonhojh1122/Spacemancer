using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool defaultOn = true;
        [SerializeField] bool pauseGameplayInput = true;
        [SerializeField] GameObject defaultSelected;
        CanvasGroup canvasGroup;
        static EventSystem eventSystem;

        public float Duration
        {
            get => duration;
        }

        bool isOn;

        public bool IsOn {
            get => isOn;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Start()
        {
            if (defaultOn == false)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0;
            }
            else
            {
                if (pauseGameplayInput)
                    Input.InputManager.Instance.ToggleGameplayInput(true);
                SelectDefault();
            }
            isOn = defaultOn;
        }

        public void FadeOut()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isOn = false;
            if (pauseGameplayInput)
                Input.InputManager.Instance.ToggleGameplayInput(false);
            StartCoroutine(Fade(false));
        }

        public void FadeIn()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isOn = true;
            if (pauseGameplayInput)
                Input.InputManager.Instance.ToggleGameplayInput(true);
            SelectDefault();
            StartCoroutine(Fade(true));
        }

        System.Collections.IEnumerator Fade(bool isFadeIn)
        {
            float target = (isFadeIn) ? 1 : 0;
            float start = canvasGroup.alpha;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
                yield return null;
            }
        }

        public void Toggle()
        {
            if (isOn)
                FadeOut();
            else
                FadeIn();
        }

        public void SelectDefault()
        {
            if (defaultSelected != null)
                eventSystem.SetSelectedGameObject(defaultSelected);
        }

    }
}