
using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class DefaultMaterialColor : MonoBehaviour
    {
        static DefaultMaterialColor _instance;
        public static DefaultMaterialColor Instance {
            get => _instance;
        }
        [SerializeField] List<Dimension.ColorSetting> colorSettings;

        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instance of World created");
            _instance = this;

            if(Dimension.MaterialColor.Count == 0)
            {
                foreach (Dimension.ColorSetting setting in colorSettings)
                {
                    Dimension.MaterialColor.Add(setting.colorTag, setting.color32);
                }
            }
        }
    }
}
