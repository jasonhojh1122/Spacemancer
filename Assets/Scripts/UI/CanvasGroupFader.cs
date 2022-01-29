using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool defaultOn = true;
        CanvasGroup canvasGroup;

        bool isOn;

        public bool IsOn {
            get => isOn;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (defaultOn == false)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0;
            }
            isOn = defaultOn;
        }

        public void FadeOut()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isOn = false;
            StartCoroutine(Fade(false));
        }

        public void FadeIn()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isOn = true;
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

    }
}