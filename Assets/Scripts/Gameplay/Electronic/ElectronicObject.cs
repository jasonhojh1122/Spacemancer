
using UnityEngine;

namespace Gameplay.Electronic
{
    public abstract class ElectronicObject : GameplayObject
    {
        public abstract void TurnOn();
        public abstract void TurnOff();
    }
}
