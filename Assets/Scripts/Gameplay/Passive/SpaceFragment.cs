
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class SpaceFragment : MonoBehaviour
    {
        static SpaceDevice.SpaceFragmentContainer container;
        Collider col;

        private void Awake()
        {
            col = GetComponent<Collider>();
            if (container == null)
                container = FindObjectOfType<SpaceDevice.SpaceFragmentContainer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                container.Gain();
                gameObject.SetActive(false);
            }
        }
    }
}
