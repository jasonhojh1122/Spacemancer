using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Electronic
{

    public class Stabilizer : ElectronicObject
    {
        [SerializeField] List<Core.ObjectColor> toStabilized;
        bool used = false;

        public override void TurnOn()
        {
            if (!used)
            {
                used = true;
                foreach (var oc in toStabilized)
                {
                    oc.Color = Core.Dimension.Color.WHITE;
                }
            }

        }

        public override void TurnOff()
        {

        }

        public override void Toggle()
        {

        }

        public override void OnColorChange()
        {

        }

        public override void OnDimensionChange()
        {

        }
    }
}