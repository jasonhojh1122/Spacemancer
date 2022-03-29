
using UnityEngine;

namespace Gameplay.Electronic
{
    public abstract class ElectronicObject : GameplayObject
    {
        [SerializeField] public UnityEngine.Events.UnityEvent OnTurnOn;
        [SerializeField] public UnityEngine.Events.UnityEvent OnTurnOff;
        protected bool isOn = false;
        public abstract void TurnOn();
        public abstract void TurnOff();
        public abstract void Toggle();
    }
}
