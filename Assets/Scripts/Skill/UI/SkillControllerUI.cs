using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Skill.UI
{
    public class SkillControllerUI : MonoBehaviour
    {
        [SerializeField] List<SkillSelectionIndicator> selectionIndicators;
        Dictionary<Dimension.Color, SkillSelectionIndicator> map;
        [SerializeField] UnityEngine.UI.Image remainIndicatorPrefab;
        [SerializeField] Color32 remainTint;
        [SerializeField] UnityEngine.UI.GridLayoutGroup remainLayoutGroup;
        [SerializeField] RectTransform remainRectTransform;
        [SerializeField] int remainSpacing = 5;
        List<UnityEngine.UI.Image> remainIndicators;
        int curRemain;

        private void Awake()
        {
            map = new Dictionary<Dimension.Color, SkillSelectionIndicator>();
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                map.Add(indicator.Color, indicator);
            }
        }

        public void Select(Dimension.Color color)
        {
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                if (indicator.Color == color)
                {
                    indicator.Select(true);
                }
                else
                {
                    indicator.Select(false);
                }
            }
        }

        public void Hold(Dimension.Color color)
        {
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                if (indicator.Color == color)
                {
                    indicator.Select(true);
                    indicator.Mask(false);
                }
                else
                {
                    indicator.Select(false);
                    indicator.Mask(true);
                }
            }
        }

        public void MaskExceptFor(Dimension.Color color)
        {
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                if (indicator.Color == color)
                {
                    indicator.Mask(false);
                }
                else
                {
                    indicator.Mask(true);
                }
            }
        }

        public void UnMaskAll()
        {
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                indicator.Mask(false);
            }
        }

        public void UnSelectAll()
        {
            foreach (SkillSelectionIndicator indicator in selectionIndicators)
            {
                indicator.Select(false);
            }
        }


        public void Init(int count)
        {
            remainIndicators = new List<UnityEngine.UI.Image>();
            float width = (remainRectTransform.sizeDelta.x - (count+1)*remainSpacing) / count;
            curRemain = count;
            remainLayoutGroup.cellSize = new Vector2(width, remainLayoutGroup.cellSize.y);
            remainLayoutGroup.spacing = new Vector2(remainSpacing, remainLayoutGroup.spacing.y);
            remainLayoutGroup.padding = new RectOffset(remainSpacing, remainSpacing, 0, 0);
            for (int i = 0; i < count; i++)
            {
                var indicator = GameObject.Instantiate<UnityEngine.UI.Image>(remainIndicatorPrefab);
                indicator.transform.SetParent(remainLayoutGroup.transform, false);
                remainIndicators.Add(indicator);
            }
        }

        public void Add()
        {
            remainIndicators[curRemain].color = Color.white;
            curRemain += 1;
        }

        public void Sub()
        {
            remainIndicators[curRemain-1].color = remainTint;
            curRemain -= 1;
        }

    }
}