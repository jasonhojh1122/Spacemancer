
using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Interactable
{
    public class EnergyChargerManager
    {
        Dictionary<int, bool> isUsed;

        public Dictionary<int, bool> IsUsed {
            get => isUsed;
        }

        public EnergyChargerManager()
        {
            isUsed = new Dictionary<int, bool>();
        }

        public void Register(int id)
        {
            if (!isUsed.ContainsKey(id))
                isUsed.Add(id, false);
        }
    }

    [RequireComponent(typeof(EnergyCharger))]
    public class EnergyCharger : Interactable
    {
        [SerializeField] int id;
        [SerializeField] float energyAmount = 0.2f;
        Splittable.SplittableObject so;
        static SpaceDevice.EnergyBar energyBar;
        static EnergyChargerManager manager = new EnergyChargerManager();

        private void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            if (energyBar == null)
                energyBar = FindObjectOfType<SpaceDevice.EnergyBar>();
            manager.Register(id);
        }

        public override void Interact()
        {
            if (so.IsInCorrectDim() && !manager.IsUsed[id])
            {
                Debug.Log("used");
                energyBar.AddEnergy(energyAmount);
                manager.IsUsed[id] = true;
            }
        }

        public override bool IsInteracting()
        {
            return false;
        }


    }
}
