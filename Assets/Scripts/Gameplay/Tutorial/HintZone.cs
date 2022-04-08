using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Tutorial
{
    public class HintZone : MonoBehaviour
    {
        [SerializeField] List<Animator> hintBanner;
        [SerializeField] VideoTutorial videoTutorial;
        [SerializeField] float lastTime = 3.0f;

        bool shown = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!shown && other.tag == "Player")
            {
                if (hintBanner != null)
                {
                    foreach (var banner in hintBanner)
                    {
                        StartCoroutine(Hinting(banner));
                    }
                }
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