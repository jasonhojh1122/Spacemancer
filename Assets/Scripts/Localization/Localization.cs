
using UnityEngine;
using TMPro;

namespace Localization
{
    public class Localization : MonoBehaviour
    {
        [SerializeField] string key;
        [SerializeField] TextMeshPro tmp;
        [SerializeField] TextMeshProUGUI tmpUGUI;

        private void Awake()
        {
            LocalizationManager.Instance.onLanguageChange.AddListener(this.UpdateText);
        }

        void UpdateText()
        {
            if (tmp != null)
            {
                tmp.text = LocalizationManager.Instance.Get(key);
            }
            if (tmpUGUI != null)
            {
                tmpUGUI.text = LocalizationManager.Instance.Get(key);
            }
        }
    }
}
