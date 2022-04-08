
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
        bool isNewGame;

        public static GameSaveManager Instance
        {
            get => _instance;
        }

        public GameSave GameSave
        {
            get => gameSave;
        }

        public bool IsNewGame
        {
            get => isNewGame;
        }


        private void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of GameSaveManager created.");
            _instance = this;
            dest = Application.persistentDataPath + "/save.dat";
            isNewGame = false;
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
                isNewGame = true;
                gameSave = new GameSave(0, 0);
                Save();
            }
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(gameSave);
            File.WriteAllText(dest, json);
        }

        public void NewGame()
        {
            gameSave = new GameSave(0, 0);
            Save();
        }
    }
}
