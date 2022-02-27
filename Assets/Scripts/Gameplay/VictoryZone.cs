using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{

    [RequireComponent(typeof(Collider))]
    public class VictoryZone : Zone
    {
        [SerializeField] string nextScene;

        new void OnTriggerEnter(Collider other)
        {
            TriggerEnter.Invoke(other);
            if (other.gameObject.tag == "Player")
            {
                if (nextScene == null)
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