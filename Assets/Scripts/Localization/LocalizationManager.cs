
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Events;
using Saving;

namespace Localization
{

    [System.Serializable]
    public struct Setting
    {
        public string language;
        public TextAsset commonPack;
        public TextAsset scenePack;
    }

    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] List<Setting> settings;
        static LocalizationManager _instance;
        string[] languages;
        Dictionary<string, Dictionary<string, string>> languagePack;
        int curLanguage;
        public UnityEvent onLanguageChange;

        public string[] AvailableLanguages
        {
            get => languages;
        }

        public static LocalizationManager Instance
        {
            get => _instance;
        }

        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of LocalizationManager were created");
            _instance = this;
            languagePack = new Dictionary<string, Dictionary<string, string>>();
            curLanguage = -1;
        }

        private void Start()
        {
            languages = new string[settings.Count];
            int i = 0;
            foreach (var setting in settings)
            {
                languages[i] = setting.language;
                if (languages[i].CompareTo(GameSaveManager.Instance.GameSave.language) == 0)
                    curLanguage = i;

                languagePack.Add(setting.language, new Dictionary<string, string>());
                var pairs1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(setting.commonPack.text);
                foreach (var pair in pairs1)
                    languagePack[setting.language].Add(pair.Key, pair.Value);

                if (setting.scenePack.text != null)
                {
                    var pairs2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(setting.scenePack.text);
                    foreach (var pair in pairs2)
                        languagePack[setting.language].Add(pair.Key, pair.Value);
                }
                i += 1;
            }
            if (curLanguage < 0)
                curLanguage = 0;
            onLanguageChange.Invoke();
        }

        public string Get(string key)
        {
            return languagePack[languages[curLanguage]][key];
        }

        public void NextLanguage()
        {
            curLanguage = (curLanguage + 1) % languages.Length;
            GameSaveManager.Instance.GameSave.language = languages[curLanguage];
            onLanguageChange.Invoke();
        }

    }
}

