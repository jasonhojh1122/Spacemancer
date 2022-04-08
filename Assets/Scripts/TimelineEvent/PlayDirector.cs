using UnityEngine;
using UnityEngine.Playables;

namespace TimelineEvent
{
    public class PlayDirector : MonoBehaviour
    {
        [SerializeField] PlayableDirector director;

        private void OnEnable()
        {
            director.Play();
        }
    }
}
