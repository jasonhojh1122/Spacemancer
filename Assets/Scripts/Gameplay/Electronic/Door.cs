using UnityEngine;
using Core;
using System.Collections.Generic;

namespace Gameplay.Electronic
{

    public class Door : SimpleAnimatedDevice
    {
        [SerializeField] Dimension.Color activeColor = Dimension.Color.WHITE;
        [SerializeField] List<ObjectColor> indicators;

        new void Awake()
        {
            base.Awake();
            foreach (var oc in indicators)
            {
                oc.Color = activeColor;
            }
        }

        public override void TurnOn()
        {
            if (so.IsInCorrectDim() && so.Dim.color == activeColor)
                IsOpened = true;
            else
                IsOpened = false;
        }
    }

}