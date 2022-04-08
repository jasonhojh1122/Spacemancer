
using UnityEngine;

namespace TimelineEvent
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] string sceneName;

        private void OnEnable()
        {
            SceneLoader.Instance.Load(sceneName);
        }
    }
}
