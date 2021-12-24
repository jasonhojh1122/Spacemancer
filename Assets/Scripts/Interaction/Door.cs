using UnityEngine;

namespace Interaction
{
    public class Door : Interactable
    {
        [SerializeField] string sceneName;

        public override void Interact()
        {
            if (!string.IsNullOrEmpty(sceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }
}
