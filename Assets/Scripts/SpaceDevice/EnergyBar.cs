
using UnityEngine;

using System.Collections;

namespace SpaceDevice
{
    [RequireComponent(typeof(UnityEngine.UI.Slider))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] float decreasePerSec = 0.05f;
        [SerializeField] float defaultAmount = 0.4f;

        UnityEngine.UI.Slider slider;

        public float Amount
        {
            get => slider.value;
            set => slider.value = Mathf.Max(0, Mathf.Min(value, 1));
        }

        private void Awake()
        {
            slider = GetComponent<UnityEngine.UI.Slider>();
            Amount = defaultAmount;
        }

        public void AddEnergy(float amount)
        {
            Amount += amount;
        }


    }


}
