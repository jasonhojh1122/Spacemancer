using System;

namespace Saving
{
    [Serializable]
    public class GameSave
    {
        public int highestScene;
        public int phase;
        public string language;

        public GameSave(int highestScene, int phase, string language)
        {
            this.highestScene = highestScene;
            this.phase = phase;
            this.language = language;
        }
    }
}