using UnityEngine;

namespace Gameplay.Tutorial
{
    public class HintZone : MonoBehaviour
    {
        [SerializeField] Animator text;
        [SerializeField] Animator key;
        [SerializeField] VideoTutorial videoTutorial;
        [SerializeField] float lastTime = 3.0f;

        bool shown = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!shown && other.tag == "Player")
            {
                if (text != null)
                    StartCoroutine(Hinting(text));
                if (key != null)
                    StartCoroutine(Hinting(key));
                if (videoTutorial != null)
                    videoTutorial.Show();
                shown = true;
            }
        }

        System.Collections.IEnumerator Hinting(Animator animator)
        {
            animator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(lastTime);
            animator.SetTrigger("FadeOut");
        }

    }

}