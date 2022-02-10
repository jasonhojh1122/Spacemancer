using UnityEngine;
using UnityEngine.Events;

using System;

namespace Gameplay.Interactable
{
    [Serializable]
    public class ZoneEvent : UnityEvent<Collider>{}

    [RequireComponent(typeof(Collider))]
    public class Zone : MonoBehaviour
    {

        [SerializeField] ZoneEvent TriggerEnter;
        [SerializeField] ZoneEvent TriggerStay;
        [SerializeField] ZoneEvent TriggerExit;
        Collider col;

        private void Awake()
        {
            col = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter.Invoke(other);
        }
        private void OnTriggerStay(Collider other)
        {
            TriggerStay.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            TriggerExit.Invoke(other);
        }

    }

}