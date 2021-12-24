using UnityEngine;

namespace Skill.UI
{
    public class SkillSelectionIndicator : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Image colorImage;
        [SerializeField] GameObject selectionIndicator;
        [SerializeField] GameObject mask;
        [SerializeField] Core.Dimension.Color color;

        public Core.Dimension.Color Color
        {
            get => color;
        }

        private void Start()
        {
            colorImage.color = Core.Dimension.MaterialColor[color];
            selectionIndicator.SetActive(false);
            mask.SetActive(false);
        }

        public void Select(bool isOn)
        {
            selectionIndicator.SetActive(isOn);
        }

        public void Mask(bool isOn)
        {
            mask.SetActive(isOn);
        }
    }
}