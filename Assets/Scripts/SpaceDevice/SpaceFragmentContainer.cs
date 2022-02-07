using TMPro;
using UnityEngine;

namespace SpaceDevice
{
    public class SpaceFragmentContainer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        int cnt;

        public int Count {
            get => cnt;
            set {
                cnt = value;
                if (cnt < 0) cnt = 0;
                text.text = cnt.ToString();
            }
        }

        private void Awake()
        {
            Count = 0;
        }
    }
}
