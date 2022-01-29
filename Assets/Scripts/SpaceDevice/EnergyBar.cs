
using UnityEngine;

using System.Collections;

namespace SpaceDevice
{
    [RequireComponent(typeof(UnityEngine.UI.Slider))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] float decreasePerSec = 0.05f;

        UnityEngine.UI.Slider slider;

        private void Awake()
        {
            slider = GetComponent<UnityEngine.UI.Slider>();
        }


        private void Update()
        {
            if (Core.World.Instance.Splitted)
            {
                slider.value = Mathf.Clamp01(slider.value - decreasePerSec * Time.deltaTime);
                if (Util.Fuzzy.CloseFloat(slider.value, 0))
                {
                    Core.World.Instance.Toggle();
                }
            }
        }

        public void AddEnergy(float amount)
        {

        }


    }


}
