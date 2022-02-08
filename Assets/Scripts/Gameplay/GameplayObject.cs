
using UnityEngine;

namespace Gameplay
{
    public abstract class GameplayObject : MonoBehaviour
    {
        public abstract void OnColorChange();
        public abstract void OnDimensionChange();
    }
}
