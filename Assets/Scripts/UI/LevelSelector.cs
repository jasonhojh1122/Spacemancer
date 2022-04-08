using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

using Saving;

namespace UI
{
    [RequireComponent(typeof(CanvasGroupFader))]
    public class LevelSelector : Input.InputController
    {

        [SerializeField] RectTransform content;
        [SerializeField] List<LevelSelectCard> levelCards;
        [SerializeField] int cardWidth = 420;
        [SerializeField] int cardSpace = 150;
        [SerializeField] float scrollDuration = 0.1f;
        [SerializeField] UnityEngine.Events.UnityEvent OnCardSelected;
        CanvasGroupFader fader;
        InputAction scrollAction;
        InputAction selectAction;
        InputAction closeAction;
        bool scrolling;

        int highestScene;
        int curSelected;

        int CurSelected
        {
            get => curSelected;
            set => curSelected = Mathf.Clamp(value, 0, levelCards.Count-1);
        }

        new void Awake()
        {
            base.Awake();
            fader = GetComponent<CanvasGroupFader>();
            scrollAction = playerInput.actions["Scroll"];
            selectAction = playerInput.actions["Select"];
            closeAction = playerInput.actions["Close"];
            selectAction.performed += CheckSelect;
            closeAction.performed += CheckClose;
            scrolling = false;
        }

        private void Start()
        {
            CurSelected = 0;
            highestScene = GameSaveManager.Instance.GameSave.highestScene;
            for (int i = 0; i < levelCards.Count; i++)
            {
                if (i == 0 && levelCards[i].BuildId > highestScene)
                    CurSelected = 0;
                else if (levelCards[i].BuildId > highestScene)
                    levelCards[i].Locked = true;
                else
                    CurSelected = i;
            }
            levelCards[CurSelected].IsSelected = true;
            fader.OnFadeIn.AddListener(CenterCard);
            CenterCard();
        }

        private void Update()
        {
            if (!fader.IsOn || scrolling) return;
            CheckScroll();
        }

        void CheckScroll()
        {
            var value = scrollAction.ReadValue<float>();
            if (Util.Fuzzy.CloseFloat(value, 0))
                return;

            var oldSelected = CurSelected;
            if (value > 0)
                CurSelected += 1;
            else
                CurSelected -= 1;

            if (CurSelected == oldSelected)
                return;
            else
            {
                levelCards[oldSelected].IsSelected = false;
                scrolling = true;
                StartCoroutine(ScrollAnim());
            }
        }

        void CheckSelect(InputAction.CallbackContext context)
        {
            if (fader.IsOn && !levelCards[CurSelected].Locked)
            {
                StopAllCoroutines();
                fader.FadeOut();
                SceneLoader.Instance.Load(levelCards[CurSelected].BuildId);
            }
        }

        void CheckClose(InputAction.CallbackContext context)
        {
            if (fader.IsOn)
            {
                StopAllCoroutines();
                fader.FadeOut();
            }
        }

        void CenterCard()
        {
            content.anchoredPosition = GetAnchoredPos();
        }

        System.Collections.IEnumerator ScrollAnim()
        {
            float curPos = content.anchoredPosition.x;
            Vector2 targetPos = GetAnchoredPos();
            Vector2 startPos = content.anchoredPosition;
            float t = 0;

            while (t < scrollDuration)
            {
                t += Time.deltaTime;
                content.anchoredPosition = Vector2.Lerp(startPos, targetPos, t/scrollDuration);
                yield return null;
            }
            levelCards[CurSelected].IsSelected = true;
            scrolling = false;
            OnCardSelected.Invoke();
        }

        Vector2 GetAnchoredPos()
        {
            return new Vector2(-CurSelected * (cardWidth + cardSpace), 0);
        }
    }
}
