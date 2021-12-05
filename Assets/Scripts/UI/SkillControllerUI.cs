using System.Collections.Generic;
using UnityEngine;
using Core;

public class SkillControllerUI : MonoBehaviour
{
    [SerializeField] List<SkillSelectionIndicator> indicators;
    Dictionary<Dimension.Color, SkillSelectionIndicator> map;

    private void Awake()
    {
        map = new Dictionary<Dimension.Color, SkillSelectionIndicator>();
        foreach (SkillSelectionIndicator indicator in indicators)
        {
            map.Add(indicator.Color, indicator);
        }
    }

    public void Select(Dimension.Color color)
    {
        foreach (SkillSelectionIndicator indicator in indicators)
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
        foreach (SkillSelectionIndicator indicator in indicators)
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
        foreach (SkillSelectionIndicator indicator in indicators)
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
        foreach (SkillSelectionIndicator indicator in indicators)
        {
            indicator.Mask(false);
        }
    }

    public void UnSelectAll()
    {
        foreach (SkillSelectionIndicator indicator in indicators)
        {
            indicator.Select(false);
        }
    }
}
