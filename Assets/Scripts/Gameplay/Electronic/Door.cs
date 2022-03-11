using UnityEngine;
using Core;
using System.Collections.Generic;

namespace Gameplay.Electronic
{

    public class Door : SecuredObject
    {
        public override void TurnOn()
        {
            if (so.IsInCorrectDim() && so.Dim.color == activeColor)
                IsOpened = true;
            else
                IsOpened = false;
        }
    }

}