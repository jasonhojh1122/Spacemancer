
using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Electronic
{

    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class EnergyCharger : SimpleAnimatedDevice
    {
        [SerializeField] int energyAmount = 2;
        [SerializeField] Core.Dimension.Color activeColor;
        [SerializeField] List<Core.ObjectColor> indicators;

        static SpaceDevice.EnergyBar energyBar;
        bool used;

        new void Awake()
        {
            base.Awake();
            used = false;
            so = GetComponent<Splittable.SplittableObject>();
            if (energyBar == null)
                energyBar = FindObjectOfType<SpaceDevice.EnergyBar>();
        }

        private void Start()
        {
            foreach (var i in indicators)
                i.Color = activeColor;
            CheckStatus();
        }

        public override void TurnOn()
        {
            if (!used && so.IsInCorrectDim() && so.Dim.color == activeColor)
            {
                used = true;
                IsOpened = false;
                energyBar.Amount += energyAmount;
                OnTurnOn.Invoke();
            }
        }

        public override void TurnOff()
        {
            return;
        }

        public override void Toggle()
        {
            if (!used)
            {
                TurnOn();
            }
        }

        public override void OnDimensionChange()
        {
            CheckStatus();
        }

        public override void OnColorChange()
        {
            CheckStatus();
        }

        void CheckStatus()
        {
            if (used || !so.IsInCorrectDim() || so.Dim.color != activeColor)
            {
                IsOpened = false;
            }
            else
            {
                IsOpened = true;
            }
        }

    }
}
