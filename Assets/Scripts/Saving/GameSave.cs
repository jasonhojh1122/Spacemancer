using System;

namespace Saving
{
    [Serializable]
    public class GameSave
    {
        public int highestScene;
        public int phase;

        public GameSave(int highestScene, int phase)
        {
            this.highestScene = highestScene;
            this.phase = phase;
        }
    }
}