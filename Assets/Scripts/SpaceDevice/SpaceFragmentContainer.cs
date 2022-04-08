using TMPro;
using UnityEngine;

namespace SpaceDevice
{
    public class SpaceFragmentContainer : MonoBehaviour
    {
        [SerializeField] RectTransform cursor;
        [SerializeField] TextMeshProUGUI hintText;

        int cnt;

        int Count
        {
            get => cnt;
            set
            {
                cnt = value;
                cnt = Mathf.Clamp(cnt, 0, 9);
                hintText.text = cnt.ToString();
                cursor.eulerAngles = new Vector3(0, 0, -20 * Count);
            }
        }

        private void Awake()
        {
            Count = 0;
        }

        public void Gain()
        {
            Count += 1;
        }

        public void Lose()
        {
            Count -= 1;
        }

        public bool IsSufficient()
        {
            return cnt > 0;
        }
    }
}
