
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Saving
{
    public class GameSaveManager : MonoBehaviour
    {
        static GameSaveManager _instance;
        GameSave gameSave;
        string dest;

        public static GameSaveManager Instance
        {
            get => _instance;
        }

        public GameSave GameSave
        {
            get => gameSave;
        }


        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of GameSaveManager created.");
            _instance = this;
            dest = Application.persistentDataPath + "/save.dat";
            Load();
            GameSave.highestScene = Mathf.Max(SceneManager.GetActiveScene().buildIndex, GameSave.highestScene);
            Save();
        }

        void Load()
        {
            if (File.Exists(dest))
            {
                var json = File.ReadAllText(dest);
                gameSave = JsonUtility.FromJson<GameSave>(json);
            }
            else
            {
                gameSave = new GameSave(0);
                Save();
            }
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(gameSave);
            File.WriteAllText(dest, json);
        }
    }
}
