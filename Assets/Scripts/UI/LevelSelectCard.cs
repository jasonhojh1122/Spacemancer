using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelSelectCard : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Image indicator;
        [SerializeField] UnityEngine.UI.Image lockMask;
        [SerializeField] int buildId;

        bool isSelected = false;
        bool locked = false;

        public int BuildId
        {
            get => buildId;
        }

        public bool Locked
        {
            get => locked;
            set
            {
                locked = value;
                lockMask.gameObject.SetActive(Locked);
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                indicator.gameObject.SetActive(isSelected);
            }
        }

        private void Awake()
        {
            IsSelected = false;
        }
    }

}
