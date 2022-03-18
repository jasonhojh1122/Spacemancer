using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Gameplay.Electronic
{
    public class NewPartStation : SecuredObject
    {
        [SerializeField] Tutorial.VideoTutorial videoTutorial;
        [SerializeField] GameObject newPart;

        bool used = false;

        public override void Toggle()
        {
            TurnOn();
        }

        public override void TurnOn()
        {
            if (used || !so.IsInCorrectDim() || so.Dim.color != activeColor) return;
            IsOpened = true;
            used = true;
            newPart.SetActive(true);
            videoTutorial.Show();
        }

        public override void TurnOff()
        {
            return;
        }


    }
}