using UnityEngine;
using TMPro;

namespace Text
{
    [RequireComponent(typeof(TextMeshPro))]
    public class TextColor : MonoBehaviour
    {
        [SerializeField] Core.ObjectColor parentColor;
        TextMeshPro tmPro;
        static Color transparent = new Color(0, 0, 0, 0);
        Color curColor;

        private void Awake()
        {
            tmPro = GetComponent<TextMeshPro>();
            parentColor.OnColorChanged.AddListener(SyncColor);
            Core.World.Instance.OnTransitionStart.AddListener(FadeOut);
            Core.World.Instance.OnTransitionEnd.AddListener(FadeIn);
        }

        private void Start()
        {
            SyncColor();
        }

        public void SyncColor()
        {
            curColor = Core.Dimension.MaterialColor[parentColor.Color];
            tmPro.color = curColor;
        }

        public void FadeOut()
        {
            tmPro.color = transparent;
        }

        public void FadeIn()
        {
            tmPro.color = curColor;
        }
    }
}