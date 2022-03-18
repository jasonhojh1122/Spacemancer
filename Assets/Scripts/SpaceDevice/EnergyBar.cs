
using UnityEngine;

using System.Collections.Generic;

namespace SpaceDevice
{
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] List<UnityEngine.UI.Image> lights;
        [SerializeField] [Range(0, 5)] int defaultAmount;
        [SerializeField] Color32 lightColor;
        [SerializeField] UnityEngine.UI.Slider hintSlider;

        static EnergyBar _instance;
        int amount;
        bool dirty;
        float speed = 0.6f;
        Color32 transparent = new Color32(0, 0, 0, 0);


        public int Amount
        {
            get => amount;
            set
            {
                int previous = Amount;
                amount = Mathf.Clamp(value, 0, 5);
                if (previous > Amount)
                {
                    for (int i = previous; i > Amount; i--)
                        lights[i-1].color = transparent;
                }
                else
                {
                    for (int i = previous; i < Amount; i++)
                        lights[i].color = lightColor;
                }
                dirty = true;
            }
        }

        public static EnergyBar Instance
        {
            get => _instance;
        }

        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of EnergyBar created");
            _instance = this;
            dirty = false;
            Amount = defaultAmount;
            for (int i = 0; i < lights.Count; i++)
            {
                if (i < Amount)
                    lights[i].color = lightColor;
                else
                    lights[i].color = transparent;
            }
        }

        private void Start()
        {
            hintSlider.value = Percentage();
        }

        private void Update()
        {
            if (dirty)
            {
                float target = Percentage();
                hintSlider.value = Mathf.MoveTowards(hintSlider.value, target, speed*Time.deltaTime);
                if (Util.Fuzzy.CloseFloat(hintSlider.value, target))
                {
                    dirty = false;
                    hintSlider.value = target;
                }
            }
        }

        float Percentage()
        {
            return (float)amount / (float)lights.Count;
        }

        /// <summary>
        /// Costs the energy for single action.
        /// </summary>
        public void CostSingleAction()
        {
            Amount -= 1;
        }

        /// <summary>
        /// If the energy is sufficient for a single action.
        /// </summary>
        /// <returns> True if the energy is sufficient. </returns>
        public bool IsSufficient()
        {
            return Amount > 0;
        }

    }


}
