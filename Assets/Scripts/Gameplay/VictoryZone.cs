using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{

    [RequireComponent(typeof(Collider))]
    public class VictoryZone : MonoBehaviour
    {
        [SerializeField] string nextScene;
        bool entered = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !entered)
            {
                entered = true;
                if (nextScene == null || nextScene == "")
                {
                    SceneLoader.Instance.Load();
                }
                else
                {
                    SceneLoader.Instance.Load(nextScene);
                }
            }
        }
    }
}