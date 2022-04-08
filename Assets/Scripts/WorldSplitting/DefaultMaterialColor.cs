
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core
{
    public class DefaultMaterialColor : MonoBehaviour
    {
        static DefaultMaterialColor _instance;
        public static DefaultMaterialColor Instance {
            get => _instance;
        }

        [Serializable]
        public struct ColorSetting
        {
            public Dimension.Color colorTag;
            public UnityEngine.Color32 color32;
        }


        [SerializeField] List<ColorSetting> colorSettings;

        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instance of DefaultMaterialColor created");
            _instance = this;

            if(Dimension.MaterialColor.Count == 0)
            {
                foreach (ColorSetting setting in colorSettings)
                {
                    Dimension.MaterialColor.Add(setting.colorTag, setting.color32);
                }
            }
        }
    }
}
