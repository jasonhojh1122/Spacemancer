
using UnityEngine;

namespace Gameplay.Electronic
{
    public abstract class ElectronicObject : GameplayObject
    {
        protected bool isOn = false;
        public abstract void TurnOn();
        public abstract void TurnOff();
        public abstract void Toggle();
    }
}
