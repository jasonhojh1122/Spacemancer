using System;

namespace Saving
{
    [Serializable]
    public class GameSave
    {
        public int highestScene;

        public GameSave(int highestScene)
        {
            this.highestScene = highestScene;
        }
    }
}